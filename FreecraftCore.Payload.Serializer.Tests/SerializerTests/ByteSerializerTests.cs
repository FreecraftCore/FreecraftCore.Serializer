using NUnit.Framework;
using System;
using System.Collections.Generic;
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
	}
}
