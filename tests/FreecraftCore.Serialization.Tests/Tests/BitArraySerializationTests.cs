using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class BitArraySerializationTests
	{
		[Test]
		public static void Test_Can_Read_Runescape_Index_Archive()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestBitArrayContainer>();
			serializer.Compile();

			//act
			byte[] bytes = new byte[] { 200, 0, 10, 89 };
			int[] ints = bytes
				.Select((b, index) => new Tuple<int, byte>(index, b))
				.GroupBy(t => t.Item1 / 4)
				.Select(t => t.Select(g => g.Item2).ToArray())
				.Select(bg => BitConverter.ToInt32(bg, 0))
				.ToArray();

			BitArray intBitArray = new BitArray(ints);
			BitArray byteBitArray = new BitArray(bytes);

			//The size must be equal to the length divided by 32 bits (4 byte integer) plus the
			//remainder from a modular division.
			byte[] bitmask = new byte[(intBitArray.Length / 8) + (intBitArray.Length % 8)];

			((ICollection)intBitArray).CopyTo(bitmask, 0);

			BitArray byteBitArray2 = new BitArray(bitmask);
			BitArray intBitArray2 = serializer.Deserialize<BitArray>(serializer.Serialize(intBitArray));

			//assert
			for (int i = 0; i < intBitArray.Length; i++)
				Assert.True(intBitArray[i] == byteBitArray[i]);

			Assert.AreEqual(intBitArray.Length, intBitArray2.Length);
			Assert.AreEqual(intBitArray.Length, byteBitArray.Length);

			for (int i = 0; i < intBitArray.Length; i++)
			{
				Assert.True(intBitArray[i] == byteBitArray2[i]);
				Assert.True(intBitArray[i] == intBitArray2[i]);
			}
		}

		[WireDataContract]
		public class TestBitArrayContainer
		{
			[WireMember(1)]
			public BitArray Test;
		}
	}
}
