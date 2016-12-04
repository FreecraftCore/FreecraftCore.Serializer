using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	[TestFixture]
	public static class ComplexTypeSerializerStrategyTests
	{
		public static IEnumerable<ITypeSerializerStrategy> serializers = new List<ITypeSerializerStrategy>() { new ByteSerializerStrategy(), new UInt32SerializerStrategy(), new UInt16SerializerStrategy() };

		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());

			//assert
			Assert.DoesNotThrow(() => new ComplexTypeSerializerStrategy<TestTypeClass>(factory));
		}

		[Test]
		public static void Test_Complex_Type_Has_Correct_Type_Prop()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(factory);

			//act
			bool result = serializer.SerializerType == typeof(TestTypeClass);

			//assert
			Assert.AreEqual(true, result);
		}

		[Test]
		public static void Test_Complex_Type_Writes_Non_Null_Bytes()
		{
			//arrange
			byte[] result = GetBytes(5042, 230);

			//assert
			Assert.NotNull(result);
			Assert.IsNotEmpty(result);
		}

		private static byte[] GetBytes(uint a, byte b)
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(factory);

			//act
			using (var writer = new DefaultWireMemberWriterStrategy())
			{
				serializer.Write(new TestTypeClass(a, b), writer);

				return writer.GetBytes();
			}
		}

		[Test]
		public static void Test_Complex_Type_Reads_Bytes_It_Serializes()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(factory);
			byte[] bytes = GetBytes(5024, 203);
			TestTypeClass result = null;

			//act
			using (var reader = new DefaultWireMemberReaderStrategy(bytes))
			{
				result = serializer.Read(reader);
			}

			//assert
			Assert.NotNull(result);
			Assert.AreEqual(result.a, 5024);
			Assert.AreEqual(result.b, 203);
			Assert.AreEqual(result.c, 203 + 5024);
		}

		[Test]
		public static void Test_Complex_Type_Serialization_With_Array()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			SerializerService service = new SerializerService();
			service.RegisterType<TestArrayClass>();
			service.Compile();

			//act
			TestArrayClass test = new TestArrayClass(new int[] { 1, 2, 5, 7, 1054, 6632 });
			TestArrayClass deserializedInstance = service.Deserialize<TestArrayClass>(service.Serialize(test));

			//assert
			//check all array values
			for (int i = 0; i < test.values.Length; i++)
				Assert.AreEqual(test.values[i], deserializedInstance.values[i], $"Failed index {i}.");
		}

		[Test]
		public static void Test_Complex_Type_Serialization_With_Array_Of_Known_Size()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			SerializerService service = new SerializerService();
			service.RegisterType<TestArrayClassKnownSize>();
			service.Compile();

			//act
			TestArrayClassKnownSize test = new TestArrayClassKnownSize(new int[] { 1, 2, 5, 7, 1054 });
			TestArrayClassKnownSize deserializedInstance = service.Deserialize<TestArrayClassKnownSize>(service.Serialize(test));

			//assert
			//check all array values
			for (int i = 0; i < test.values.Length; i++)
				Assert.AreEqual(test.values[i], deserializedInstance.values[i], $"Failed index {i}.");
		}

		[Test]
		public static void Test_Complex_Type_Serialization_With_Array_Of_Complex_Types_Of_Known_Size()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			SerializerService service = new SerializerService();
			service.RegisterType<TestArrayOfComplexTypes>();
			service.Compile();

			//act
			TestArrayOfComplexTypes test = new TestArrayOfComplexTypes(new TestArrayClassKnownSize[] { new TestArrayClassKnownSize(new int[] { 1, 2, 5, 7, 1054 }), new TestArrayClassKnownSize(new int[] { 6, 3, 5, 0, 10523 }) });
			TestArrayOfComplexTypes deserializedInstance = service.Deserialize<TestArrayOfComplexTypes>(service.Serialize(test));

			//assert
			//check all array values
			for (int i = 0; i < test.values.Length; i++)
				for(int j = 0; j < test.values[i].values.Length; j++)
				{
					Assert.AreEqual(test.values[i].values[j], deserializedInstance.values[i].values[j]);
				}
		}
		[Test]
		public static void Test_Complex_Type_Serialization_With_Array_Of_Enum()
		{
			//arrange
			ISerializerFactory factory = new SerializerFactory(serializers, new DefaultSerializerDecoratorService());
			SerializerService service = new SerializerService();
			service.RegisterType<TestArrayClassEnum>();
			service.Compile();

			//act
			TestArrayClassEnum test = new TestArrayClassEnum(new TestEnum[] { TestEnum.None, TestEnum.Test, TestEnum.None, TestEnum.None, TestEnum.Test });
			TestArrayClassEnum deserializedInstance = service.Deserialize<TestArrayClassEnum>(service.Serialize(test));

			//assert
			//check all array values
			for (int i = 0; i < test.values.Length; i++)
				Assert.AreEqual(test.values[i], deserializedInstance.values[i], $"Failed index {i}.");
		}


		public class TestArrayClass
		{
			[WireMember(1)]
			public int[] values;

			public TestArrayClass(int[] providedValues)
			{
				values = providedValues;
			}

			public TestArrayClass()
			{

			}
		}

		public class TestArrayClassEnum
		{
			[WireMember(1)]
			public TestEnum[] values;

			public TestArrayClassEnum(TestEnum[] providedValues)
			{
				values = providedValues;
			}

			public TestArrayClassEnum()
			{

			}
		}

		public class TestArrayClassKnownSize
		{
			[KnownSize(5)]
			[WireMember(1)]
			public int[] values;

			public TestArrayClassKnownSize(int[] providedValues)
			{
				values = providedValues;
			}

			public TestArrayClassKnownSize()
			{

			}
		}

		public class TestArrayOfComplexTypes
		{
			[KnownSize(5)]
			[WireMember(1)]
			public TestArrayClassKnownSize[] values;

			public TestArrayOfComplexTypes(TestArrayClassKnownSize[] providedValues)
			{
				values = providedValues;
			}

			public TestArrayOfComplexTypes()
			{

			}
		}

		public enum TestEnum : byte
		{
			None = 0,
			Test = 1
		}

		[WireMessage]
		public class TestTypeClass
		{
			[WireMember(1)]
			public uint a;

			[WireMember(2)]
			public byte b;

			[WireMember(3)]
			public UInt16 c { get; private set; }

			[WireMember(4)]
			public TestEnum enumTest;

			public TestTypeClass(uint aVal, byte bVal)
			{
				a = aVal;
				b = bVal;
				c = (ushort)((uint)a + (uint)b);
			}

			public TestTypeClass()
			{
			}
		}
	}
}
