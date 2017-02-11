using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class UInt16SerializerTests
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
			Assert.False(writer.WriterStream.Length == 0);
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

		[Test]
		public static void Test_UShort_Serializer_Produces_Same_Values_As_Other_Methods()
		{
			//arrange
			ushort data = 5625;
			UInt16SerializerStrategy strategy = new UInt16SerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write((ushort)data, writer);
			byte[] stratBytes = writer.GetBytes();
			byte[] bitConverted = BitConverter.GetBytes(data);
			byte[] bitConvertedFromInt = BitConverter.GetBytes((uint) data);
			byte[] typeStratConverter = strategy.GetBytes((ushort)data);

			//assert
			Assert.True(BitConverter.IsLittleEndian);
			for (int i = 0; i < stratBytes.Length; i++)
			{
				Assert.AreEqual(stratBytes[i], bitConverted[i]);
				Assert.AreEqual(stratBytes[i], bitConvertedFromInt[i]);
				Assert.AreEqual(stratBytes[i], typeStratConverter[i]);
			}
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
