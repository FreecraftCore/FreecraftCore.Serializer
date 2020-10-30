﻿using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests
{
	[TestFixture]
	public class StringLengthPrefixTests
	{
		[TestCase("Hello")]
		[TestCase("Hello!")]
		[TestCase("Testing")]
		[TestCase("WooOooOoOo")]
		public static void Can_Serialize_Length_Prefixed_String(string value)
		{
			//arrange
			var serializer = LengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, ASCIIStringTerminatorTypeSerializerStrategy, int>.Instance;
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			serializer.Write(value, buffer, ref offset);

			//assert
			Assert.AreNotEqual(0, offset);
			Assert.AreNotEqual(0, buffer[0]);
		}

		[TestCase("Hello")]
		[TestCase("Hello!")]
		[TestCase("Testing")]
		[TestCase("WooOooOoOo")]
		public static void Can_Serializer_Encodes_Correct_Length(string value)
		{
			//arrange
			var serializer = LengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, ASCIIStringTerminatorTypeSerializerStrategy, int>.Instance;
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			serializer.Write(value, buffer, ref offset);
			offset = 0;
			int length = GenericTypePrimitiveSerializerStrategy<int>.Instance.Read(buffer, ref offset);

			//assert
			Assert.AreNotEqual(0, length);
			Assert.AreEqual(value.Length + 1, length); //SendSize default sends terminator in size
		}

		[TestCase("Hello")]
		[TestCase("Hello!")]
		[TestCase("Testing")]
		[TestCase("WooOooOoOo")]
		public static void Can_Serializer_Encodes_Correct_Length_Byte(string value)
		{
			//arrange
			var serializer = LengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, ASCIIStringTerminatorTypeSerializerStrategy, byte>.Instance;
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			serializer.Write(value, buffer, ref offset);
			offset = 0;
			byte length = GenericTypePrimitiveSerializerStrategy<byte>.Instance.Read(buffer, ref offset);

			//assert
			Assert.AreNotEqual(0, length);
			Assert.AreEqual(value.Length + 1, length); //SendSize default sends terminator in size
		}

		[TestCase("Hello")]
		[TestCase("Hello!")]
		[TestCase("Testing")]
		[TestCase("WooOooOoOo")]
		public static void Can_Serializer_Deserialize_To_Equivalent_String(string value)
		{
			//arrange
			var serializer = LengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, ASCIIStringTerminatorTypeSerializerStrategy, int>.Instance;
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			serializer.Write(value, buffer, ref offset);
			offset = 0;
			string result = serializer.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(value, result);
		}

		//Want to make sure that reading is independent of null terminators.
		[TestCase("Hello")]
		[TestCase("Hello!")]
		[TestCase("Testing")]
		[TestCase("WooOooOoOo")]
		public static void Can_Serializer_Deserialize_To_Equivalent_String_Large_Null_Terminator_Trailing_String(string value)
		{
			//arrange
			var serializer = LengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, ASCIIStringTerminatorTypeSerializerStrategy, int>.Instance;
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			serializer.Write(value, buffer, ref offset);
			offset = 0;
			GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(value.Length * 2, buffer, ref offset);
			offset = 0;
			string result = serializer.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(value, result);
		}
	}
}
