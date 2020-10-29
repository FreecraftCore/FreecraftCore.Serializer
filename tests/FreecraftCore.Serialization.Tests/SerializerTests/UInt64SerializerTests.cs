using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class UInt6464SerializerTests
	{
		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)28532674754578)]
		public void Test_Int_Serializer_Doesnt_Throw_On_Serialize(UInt64 data)
		{
			var strategy = GenericTypePrimitiveSerializerStrategy<UInt64>.Instance;
			int offset = 0;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[sizeof(UInt64)]), ref offset));
		}

		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)2753257547346)]
		public void Test_Int_Serializer_Writes_Ints_Into_WriterStream(UInt64 data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<UInt64>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(UInt64)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);

			//assert
			Assert.False(offset == 0);
		}

		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)253642)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(UInt64 data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<UInt64>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(UInt64)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			UInt64 intvalue = (UInt64)strategy.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(data, intvalue);
		}
	}
}
