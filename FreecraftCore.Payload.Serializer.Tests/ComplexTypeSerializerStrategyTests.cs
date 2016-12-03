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
			//assert
			Assert.DoesNotThrow(() => new ComplexTypeSerializerStrategy<TestTypeClass>(serializers));
		}

		[Test]
		public static void Test_Complex_Type_Has_Correct_Type_Prop()
		{
			//arrange
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(serializers);

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
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(serializers);

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
			ComplexTypeSerializerStrategy<TestTypeClass> serializer = new ComplexTypeSerializerStrategy<TestTypeClass>(serializers);
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

		[WireMessage]
		public class TestTypeClass
		{
			[WireMember(1)]
			public uint a;

			[WireMember(2)]
			public byte b;

			[WireMember(3)]
			public UInt16 c { get; private set; }

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
