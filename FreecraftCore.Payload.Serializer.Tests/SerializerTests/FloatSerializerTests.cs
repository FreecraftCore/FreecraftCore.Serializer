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
	public class FloatSerializerTests
	{
		[Test]
		[TestCase(10.425f)]
		[TestCase(255.421f)]
		[TestCase(-50212.0f)]
		public void Test_float_Serializer_Doesnt_Throw_On_Serialize(float data)
		{
			FloatSerializerStrategy strategy = new FloatSerializerStrategy();

			Assert.DoesNotThrow(() => strategy.Write(data, new TestStorageWriterMock()));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_float_Serializer_Writes_floats_Into_WriterStream(float data)
		{
			//arrange
			FloatSerializerStrategy strategy = new FloatSerializerStrategy();
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
		public void Test_float_Serializer_Writes_And_Reads_Same_float(float data)
		{
			//arrange
			FloatSerializerStrategy strategy = new FloatSerializerStrategy();
			TestStorageWriterMock writer = new TestStorageWriterMock();
			TestStorageReaderMock reader = new TestStorageReaderMock(writer.WriterStream);

			//act
			strategy.Write(data, writer);
			writer.WriterStream.Position = 0;
			float b = BitConverter.ToSingle(reader.ReadBytes(4), 0);

			//assert
			Assert.AreEqual(data, b);
		}
	}
}
