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
			int[] values = new int[] { 0,0,0,0,0,0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 0, 0, 0 };
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			WoWZLibCompressionTypeSerializerDecorator<PrimitiveArrayTypeSerializerStrategy<int>, int[]>.Instance.Write(values, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.IsTrue(bytes.Length < (values.Length - 1) * sizeof(int), $"ValuesSize: {(values.Length - 1) * sizeof(int)} Bytes: {bytes.Length}");
		}

		[Test]
		public static void Test_Can_Deserialize_Compression_Marked_Class()
		{
			//arrange
			int[] values = new int[] { 1, 5, 7, 1, 1, 1, 1, 1, 1, 5, 1, 5, 6 };
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//act
			WoWZLibCompressionTypeSerializerDecorator<PrimitiveArrayTypeSerializerStrategy<int>, int[]>.Instance.Write(values, buffer, ref offset);

			//Reverse it!
			int[] values2 = WoWZLibCompressionTypeSerializerDecorator<PrimitiveArrayTypeSerializerStrategy<int>, int[]>.Instance.Read(buffer, 0);

			//assert
			Assert.NotNull(values2);
			Assert.AreEqual(values.Length, values2.Length);
			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], values2[i]);
		}
	}
}
