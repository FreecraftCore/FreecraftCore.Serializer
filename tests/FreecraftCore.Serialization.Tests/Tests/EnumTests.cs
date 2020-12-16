using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class EnumTests
	{
		[Test]
		public static void Test_Serializer_Can_Read_Write_Type_With_Enum_Field()
		{
			//arrange
			var serializer = GenericPrimitiveEnumTypeSerializerStrategy<TestEnum, byte>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			serializer.Write(TestEnum.Ok, buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;
			var resultValue = serializer.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(resultValue, TestEnum.Ok);
		}

		public enum TestEnum : byte
		{
			Ok,
			No,
			Something
		}
		
		public enum TestEnum2 : byte
		{

		}
	}
}
