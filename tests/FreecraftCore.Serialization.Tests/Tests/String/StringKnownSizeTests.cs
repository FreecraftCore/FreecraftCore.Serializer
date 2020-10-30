using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests
{
	[TestFixture]
	public class StringKnownSizeTests
	{
		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void Can_Serialize_KnownSize_Strings(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			KnownSizeStringSerializerHelper.Write(value, buffer, ref offset, fixedSize, encoding, shouldTerminate);

			//assert
			Assert.AreNotEqual(0, offset);
			Assert.AreNotEqual(0, buffer[0]);
		}

		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void Can_Serializer_Deserialize_To_Equivalent_String(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			KnownSizeStringSerializerHelper.Write(value, buffer, ref offset, fixedSize, encoding, shouldTerminate);
			offset = 0;
			string result = KnownSizeStringSerializerHelper.Read(buffer, ref offset, fixedSize, encoding, shouldTerminate);

			//assert
			Assert.AreEqual(value, result);
		}
	}
}
