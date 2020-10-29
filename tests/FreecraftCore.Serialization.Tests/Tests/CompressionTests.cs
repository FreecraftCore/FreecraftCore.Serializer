using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class CompressionTests
	{
		[Test]
		public static void Test_Can_Serialize_Compression_Marked_Class()
		{
			//arrange
			int[] values = new int[] { 0,0,0,0,0,0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			Span<byte> bufferOutput = new Span<byte>(new byte[1024]);
			int offset = 0;
			int offsetOutput = 0;

			//act
			PrimitiveArrayTypeSerializerStrategy<int>.Instance.Write(values, buffer, ref offset);

			buffer = buffer.Slice(0, offset);
			offset = 0;

			ZLibCompressionBinaryMutatorStrategy.Instance.Mutate(buffer, ref offset, bufferOutput, ref offsetOutput);
			byte[] bytes = bufferOutput.Slice(0, offsetOutput).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.IsTrue(bytes.Length < ((values.Length - 1) * sizeof(int)));
		}

		[Test]
		public static void Test_Can_Deserialize_Compression_Marked_Class()
		{
			//arrange
			int[] values = new int[] { 1, 5, 7, 1, 1, 1, 1, 1, 1, 5, 1, 5, 6 };
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			Span<byte> bufferOutput = new Span<byte>(new byte[1024]);
			int offset = 0;
			int offsetOutput = 0;

			//act
			PrimitiveArrayTypeSerializerStrategy<int>.Instance.Write(values, buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			ZLibCompressionBinaryMutatorStrategy.Instance.Mutate(buffer, ref offset, bufferOutput, ref offsetOutput);

			//Reverse it!
			offset = 0;
			offsetOutput = 0;
			ZLibCompressionBinaryMutatorStrategy.Instance.Mutate(bufferOutput, ref offsetOutput, buffer, ref offset);
			buffer = buffer.Slice(0, offset);
			offset = 0;
			int[] values2 = PrimitiveArrayTypeSerializerStrategy<int>.Instance.Read(buffer, ref offset);

			//assert
			Assert.NotNull(values2);
			Assert.AreEqual(values.Length, values2.Length);
			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], values2[i]);
		}
	}
}
