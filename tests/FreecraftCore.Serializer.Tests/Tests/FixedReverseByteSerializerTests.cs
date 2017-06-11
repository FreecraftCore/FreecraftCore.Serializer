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
		public static void Test_Can_Register_ReverseFixedByteArray_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//assert
			Assert.DoesNotThrow(() => serializer.RegisterType<ReverseArrayByteTest>());
		}

		[Test]
		public static void Test_Can_Serialize_ReverseFixedByteArray_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReverseArrayByteTest>();
			serializer.Compile();
			ReverseArrayByteTest test = new ReverseArrayByteTest(new byte[] { 1, 2, 3});


			//arrange
			byte[] bytes = serializer.Serialize(test);

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length == 3);
		}

		[Test]
		public static void Test_Serialized_ReverseArrayBytes_Are_Reversed()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReverseArrayByteTest>();
			serializer.Compile();
			ReverseArrayByteTest test = new ReverseArrayByteTest(new byte[] { 1, 2, 3 });


			//arrange
			byte[] bytes = serializer.Serialize(test);

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length == 3);

			for(int i = 0; i < 3; i++)
				Assert.AreEqual(test.Bytes[3 - i - 1], bytes[i], $"The {i}th index was incorrect.");
		}

		[WireDataContract]
		public class ReverseArrayByteTest
		{
			[ReverseData]
			[KnownSize(3)]
			[WireMember(1)]
			public byte[] Bytes { get; }

			public ReverseArrayByteTest(byte[] bytes)
			{
				Bytes = bytes;
			}

			public ReverseArrayByteTest()
			{
				
			}
		}
	}
}
