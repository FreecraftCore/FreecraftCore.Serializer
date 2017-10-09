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
			SerializerService service = new SerializerService();
			service.RegisterType<TestNoSizeByteArray>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize(new TestNoSizeByteArray(){ Bytes = new byte[size] });

			//assert
			Assert.NotNull(bytes);
			Assert.AreEqual(size, bytes.Length);
		}

		[Test]
		public void Test_Can_Handle_Large_KnownSize_Array_Objects()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestLargeKnownSizeArray>();
			serializer.Compile();
			byte[] values = Enumerable.Repeat((byte)153, 500).ToArray();

			//act
			byte[] bytes = serializer.Serialize(new TestLargeKnownSizeArray(values));
			TestLargeKnownSizeArray deserialized = serializer.Deserialize<TestLargeKnownSizeArray>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.NotNull(deserialized);
			Assert.True(bytes.Length == 500);

			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], deserialized.Bytes[i]);
		}


		[WireDataContract]
		public class TestNoSizeByteArray
		{
			[ReadToEnd]
			[WireMember(1)]
			public byte[] Bytes { get; set; }
		}

		[WireDataContract]
		public class TestLargeKnownSizeArray
		{
			[KnownSize(500)]
			[WireMember(1)]
			public byte[] Bytes { get; set; }

			public TestLargeKnownSizeArray(byte[] bytes)
			{
				if(bytes == null) throw new ArgumentNullException(nameof(bytes));

				Bytes = bytes;
			}

			public TestLargeKnownSizeArray()
			{
				
			}
		}
	}
}
