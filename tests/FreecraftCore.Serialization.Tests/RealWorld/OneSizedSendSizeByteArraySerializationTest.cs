using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore
{
	/// <summary>
	/// These tests exist because a fault was found when trying to serializer a byte array with SendSize with a byte key.
	/// Size 1 caused issued when used with the async serializer. The non-async serializer did fine. So this test helps to
	/// cover that case.
	/// </summary>
	[TestFixture]
	public class OneSizedSendSizeByteArraySerializationTest
	{
		[Test]
		public void Test_Can_Serializer_OneByteSized_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestOneSizedByteArray>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.SerializeAsync(new TestOneSizedByteArray(new byte[1] {7})).Result;

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
			Assert.AreEqual(bytes.Length, 3);
		}

		[Test]
		public void Test_Can_Deserialize_OneByteSized_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestOneSizedByteArray>();
			serializer.Compile();

			//act
			TestOneSizedByteArray result = serializer.DeserializeAsync<TestOneSizedByteArray>(new byte[3]{ 0, 1, 7}).Result;

			//assert
			Assert.NotNull(result);
			Assert.IsNotEmpty(result.Bytes);
			Assert.AreEqual(1, result.Bytes.Length);
			Assert.AreEqual(7, result.Bytes[0]);
		}

		[WireDataContract]
		public class TestOneSizedByteArray
		{
			public enum TestEnum : byte
			{
				Zero,
				One,
				Two
			}

			[WireMember(1)]
			public TestEnum EnumVal { get; }

			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[WireMember(2)]
			public byte[] Bytes { get; }

			/// <inheritdoc />
			public TestOneSizedByteArray(byte[] bytes)
			{
				Bytes = bytes;
			}

			public TestOneSizedByteArray()
			{
				
			}
		}
	}
}
