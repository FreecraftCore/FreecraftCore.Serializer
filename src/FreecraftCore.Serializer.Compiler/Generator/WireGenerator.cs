using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	[Generator]
	public sealed class WireGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
#if DEBUG
			if(!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				INamedTypeSymbol[] symbols = context
					.Compilation
					.Assembly
					.GlobalNamespace
					.GetAllTypes()
					.Where(t => t.ContainingAssembly != null && t.ContainingAssembly.Equals(context.Compilation.Assembly, SymbolEqualityComparer.Default))
					.ToArray();

				ISerializationSourceOutputStrategy outputStrategy = new ExternalContentCollectorSerializationSourceOutputStrategy();
				SerializerSourceEmitter emitter = new SerializerSourceEmitter(symbols, outputStrategy, context.Compilation);
				emitter.Generate();

				foreach (var entry in outputStrategy.Content)
					context.AddSource(entry.Key, entry.Value);
			}
			catch (System.Reflection.ReflectionTypeLoadException e)
			{
				context.AddSource("Error.txt", $"{e}\n\nLoader: {e.LoaderExceptions.Select(ex => ex.ToString()).Aggregate((s1, s2) => $"{s1}\n{s2}")}");
				throw;
			}
			catch (Exception e)
			{
				context.AddSource("Error.txt", e.ToString());

				try
				{
					context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, e.StackTrace));
				}
				finally
				{
					throw e;
				}
			}
		}
	}
}
