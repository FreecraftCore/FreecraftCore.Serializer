using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
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

		public SerializerSourceEmitter(IEnumerable<INamedTypeSymbol> targetAssembly, [NotNull] ISerializationSourceOutputStrategy serializationOutputStrategy, [NotNull] Compilation compilationUnit)
		{
			TargetAssembly = targetAssembly ?? throw new ArgumentNullException(nameof(targetAssembly));
			SerializationOutputStrategy = serializationOutputStrategy ?? throw new ArgumentNullException(nameof(serializationOutputStrategy));
			CompilationUnit = compilationUnit ?? throw new ArgumentNullException(nameof(compilationUnit));
		}

		public void Generate()
		{
			//Find all serializable types that are serializable from/to
			INamedTypeSymbol[] serializableTypes = TargetAssembly
				.Where(IsSerializableType)
				.ToArray();

			foreach (INamedTypeSymbol type in serializableTypes)
			{
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
						foreach (Type[] gta in PrimitiveGenericAttribute.Instance
							.Permutations(type.Arity)
							.Select(em => em.ToArray())
							.ToArray())
						{
							//Represents an array of type arguments for symbols
							ITypeSymbol[] genericTypeArgSymbols = gta.Select(t => CompilationUnit.GetTypeByMetadataName(t.FullName))
								.Cast<ITypeSymbol>()
								.ToArray();

							INamedTypeSymbol boundGenericType = type.Construct(genericTypeArgSymbols);
							WriteSerializerStrategyClass(boundGenericType);
						}
					}

					//If it's marked with KnownGeneric attribute then we should generate
					//a serializer for each closed generic type.
					if(type.HasAttributeExact<KnownGenericAttribute>())
					{
						foreach (AttributeData data in type.GetAttributesExact<KnownGenericAttribute>())
						{
							//Skip, this is include attribute. Maybe warn?
							if (data.ConstructorArguments.IsDefaultOrEmpty)
								continue;

							//TODO: Does this work? Arrays may work different with API
							//create types for each tuple of generic attributes
							INamedTypeSymbol boundGenericType = type.Construct(data.ConstructorArguments.First().Values.Select(v => v.Value).Cast<ITypeSymbol>().ToArray());
							WriteSerializerStrategyClass(boundGenericType);
						}
					}

					if (type.HasAttributeExact<ClosedGenericAttribute>())
					{
						foreach (AttributeData closedTypeAttri in type.GetAttributesExact<ClosedGenericAttribute>())
							WriteSerializerStrategyClass((INamedTypeSymbol) closedTypeAttri.ConstructorArguments.First().Value);
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
					WriteSerializerStrategyClass(type);
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

		private void WriteSerializerStrategyClass(INamedTypeSymbol typeSymbol)
		{
			try
			{
				ICompilationUnitEmittable implementationEmittable = CreateEmittableImplementationSerializerStrategy(typeSymbol);
				//This cased issues in Analyzer/Generator due to dependency loading
				SyntaxNode implementationFormattedNode = Formatter.Format(implementationEmittable.CreateUnit(), new AdhocWorkspace());

				WriteEmittedFile(implementationFormattedNode, implementationEmittable, "");
			}
			catch (Exception e)
			{
				//We should hint to end user what Type failed and where.
				throw new InvalidOperationException($"Type: {typeSymbol.Name} encountered compilation failure. Reason: {e}", e);
			}
		}

		private void WriteEmittedFile(SyntaxNode formattedNode, ICompilationUnitEmittable emittable, string appendedName)
		{
			StringBuilder sb = new StringBuilder();
			using (TextWriter classFileWriter = new StringWriter(sb))
				formattedNode.WriteTo(classFileWriter);

			if (!String.IsNullOrWhiteSpace(appendedName))
				SerializationOutputStrategy.Output($"{emittable.UnitName}_{appendedName}", sb.ToString());
			else
				SerializationOutputStrategy.Output($"{emittable.UnitName}", sb.ToString());
		}

		private ICompilationUnitEmittable CreateEmittableImplementationSerializerStrategy(INamedTypeSymbol type)
		{
			//Abstract types ALWAYS must be polymorphic
			if (type.IsAbstract)
			{
				return new PolymorphicSerializerImplementationCompilationUnitEmitter(type, CompilationUnit);
			}
			else
			{
				return new RegularSerializerImplementationCompilationUnitEmitter(type);
			}
		}
	}
}
