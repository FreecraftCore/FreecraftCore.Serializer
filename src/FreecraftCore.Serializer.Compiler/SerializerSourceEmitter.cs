using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

		public void Generate()
		{
			//Find all serializable types that aren't abstracts.
			Type[] serializableTypes = TargetAssembly
				.GetTypes()
				.Where(t => t.GetCustomAttribute<WireDataContractAttribute>() != null && !t.IsAbstract)
				.ToArray();

			List<CompilationUnitSyntax> compilationUnits = new List<CompilationUnitSyntax>(serializableTypes.Length);

			string rootPath = Path.Combine(OutputPath, $"Strategy");
			if (!Directory.Exists(rootPath))
				Directory.CreateDirectory(rootPath);

			foreach (var type in serializableTypes)
			{
				WriteSerializerStrategyClass(type);
			}
		}

		private void WriteSerializerStrategyClass(Type type)
		{
			ICompilationUnitEmittable templateEmittable = CreateEmittableTemplateSerializerStrategy(type);
			ICompilationUnitEmittable implementationEmittable = CreateEmittableImplementationSerializerStrategy(type);
			SyntaxNode templateFormattedNode = Formatter.Format(templateEmittable.CreateUnit(), new AdhocWorkspace());
			SyntaxNode implementationFormattedNode = Formatter.Format(implementationEmittable.CreateUnit(), new AdhocWorkspace());

			WriteEmittedFile(templateFormattedNode, templateEmittable, "Root");
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

		private static ICompilationUnitEmittable CreateEmittableTemplateSerializerStrategy(Type type)
		{
			Type serializerTypeEmitter = typeof(SerializerTemplateCompilationUnitEmitter<>)
				.MakeGenericType(type);

			return (ICompilationUnitEmittable) Activator.CreateInstance(serializerTypeEmitter);
		}

		private static ICompilationUnitEmittable CreateEmittableImplementationSerializerStrategy(Type type)
		{
			Type serializerTypeEmitter = typeof(SerializerImplementationCompilationUnitEmitter<>)
				.MakeGenericType(type);

			return (ICompilationUnitEmittable)Activator.CreateInstance(serializerTypeEmitter);
		}
	}
}
