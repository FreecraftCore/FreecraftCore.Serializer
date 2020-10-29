using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class FixedReverseByteSerializerTests
	{
		[Test]
		public static void Test_Can_Serialize_ReverseFixedByteArray_Type()
		{
			//arrange
			var mutator = ReverseBinaryMutatorStrategy.Instance;
			byte[] values = new byte[] {1, 2, 3};
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//arrange
			mutator.Mutate(buffer, ref offset, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length == 3);
		}

		[Test]
		public static void Test_Serialized_ReverseArrayBytes_Are_Reversed()
		{
			var mutator = ReverseBinaryMutatorStrategy.Instance;
			byte[] values = new byte[] {1, 2, 3};
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			int offset = 0;

			//arrange
			mutator.Mutate(buffer, ref offset, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray();

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length == 3);

			for(int i = 0; i < 3; i++)
				Assert.AreEqual(values[3 - i - 1], bytes[i], $"The {i}th index was incorrect.");
		}
	}
}
