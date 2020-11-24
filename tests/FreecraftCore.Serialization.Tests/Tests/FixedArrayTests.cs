using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization
{
	[TestFixture]
	public class FixedArrayTests
	{
		[Test]
		[TestCase(5)]
		public static void Test_Can_Serialize_FixedArray_Type(int size)
		{
			//arrange
			byte[] bytes = new byte[size];
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			FixedSizePrimitiveArrayTypeSerializerStrategy<byte, Static_Int32_5>.Instance.Write(bytes, buffer, ref offset);

			//assert
			Assert.AreEqual(new Static_Int32_5().Value, offset);
			for(int i = 0; i < size; i++)
				Assert.AreEqual(bytes[i], buffer[i]);
		}

		[Test]
		[TestCase(1)]
		[TestCase(256)]
		[TestCase(3)]
		public static void Test_Cannot_Serialize_FixedArray_Type_Incorrect_Length(int size)
		{
			//arrange
			byte[] bytes = new byte[size];
			int offset = 0;

			//act
			Assert.Throws<InvalidOperationException>(() =>
			{
				Span<byte> buffer = new Span<byte>(new byte[1024]);
				FixedSizePrimitiveArrayTypeSerializerStrategy<byte, Static_Int32_5>.Instance.Write(bytes, buffer, ref offset);
			});
		}

		[Test]
		[TestCase(5)]
		public static void Test_Can_Deserialize_FixedArray_Type(int size)
		{
			//arrange
			byte[] bytes = new byte[size];
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			FixedSizePrimitiveArrayTypeSerializerStrategy<byte, Static_Int32_5>.Instance.Write(bytes, buffer, ref offset);
			offset = 0;
			byte[] output = FixedSizePrimitiveArrayTypeSerializerStrategy<byte, Static_Int32_5>.Instance.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(size, output.Length);
			for(int i = 0; i < size; i++)
				Assert.AreEqual(bytes[i], output[i]);
		}

		public class Static_Int32_5 : StaticTypedNumeric<int>
		{
			public override int Value => 5;
		}
	}
}
