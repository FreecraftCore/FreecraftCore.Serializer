using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsUTF8
	{
		public const int NULL_TERMINATOR_SIZE = 1;

		private static IBaseEncodableTypeSerializerStrategy Serializer { get; } = UTF8StringTypeSerializerStrategy.Instance;
		private static IBaseEncodableTypeSerializerStrategy TerminatorSerializer { get; } = UTF8StringTerminatorTypeSerializerStrategy.Instance;

		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			Serializer.Write("hello", buffer, ref offset);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Fixed_String_Can_Write_Proper_Length(string input)
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			Serializer.Write(input, buffer, ref offset);
			TerminatorSerializer.Write(input, buffer, ref offset);

			//assert
			Assert.AreEqual(Encoding.UTF8.GetByteCount(input) + NULL_TERMINATOR_SIZE, offset);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Fixed_String_Can_Read(string input)
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			Serializer.Write(input, buffer, ref offset);
			TerminatorSerializer.Write(input, buffer, ref offset);

			offset = 0;
			TerminatorSerializer.Read(buffer, ref offset);
			string value = Serializer.Read(buffer, ref offset);

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual(input, value);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings(string input)
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			Serializer.Write(input, buffer, ref offset);

			Assert.AreEqual(new string(input.Reverse().ToArray()), new string(Serializer.EncodingStrategy.GetChars(buffer.Slice(0, offset).ToArray())));
		}

		//Don't terminate is the DEFAULT now.
		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_DontTerminate_Serializer_Doesnt_Add_Terminator(string input)
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			Serializer.Write(input, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0);

			Assert.True(bytes.Length == Encoding.UTF8.GetBytes(input).Length);
		}
	}
}
