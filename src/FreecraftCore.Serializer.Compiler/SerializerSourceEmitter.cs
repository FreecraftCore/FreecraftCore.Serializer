using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// This object can be used to emit the source code for serialization
	/// of specific types.
	/// </summary>
	public sealed class SerializerSourceEmitter
	{
		/// <summary>
		/// The assembly to search and emit serialization code for.
		/// </summary>
		public IEnumerable<INamedTypeSymbol> TargetAssembly { get; }

		public Compilation CompilationUnit { get; }

		public ISerializationSourceOutputStrategy SerializationOutputStrategy { get; }

		private HashSet<string> GeneratedTypeNames { get; } = new();

		private List<ITypeSymbol> RequiredGenericSerializers { get; } = new List<ITypeSymbol>();

		public SerializerSourceEmitter(IEnumerable<INamedTypeSymbol> targetAssembly, [NotNull] ISerializationSourceOutputStrategy serializationOutputStrategy, [NotNull] Compilation compilationUnit)
		{
			TargetAssembly = targetAssembly ?? throw new ArgumentNullException(nameof(targetAssembly));
			SerializationOutputStrategy = serializationOutputStrategy ?? throw new ArgumentNullException(nameof(serializationOutputStrategy));
			CompilationUnit = compilationUnit ?? throw new ArgumentNullException(nameof(compilationUnit));
		}

		public void Generate(CancellationToken token)
		{
			//Find all serializable types that are serializable from/to
			INamedTypeSymbol[] serializableTypes = TargetAssembly
				.Where(IsSerializableType)
				.ToArray();

			foreach (INamedTypeSymbol type in serializableTypes)
			{
				if (token.IsCancellationRequested)
					return;

				//THIS SHOULD NEVER HAPPEN!
				//if (type.IsGenericType && !type.IsUnboundGenericType)
				//	throw new InvalidOperationException($"Encountered unbound generic Type: {type.Name} in top-level search.");

				//This is for GENERIC types.
				//We can only realistically support closed generic forward declared types
				//AND also primitive generic type support.
				if (type.IsGenericType)
				{
					if (type.HasAttributeExact<PrimitiveGenericAttribute>())
					{
						foreach (Type[] gta in PrimitiveGenericAttribute.Instance.GetPermutations(type.Arity))
						{
							if (token.IsCancellationRequested)
								return;

							//Represents an array of type arguments for symbols
							ITypeSymbol[] genericTypeArgSymbols = gta.Select(t => CompilationUnit.GetTypeByMetadataName(t.FullName))
								.Cast<ITypeSymbol>()
								.ToArray();

							INamedTypeSymbol boundGenericType = type.Construct(genericTypeArgSymbols);
							WriteSerializerStrategyClass(boundGenericType, token);
						}
					}

					//If it's marked with KnownGeneric attribute then we should generate
					//a serializer for each closed generic type.
					if(type.HasAttributeExact<KnownGenericAttribute>())
					{
						foreach (AttributeData data in type.GetAttributesExact<KnownGenericAttribute>())
						{
							if (token.IsCancellationRequested)
								return;

							//Skip, this is include attribute. Maybe warn?
							if (data.ConstructorArguments.IsDefaultOrEmpty)
								continue;

							//TODO: Does this work? Arrays may work different with API
							//create types for each tuple of generic attributes
							INamedTypeSymbol boundGenericType = type.Construct(data.ConstructorArguments.First().Values.Select(v => v.Value).Cast<ITypeSymbol>().ToArray());
							WriteSerializerStrategyClass(boundGenericType, token);
						}
					}

					if (token.IsCancellationRequested)
						return;

					if (type.HasAttributeExact<ClosedGenericAttribute>())
					{
						foreach (AttributeData closedTypeAttri in type.GetAttributesExact<ClosedGenericAttribute>())
						{
							if (token.IsCancellationRequested)
								return;

							WriteSerializerStrategyClass((INamedTypeSymbol)closedTypeAttri.ConstructorArguments.First().Value, token);
						}
					}

#warning TODO: Reimplement BaseGenericListAttribute support
					//To support custom generic lists we created this open-ended base attribute
					//and we just create combinations from it.
					/*foreach (BaseGenericListAttribute genericAttri in type.GetCustomAttributes<BaseGenericListAttribute>())
					{
						//Iterate the generic type list and we can build.
						//closed generic types for all variants
						foreach(IEnumerable<Type> genericParameterList in genericAttri.Permutations(type.GetGenericParameterCount()))
							WriteSerializerStrategyClass(type.MakeGenericType(genericParameterList.ToArray()));
					}*/
				}
				else
					WriteSerializerStrategyClass(type, token);
			}

			//It's important that we dynamically emit generic serializers
			//if none are found in refences or soon-to-be emitted serializers

			//copy because something will modify this array and then we need to continue until it's empty
			//because we could have an object graph that goes deep into multiple generics.
			int genericDepth = 0;
			do
			{
				if (token.IsCancellationRequested)
					return;

				ITypeSymbol[] genericTypeSymbols = RequiredGenericSerializers.ToArray();
				RequiredGenericSerializers.Clear();
				EmitRequestedGenericTypeSerializers(genericTypeSymbols, token);
				genericDepth++;

				//This loop is kinda dangerous, but we assume it'll eventually terminate.
			} while (RequiredGenericSerializers.Any() && genericDepth < 10);

			if (genericDepth >= 10)
				throw new InvalidOperationException($"Automatic generic type serialization depth exceed maximum depth.");
		}

		private void EmitRequestedGenericTypeSerializers(ITypeSymbol[] genericTypeSymbols, CancellationToken token)
		{
			foreach (ITypeSymbol genericTypeSymbol in genericTypeSymbols)
			{
				if (token.IsCancellationRequested)
					return;

				GeneratedSerializerNameStringBuilder builder = new GeneratedSerializerNameStringBuilder(genericTypeSymbol);
				string genericSerializerName = builder.BuildName();

				//If an emitted serializer name matches the expected generic serializer name
				//then one is going to be emitted so we should not do anything.
				if (GeneratedTypeNames.Contains(genericSerializerName))
					continue;

				//TODO: Don't hardcore default namespace!!
				//Now we check if one exists in the compilation unit or references
				INamedTypeSymbol symbol = CompilationUnit.GetTypeByMetadataName($"FreecraftCore.Serializer.{genericSerializerName}");
				if (symbol != null)
					continue;

				//TODO: Calling this may end up having us require MORE generic serializers but this case isn't handled yet.
				//At this point we don't have a generic serializer defined for the closed generic type
				//and we should just emit one otherwise compilation will fail.
				WriteSerializerStrategyClass((INamedTypeSymbol) genericTypeSymbol, token);
			}
		}

		private static bool IsSerializableType(INamedTypeSymbol symbol)
		{
			//We don't serialize enums!
			if (symbol.IsEnumType())
				return false;

			//TODO: We don't actually support non-abstract polymorphic serialization yet
			//Either it has a subtype value or it's NOT abstract. Abstracts can't be deserialized without base type linking
			//so we don't generate serialization code for them.
			//Imagine as: wireContractAttribute != null && (wireContractAttribute.OptionalSubTypeKeySize.HasValue || !t.IsAbstract);
			return symbol
				.GetAttributes()
				.Any(a =>
				{
					//TODO: If we ever change WireDataContractAttribute CTOR this breaks
					return a.AttributeClass.IsTypeLike<WireDataContractAttribute>() && (!symbol.IsAbstract || a.ConstructorArguments.Any());
				});
		}

		private void WriteSerializerStrategyClass(INamedTypeSymbol typeSymbol, CancellationToken token)
		{
			try
			{
				if (token.IsCancellationRequested)
					return;

				ICompilationUnitEmittable implementationEmittable = CreateEmittableImplementationSerializerStrategy(typeSymbol, token);

				//This cased issues in Analyzer/Generator due to dependency loading
				//SyntaxNode implementationFormattedNode = Formatter.Format(implementationEmittable.CreateUnit(), new AdhocWorkspace());

				if (token.IsCancellationRequested)
					return;

				WriteEmittedFile(implementationEmittable.CreateUnit().NormalizeWhitespace("\t", true), implementationEmittable, token);


				if (token.IsCancellationRequested)
					return;

				RequiredGenericSerializers.AddRange(implementationEmittable.GetRequestedGenericTypes());
			}
			catch (Exception e)
			{
				//We should hint to end user what Type failed and where.
				throw new InvalidOperationException($"Type: {typeSymbol.Name} encountered compilation failure. Reason: {e}", e);
			}
		}

		private void WriteEmittedFile(SyntaxNode formattedNode, ICompilationUnitEmittable emittable, CancellationToken token)
		{
			StringBuilder sb = new StringBuilder();
			using (TextWriter classFileWriter = new StringWriter(sb))
				formattedNode.WriteTo(classFileWriter);

			if (token.IsCancellationRequested)
				return;

			SerializationOutputStrategy.Output($"{emittable.UnitName}", sb.ToString());
			GeneratedTypeNames.Add(emittable.UnitName);
		}

		private ICompilationUnitEmittable CreateEmittableImplementationSerializerStrategy(INamedTypeSymbol type, CancellationToken token)
		{
			//Abstract types ALWAYS must be polymorphic
			if (type.IsAbstract)
			{
				return new PolymorphicSerializerImplementationCompilationUnitEmitter(type, CompilationUnit, token);
			}
			else
			{
				return new RegularSerializerImplementationCompilationUnitEmitter(type, token);
			}
		}
	}
}
