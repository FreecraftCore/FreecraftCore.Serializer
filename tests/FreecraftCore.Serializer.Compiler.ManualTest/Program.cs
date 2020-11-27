using System;
using FreecraftCore.Serializer.Tests;

namespace FreecraftCore.Serializer.Compiler.ManualTest
{
	class Program
	{
		static void Main(string[] args)
		{
			SerializerSourceEmitter emitter = new SerializerSourceEmitter(typeof(SerializerSourceEmitter).Assembly, "");
			emitter.Generate();

			//emitter = new SerializerSourceEmitter(typeof(AuthLogonPacketTests).Assembly, "");
			//emitter.Generate();
		}
	}
}
