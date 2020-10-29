using System;

namespace FreecraftCore.Serializer.Compiler.ManualTest
{
	class Program
	{
		static void Main(string[] args)
		{
			SerializerSourceEmitter emitter = new SerializerSourceEmitter(typeof(SerializerSourceEmitter).Assembly, "");
			emitter.Generate();
		}
	}
}
