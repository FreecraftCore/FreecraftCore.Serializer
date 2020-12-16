using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
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
			int offset = 0;
			var strategy = GenericTypePrimitiveSerializerStrategy<byte>.Instance;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[1]), ref offset));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Doesnt_Throw_On_Serialize_Specialized(byte data)
		{
			int offset = 0;
			var strategy = BytePrimitiveSerializerStrategy.Instance;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[1]), ref offset));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_Bytes_Into_WriterStream(byte data)
		{
			//arrange
			int offset = 0;
			var strategy = GenericTypePrimitiveSerializerStrategy<byte>.Instance;

			//act
			strategy.Write(data, new Span<byte>(new byte[1]), ref offset);

			//assert
			Assert.False(offset == 0);
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_Bytes_Into_WriterStream_Specialized(byte data)
		{
			//arrange
			int offset = 0;
			var strategy = BytePrimitiveSerializerStrategy.Instance;

			//act
			strategy.Write(data, new Span<byte>(new byte[1]), ref offset);

			//assert
			Assert.False(offset == 0);
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(byte data)
		{
			//arrange
			int offset = 0;
			var strategy = GenericTypePrimitiveSerializerStrategy<byte>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[1]);

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			byte b = strategy.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(data, b);
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte_Specialized(byte data)
		{
			//arrange
			int offset = 0;
			var strategy = BytePrimitiveSerializerStrategy.Instance;
			Span<byte> buffer = new Span<byte>(new byte[1]);

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			byte b = strategy.Read(buffer, ref offset);

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
			int offset = 0;
			var strategy = GenericTypePrimitiveSerializerStrategy<sbyte>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[1]);

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;

			//assert
			Assert.AreEqual(data, strategy.Read(buffer, ref offset));
		}

		[Test]
		[TestCase(0)]
		[TestCase(Byte.MaxValue)]
		[TestCase(50)]
		public void Test_SByte_Serializer_Writes_And_Reads_Same_Byte_Specialized(byte data)
		{
			//arrange
			int offset = 0;
			var strategy = BytePrimitiveSerializerStrategy.Instance;
			Span<byte> buffer = new Span<byte>(new byte[1]);

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;

			//assert
			Assert.AreEqual(data, strategy.Read(buffer, ref offset));
		}
	}
}
