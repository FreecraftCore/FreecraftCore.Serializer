using System;
using System.Collections.Concurrent;
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
		public static ConcurrentDictionary<string, Assembly> AssemblyLoadedMap { get; } = new ConcurrentDictionary<string, Assembly>();

		static WireGenerator()
		{
/*#if DEBUG
			if(!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif*/

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
		}

		public static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string simpleName = args.Name.Split(',').First();
			if (AssemblyLoadedMap.ContainsKey(simpleName))
				return AssemblyLoadedMap[simpleName];

			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Split(',').First() == simpleName);
		}

		private static void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			AssemblyLoadedMap[args.LoadedAssembly.GetName().Name] = args.LoadedAssembly;
		}

		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				// TODO: WARNING: This is a MEGA HACK to try to get this source generator not to run within the IDE
				// and ONLY on external builds. It's a disaster for perf in the IDE and I cannot invest time to solve that
				// Hopefully this is a good enough hacky short-term solution.
				// The entry assembly in the IDE will be "csc"
				if (Assembly.GetEntryAssembly()?.GetName().Name.ToLowerInvariant().Contains("csc") == true)
					return;

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
