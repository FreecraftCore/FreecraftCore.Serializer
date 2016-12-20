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
		public static void Test_Serializer_Can_Read_Write_Type_With_Enum_Field()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireMessageWithEnum>();
			service.Compile();

			//act
			WireMessageWithEnum testInstance = service.Deserialize<WireMessageWithEnum>((service.Serialize<WireMessageWithEnum>(new WireMessageWithEnum(TestEnum.Ok))));

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Ok);
		}

		[Test]
		public static void Test_Serializer_Can_Register_WireType_With_EnumString_Field()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<WireMessageWithStringEnum>());
			Assert.True(service.isTypeRegistered<WireMessageWithStringEnum>());
		}

		[Test]
		public static void Test_Serializer_Can_Read_Write_Type_With_EnumString_Field()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireMessageWithStringEnum>();
			service.Compile();

			//act
			WireMessageWithStringEnum testInstance = service.Deserialize<WireMessageWithStringEnum>((service.Serialize<WireMessageWithStringEnum>(new WireMessageWithStringEnum(TestEnum.Something))));

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Something);
		}

		[Test]
		public static void Test_Serializer_Can_Read_Enum_As_String()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<WireMessageWithStringEnum>();
			service.RegisterType<TestIsSerializingToStringClass>();
			service.Compile();

			//act
			TestIsSerializingToStringClass testInstance = service.Deserialize<TestIsSerializingToStringClass>((service.Serialize<WireMessageWithStringEnum>(new WireMessageWithStringEnum(TestEnum.Something))));

			//assert
			Assert.AreEqual(testInstance.test, TestEnum.Something.ToString());
		}

		[WireMessage]
		public class WireMessageWithEnum
		{
			[WireMember(1)]
			public TestEnum test;

			public WireMessageWithEnum(TestEnum value)
			{
				test = value;
			}

			public WireMessageWithEnum()
			{

			}
		}

		[WireMessage]
		public class WireMessageWithStringEnum
		{
			[KnownSize(9)]
			[EnumString]
			[WireMember(1)]
			public TestEnum test;

			public WireMessageWithStringEnum(TestEnum value)
			{
				test = value;
			}

			public WireMessageWithStringEnum()
			{

			}
		}

		[WireMessage]
		public class TestIsSerializingToStringClass
		{
			[WireMember(1)]
			public string test;

			public TestIsSerializingToStringClass()
			{

			}
		}

		public enum TestEnum : byte
		{
			Ok,
			No,
			Something
		}
	}
}
