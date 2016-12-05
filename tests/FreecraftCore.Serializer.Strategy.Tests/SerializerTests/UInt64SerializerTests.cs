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
	public class UInt6464SerializerTests
	{
		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)28532674754578)]
		public void Test_Int_Serializer_Doesnt_Throw_On_Serialize(UInt64 data)
		{
			UInt64SerializerStrategy strategy = new UInt64SerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)2753257547346)]
		public void Test_Int_Serializer_Writes_Ints_Into_WriterStream(UInt64 data)
		{
			//arrange
			UInt64SerializerStrategy strategy = new UInt64SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();

			//act
			strategy.Write(data, writer);

			//assert
			Assert.NotZero(writer.WriterStream.Length);
		}

		[Test]
		[TestCase(UInt64.MaxValue)]
		[TestCase(UInt64.MinValue)]
		[TestCase((UInt64)253642)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(UInt64 data)
		{
			//arrange
			UInt64SerializerStrategy strategy = new UInt64SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			UInt64 intvalue = strategy.Read(reader);

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
