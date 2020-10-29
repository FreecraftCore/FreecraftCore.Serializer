using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class UIntSerializerTests
	{
		[Test]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		[TestCase((uint)28532)]
		public void Test_Int_Serializer_Doesnt_Throw_On_Serialize(uint data)
		{
			var strategy = GenericTypePrimitiveSerializerStrategy<uint>.Instance;
			int offset = 0;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[sizeof(uint)]), ref offset));
		}

		[Test]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		[TestCase((uint)27532)]
		public void Test_Int_Serializer_Writes_Ints_Into_WriterStream(uint data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<uint>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(uint)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);

			//assert
			Assert.False(offset == 0);
		}

		[Test]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		[TestCase((uint)253642)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(uint data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<uint>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(uint)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			uint intvalue = (uint)strategy.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(data, intvalue);
		}
	}
}
