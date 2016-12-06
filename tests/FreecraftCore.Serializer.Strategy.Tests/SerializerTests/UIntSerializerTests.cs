using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			UInt32SerializerStrategy strategy = new UInt32SerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		[TestCase((uint)27532)]
		public void Test_Int_Serializer_Writes_Ints_Into_WriterStream(uint data)
		{
			//arrange
			UInt32SerializerStrategy strategy = new UInt32SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();

			//act
			strategy.Write(data, writer);

			//assert
			Assert.False(writer.WriterStream.Length == 0);
		}

		[Test]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		[TestCase((uint)253642)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(uint data)
		{
			//arrange
			UInt32SerializerStrategy strategy = new UInt32SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			uint intvalue = strategy.Read(reader);

			//assert
			Assert.AreEqual(data, intvalue);
		}

		/*[Test]
		[TestCase(0,1,2,3)]
		[TestCase(255,0,255,0)]
		[TestCase(1,1,1,1)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_ByteArray(params byte[] data)
		{
			//arrange
			ByteSerializerStrategy strategy = new ByteSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			byte b = reader.ReadByte();

			//assert
			Assert.AreEqual(data, b);
		}*/
	}
}
