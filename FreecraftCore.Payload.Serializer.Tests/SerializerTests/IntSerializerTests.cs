using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	[TestFixture]
	public class IntSerializerTests
	{
		[Test]
		[TestCase(int.MaxValue)]
		[TestCase(int.MinValue)]
		[TestCase(28532)]
		public void Test_Int_Serializer_Doesnt_Throw_On_Serialize(int data)
		{
			IntSerializerStrategy strategy = new IntSerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(int.MaxValue)]
		[TestCase(int.MinValue)]
		[TestCase(27532)]
		public void Test_Int_Serializer_Writes_Ints_Into_WriterStream(int data)
		{
			//arrange
			IntSerializerStrategy strategy = new IntSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();

			//act
			strategy.Write(data, writer);

			//assert
			Assert.NotZero(writer.WriterStream.Length);
		}

		[Test]
		[TestCase(int.MaxValue)]
		[TestCase(int.MinValue)]
		[TestCase(253642)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(int data)
		{
			//arrange
			IntSerializerStrategy strategy = new IntSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			int intvalue = strategy.Read(reader);

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
