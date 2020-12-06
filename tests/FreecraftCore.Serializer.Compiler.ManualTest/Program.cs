using System;
using System.Collections.Generic;
using System.IO;
using FreecraftCore.Serializer.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FreecraftCore.Serializer.Compiler.ManualTest
{
	class Program
	{
		static void Main(string[] args)
		{
			CSharpCompilation compilation = CSharpCompilation.Create("Test", new Microsoft.CodeAnalysis.SyntaxTree[]
			{
				CSharpSyntaxTree.ParseText(File.ReadAllText(@"C:\Users\Glader\Documents\Github\FreecraftCore\FreecraftCore.Serializer\tests\FreecraftCore.Serializer.Compiler.ManualTest\WoWAuthDTOs.cs"))
			}, new MetadataReference[]
			{
				MetadataReference.CreateFromFile(typeof(WireDataContractAttribute).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(SerializerService).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Int32).Assembly.Location), 
				MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location), 
			});

			SerializerSourceEmitter emitter = new SerializerSourceEmitter(compilation.GetAllTypes(), new WriteToFileSerializationSourceOutputStrategy(""), compilation);
			emitter.Generate();

			//emitter = new SerializerSourceEmitter(typeof(AuthLogonPacketTests).Assembly, "");
			//emitter.Generate();

			//emitter = new SerializerSourceEmitter(typeof(FreecraftCore.Serializer.Perf.BaseLogonProofResult).Assembly, new WriteToFileSerializationSourceOutputStrategy(""));
			//emitter.Generate();
		}
	}
}
