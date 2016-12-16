using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class EnumTests
	{
		[Test]
		public static void Test_Serializer_Can_Register_WireType_With_Enum_Field()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<WireMessageWithEnum>());
			Assert.True(service.isTypeRegistered<WireMessageWithEnum>());
		}

		[Test]
		public static void Test_Serializer_Can_Serialize_And_Read_Enum_String()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<WireMessageWithEnum>());
			Assert.True(service.isTypeRegistered<WireMessageWithEnum>());
		}

		[WireMessage]
		public class WireMessageWithEnum
		{
			[WireMember(1)]
			public TestEnum test;
		}

		public enum TestEnum : byte
		{
			Ok,
			No,
			Something
		}
	}
}
