using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization
{
	[TestFixture]
	public static class BoolSerializerTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public static void Test_Generic_Primitive_Boolean_Serializer_Write_1_Byte(bool value)
		{
			//arrange
			GenericTypePrimitiveSerializerStrategy<Boolean> serializer = new GenericTypePrimitiveSerializerStrategy<bool>();
			Span<byte> bytes = new Span<byte>(new byte[4]);
			int offset = 0;

			//act
			serializer.Write(value, bytes, ref offset);

			//assert
			Assert.AreEqual(1, offset);
		}

		[Test]
		public static void Test_Generic_Primitive_Boolean_Serializer_Read_1_Byte()
		{
			//arrange
			GenericTypePrimitiveSerializerStrategy<Boolean> serializer = new GenericTypePrimitiveSerializerStrategy<bool>();
			Span<byte> bytes = new Span<byte>(new byte[4]);
			int offset = 0;

			//act
			bool value = serializer.Read(bytes, ref offset);

			//assert
			Assert.AreEqual(1, offset);
		}
	}
}
