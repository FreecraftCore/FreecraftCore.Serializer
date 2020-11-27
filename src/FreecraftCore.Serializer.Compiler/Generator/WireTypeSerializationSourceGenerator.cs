using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace FreecraftCore.Serializer
{
	//See: https://github.com/dotnet/roslyn-sdk/blob/master/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs
	[Generator]
	public class WireTypeSerializationSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			
		}
	}
}
