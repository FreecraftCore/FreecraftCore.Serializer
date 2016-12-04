using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	[TestFixture]
	public class ByteSerializerTests
	{
		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Doesnt_Throw_On_Serialize(byte data)
		{
			ByteSerializerStrategy strategy = new ByteSerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_Bytes_Into_WriterStream(byte data)
		{
			//arrange
			ByteSerializerStrategy strategy = new ByteSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();

			//act
			strategy.Write(data, writer);

			//assert
			Assert.NotZero(writer.WriterStream.Length);
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(byte data)
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
		}

		[Test]
		[TestCase((sbyte)0)]
		[TestCase(SByte.MaxValue)]
		[TestCase((sbyte)-50)]
		[TestCase((sbyte)50)]
		public void Test_SByte_Serializer_Writes_And_Reads_Same_Byte(sbyte data)
		{
			//arrange
			SByteSerializerStrategy strategy = new SByteSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;

			//assert
			Assert.AreEqual(data, strategy.Read(reader));
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
