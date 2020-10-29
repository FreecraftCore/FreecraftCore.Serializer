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
			string value = Serializer.Read(buffer, ref offset);
			TerminatorSerializer.Read(buffer, ref offset);

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
		[TestCase("Les Mise\u0301rables")]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings(string input)
		{
			//arrange
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//WARNING: When reversing NON-ASCII strings like Unicode we CANNOT naively reverse them
			//act
			Serializer.Write(new string(input.Reverse().ToArray()), buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;

			//WARNING: When reversing NON-ASCII strings like Unicode we CANNOT naively reverse them
			//ReverseBinaryMutatorStrategy.Instance.Mutate(buffer, ref offset, buffer, ref offset);
			string reversedString = new string(input.Reverse().ToArray());
			offset = 0;

			Assert.AreEqual(reversedString, Serializer.Read(buffer, ref offset));
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
			byte[] trueEncodedBytes = Encoding.UTF8.GetBytes(input);

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0, $"Bytes: {bytes.Aggregate("", (s, b) => $"{s} {b}")}");

			Assert.AreEqual(trueEncodedBytes.Length, bytes.Length, $"TBytes: {trueEncodedBytes.Aggregate("", (s, b) => $"{s} {b}")}\n Bytes: {bytes.Aggregate("", (s, b) => $"{s} {b}")}");
		}
	}
}
