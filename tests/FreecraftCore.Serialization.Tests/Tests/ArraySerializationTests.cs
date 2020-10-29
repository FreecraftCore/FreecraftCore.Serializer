using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;

namespace FreecraftCore.Serialization
{
	[TestFixture]
	public class ArraySerializationTests
	{
		[Test]
		[TestCase(5)]
		public void Test_Size_Is_Same_As_Byte_Count(int size)
		{
			//arrange
			var serializer = PrimitiveArrayTypeSerializerStrategy<byte>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[size * 2]);
			int offset = 0;

			//act
			serializer.Write(new byte[size], buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.AreEqual(size, bytes.Length);
		}

		[Test]
		public void Test_Can_Handle_Large_KnownSize_Array_Objects()
		{
			//arrange
			byte[] values = Enumerable.Repeat((byte)153, 500).ToArray();
			var serializer = PrimitiveArrayTypeSerializerStrategy<byte>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[500 * 2]);
			int offset = 0;

			//act
			serializer.Write(values, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();
			offset = 0;
			byte[] deserialized = serializer.Read(buffer, ref offset);

			//assert
			Assert.NotNull(bytes);
			Assert.NotNull(deserialized);
			Assert.True(bytes.Length == 500);

			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], deserialized[i]);
		}
	}
}
