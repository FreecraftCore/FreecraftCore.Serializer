using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
		public Assembly TargetAssembly { get; }

		/// <summary>
		/// The output path for serializers.
		/// </summary>
		public string OutputPath { get; }

		public SerializerSourceEmitter(Assembly targetAssembly, string outputPath)
		{
			TargetAssembly = targetAssembly ?? throw new ArgumentNullException(nameof(targetAssembly));
			OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
		}

		public SerializerSourceEmitter([NotNull] string targetAssemblyPath, [NotNull] string outputPath)
		{
			if (string.IsNullOrWhiteSpace(targetAssemblyPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetAssemblyPath));
			if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputPath));

			TargetAssembly = Assembly.LoadFile(targetAssemblyPath);
			OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
		}

		public void Generate()
		{
			//Find all serializable types that aren't abstracts.
			Type[] serializableTypes = TargetAssembly
				.GetTypes()
				.Where(IsSerializableType)
				.ToArray();

			string rootPath = Path.Combine(OutputPath, $"Strategy");
			if (!Directory.Exists(rootPath))
				Directory.CreateDirectory(rootPath);

			foreach (var type in serializableTypes)
			{
				//This is for GENERIC types.
				//We can only realistically support closed generic forward declared types
				//AND also primitive generic type support.
				if (type.IsGenericType)
				{
					//If it's marked with KnownGeneric attribute then we should generate
					//a serializer for each closed generic type.
					var knownGenericAttris = type.GetCustomAttributes<KnownGenericAttribute>();
					foreach (var gta in knownGenericAttris)
						WriteSerializerStrategyClass(type.MakeGenericType(gta.GenericTypeParameters));

					foreach (ClosedGenericAttribute closedTypeAttri in type.GetCustomAttributes<ClosedGenericAttribute>())
						WriteSerializerStrategyClass(closedTypeAttri.GenericTypeParameter);

					//To support custom generic lists we created this open-ended base attribute
					//and we just create combinations from it.
					foreach (BaseGenericListAttribute genericAttri in type.GetCustomAttributes<BaseGenericListAttribute>())
					{
						//Iterate the generic type list and we can build.
						//closed generic types for all variants
						foreach(IEnumerable<Type> genericParameterList in genericAttri.Permutations(type.GetGenericParameterCount()))
							WriteSerializerStrategyClass(type.MakeGenericType(genericParameterList.ToArray()));
					}
				}
				else
					WriteSerializerStrategyClass(type);
			}
		}

		private static bool IsSerializableType(Type t)
		{
			WireDataContractAttribute wireContractAttribute = t.GetCustomAttribute<WireDataContractAttribute>();

			//TODO: We don't actually support non-abstract polymorphic serialization yet
			//Either it has a subtype value or it's NOT abstract. Abstracts can't be deserialized without base type linking
			//so we don't generate serialization code for them.
			return wireContractAttribute != null && (wireContractAttribute.OptionalSubTypeKeySize.HasValue || !t.IsAbstract);
		}

		private void WriteSerializerStrategyClass(Type type)
		{
			ICompilationUnitEmittable implementationEmittable = CreateEmittableImplementationSerializerStrategy(type);
			SyntaxNode implementationFormattedNode = Formatter.Format(implementationEmittable.CreateUnit(), new AdhocWorkspace());

			WriteEmittedFile(implementationFormattedNode, implementationEmittable, "Impl");
		}

		private void WriteEmittedFile(SyntaxNode formattedNode, ICompilationUnitEmittable emittable, string appendedName)
		{
			StringBuilder sb = new StringBuilder();
			using (TextWriter classFileWriter = new StringWriter(sb))
				formattedNode.WriteTo(classFileWriter);

			string rootPath = Path.Combine(OutputPath, $"Strategy");

			//Write the packet file out
			File.WriteAllText(Path.Combine(rootPath, $"{emittable.UnitName}_{appendedName}.cs"), sb.ToString());
		}

		private static ICompilationUnitEmittable CreateEmittableImplementationSerializerStrategy(Type type)
		{
			Type serializerTypeEmitter = !type.IsAbstract
				? typeof(RegularSerializerImplementationCompilationUnitEmitter<>).MakeGenericType(type)
				: typeof(PolymorphicSerializerImplementationCompilationUnitEmitter<>).MakeGenericType(type);

			return (ICompilationUnitEmittable)Activator.CreateInstance(serializerTypeEmitter);
		}
	}
}
