using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	[TestFixture]
	public class Int16SerializerTests
	{
		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)28532)]
		public void Test_Int16_Serializer_Doesnt_Throw_On_Serialize(UInt16 data)
		{
			UInt16SerializerStrategy strategy = new UInt16SerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)27532)]
		public void Test_Int16_Serializer_Writes_Int16s_Int16o_WriterStream(UInt16 data)
		{
			//arrange
			UInt16SerializerStrategy strategy = new UInt16SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();

			//act
			strategy.Write(data, writer);

			//assert
			Assert.NotZero(writer.WriterStream.Length);
		}

		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)500)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(UInt16 data)
		{
			//arrange
			UInt16SerializerStrategy strategy = new UInt16SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			UInt16 Int16value = strategy.Read(reader);

			//assert
			Assert.AreEqual(data, Int16value);
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
