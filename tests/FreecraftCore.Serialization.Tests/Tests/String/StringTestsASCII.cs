using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsASCII
	{
		private static IBaseEncodableTypeSerializerStrategy Serializer { get; } = ASCIIStringTypeSerializerStrategy.Instance;

		[Test]
		public static void Test_String_Serializer_Serializes()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write("Hello!", buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;
			string value = Serializer.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(value, "Hello!");
		}

		[Test]
		public static void Test_String_Serializer_Can_Serialize_Empty_String()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write(String.Empty, buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;
			string value = Serializer.Read(buffer, ref offset);

			//Change was made here that makes null strings empty strings
			//This seems preferable and easier to deal with. Nullrefs are bad
			//and also serializing null is harder than serializing empty
			//this is overall less error prone.
			//assert
			Assert.NotNull(value);
		}

		[Test]
		public static void Test_String_With_Null_Terminator_Is_Same()
		{
			//arrange
			Assert.AreEqual("Hello", "Hello".PadRight(5, '\0'));

			Assert.AreEqual("Hello".Length, "Hello".PadRight(5, '\0').Length);
		}

		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[5 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write("hello", buffer, ref offset);
		}

		[Test]
		public static void Test_Fixed_String_Can_Write_Proper_Length()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[5 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write("hello", buffer, ref offset);

			//WARNING: Old test assumed serializer wrote null terminator by default. That's done at source generation time now!!
			//assert
			Assert.AreEqual(5 * Serializer.CharacterSize, offset);
		}

		[Test]
		public static void Test_Fixed_String_Can_Read()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[5 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write("hello", buffer, ref offset);
			offset = 0;
			string value = Serializer.Read(buffer, ref offset);

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual("hello", value);
		}

		[Test]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[5 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			Serializer.Write("hello", buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;
			ReverseBinaryMutatorStrategy.Instance.Mutate(buffer, ref offset, buffer, ref offset);

			//WARNING: Old test assumed serializer wrote null terminator by default. That's done at source generation time now!!
			Assert.AreEqual("olleH", new string(Serializer.EncodingStrategy.GetChars(buffer.ToArray())));
		}

		[Test]
		public static void Test_Can_Deserialize_To_String_With_Only_Null_Terminator()
		{
			//arrange
			int offset = 0;

			//act
			string value = Serializer.Read(new Span<byte>(new byte[Serializer.CharacterSize]), ref offset);

			//assert
			Assert.NotNull(value, "String was null.");
			Assert.True(value.Length == 0);
		}

		[Test]
		public static void Test_Can_Serialize_To_String_With_Only_Null_Terminator()
		{
			//arrange
			var serializer = ASCIIStringTypeSerializerStrategy.Instance;
			Span<byte> buffer = new Span<byte>(new byte[5 * Serializer.CharacterSize]);
			int offset = 0;

			//act
			serializer.Write(String.Empty, buffer, ref offset);

			//assert
			Assert.True(offset == 0);
			Assert.True(buffer[0] == 0);
		}
	}
}
