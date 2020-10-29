using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
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
			var strategy = GenericTypePrimitiveSerializerStrategy<float>.Instance;
			int offset = 0;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[sizeof(float)]), ref offset));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_float_Serializer_Writes_floats_Into_WriterStream(float data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<float>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(float)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);

			//assert
			Assert.False(offset == 0);
			Assert.True(offset == sizeof(float));
		}

		[Test]
		[TestCase(0)]
		[TestCase(255)]
		[TestCase(1)]
		public void Test_float_Serializer_Writes_And_Reads_Same_float(float data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<float>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(float)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			float b = BitConverter.ToSingle(buffer.Slice(0, sizeof(float)).ToArray(), 0);
			float b2 = strategy.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(data, b);
			Assert.AreEqual(data, b2);
		}
	}
}
