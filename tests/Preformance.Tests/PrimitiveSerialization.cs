using FreecraftCore.Serializer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;


namespace Preformance.Tests
{
	public class PrimitiveSerialization
	{
		static void Main([NotNull] string[] args)
		{
			//first
			TestSingleInt testInstance = new TestSingleInt(5);
			//
			TestMoreComplexType complex = new TestMoreComplexType(new int[] {1, 2, 3, 4, 5, 6});//7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, });
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSingleInt>();
			serializer.RegisterType<TestMoreComplexType>();
			serializer.Compile();
			serializer.Serialize(new TestSingleInt(5));
			ProtoBuf.Serializer.Serialize(new MemoryStream(), new TestSingleInt(5));
			ProtoBuf.Serializer.Serialize(new MemoryStream(), complex);
			Stopwatch serializerWatch = new Stopwatch();

			/*
			#region SimpleSingleIntTest
			//arrange
			using (MemoryStream stream = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(stream, new TestSingleInt(5));
			}

			
			GC.Collect();
			serializerWatch.Start();

			for (int i = 0; i < 100000; i++)
			{
				serializer.Serialize(testInstance);
			}
			serializerWatch.Stop();

			Console.WriteLine($"{serializerWatch.ElapsedTicks} - Time for FreecraftCore Serializer");

			GC.Collect();
			serializerWatch.Reset();
			serializerWatch.Start();


			for (int i = 0; i < 100000; i++)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, new TestSingleInt(5));
					stream.Position = 0;
				}
			}

			serializerWatch.Stop();

			Console.WriteLine($"{serializerWatch.ElapsedTicks} - Time for Protobuf-net Serializer");

			Console.ReadKey();
			#endregion
			*/

			//so the cache is built
			serializer.Serialize(complex);

			GC.Collect();
			serializerWatch.Reset();
			serializerWatch.Start();
			
			for (int i = 0; i < 100000; i++)
			{
				serializer.Serialize(complex);
			}
			serializerWatch.Stop();

			Console.WriteLine($"{serializerWatch.ElapsedTicks} - Time for FreecraftCore Serializer");

			GC.Collect();
			serializerWatch.Reset();
			serializerWatch.Start();

			for (int i = 0; i < 100000; i++)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, complex);
					stream.Position = 0;
				}
			}

			serializerWatch.Stop();

			Console.WriteLine($"{serializerWatch.ElapsedTicks} - Time for Protobuf-net Serializer");

			TestMoreComplexType test = serializer.Deserialize<TestMoreComplexType>(serializer.Serialize(serializer.Deserialize<TestMoreComplexType>(serializer.Serialize(complex))));

			for (int i = 0; i < test.intArray.Length; i++)
			{
				Console.WriteLine($"{complex.intArray[i]} vs {test.intArray[i]}");

				if (complex.intArray[i] != test.intArray[i])
					throw new Exception();
			}
				

			Console.ReadKey();
		}

		public void Test_Single_Int_Serialization_Of_Class()
		{

		}

		[ProtoContract]
		[WireDataContract]
		public class TestSingleInt
		{
			[ProtoMember(1)]
			[WireMember(1)]
			public int a;

			public TestSingleInt(int aVal)
			{
				a = aVal;
			}

			public TestSingleInt()
			{

			}
		}

		[ProtoContract]
		[WireDataContract]
		public class TestMoreComplexType
		{
			[ProtoMember(1)]
			[WireMember(1)]
			public int[] intArray;

			[ProtoMember(2)]
			[WireMember(2)]
			public TestSingleInt nestedComplex = new TestSingleInt(5);

			[ProtoMember(3)]
			[WireMember(3)]
			public TestEnum testEnum = TestEnum.Blah;

			public TestMoreComplexType(int[] array)
			{
				intArray = array;
			}

			public TestMoreComplexType()
			{

			}
		}

		public enum TestEnum
		{
			None,
			Blah,
			Something
		}
	}
}
