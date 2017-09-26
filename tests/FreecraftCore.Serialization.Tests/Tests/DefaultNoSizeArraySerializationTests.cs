using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;

namespace FreecraftCore.Serialization
{
	[TestFixture]
	public class DefaultNoSizeArraySerializationTests
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

		[WireDataContract]
		public class TestNoSizeByteArray
		{
			[ReadToEnd]
			[WireMember(1)]
			public byte[] Bytes { get; set; }
		}
	}
}
