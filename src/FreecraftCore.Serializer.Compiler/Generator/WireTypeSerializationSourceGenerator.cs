using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	[Generator]
	public sealed class WireTypeSerializationSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				var outputStrategy = new ExternalContentCollectorSerializationSourceOutputStrategy();
				SerializerSourceEmitter emitter = new SerializerSourceEmitter(context.Compilation.GetAllTypes(), outputStrategy, context.Compilation);
				emitter.Generate();

				foreach(var entry in outputStrategy.Content)
					context.AddSource(entry.Key, entry.Value);
			}
			catch (Exception e)
			{
				context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
			}
		}

		private static string BuildStackTrace(Exception e)
		{
			return e.StackTrace
				.Replace('{', ' ')
				.Replace('}', ' ')
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Skip(2)
				.Aggregate((s1, s2) => $"{s1} {s2}");
		}
	}
}
