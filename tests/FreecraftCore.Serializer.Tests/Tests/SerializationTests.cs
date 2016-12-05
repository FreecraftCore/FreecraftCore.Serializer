using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class SerializationTests
	{
		[Test]
		public static void Test_Can_Serializer_Then_Deserialize_Empty_WireMessage()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<EmptyWireMessage>();
			serializer.Compile();

			EmptyWireMessage message = serializer.Deserialize<EmptyWireMessage>(serializer.Serialize(new EmptyWireMessage()));

			//assert
			Assert.NotNull(message);
		}

		[Test]
		public static void Test_Can_Serializer_Then_Deserialize_Basic_Wire_Message()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<BasicWireMessage>();
			serializer.Compile();

			BasicWireMessage message = serializer.Deserialize<BasicWireMessage>(serializer.Serialize(new BasicWireMessage(5)));

			//assert
			Assert.NotNull(message);
		}

		[Test]
		public static void Test_Serializer_Can_Register_Most_Complex_Type_Possible()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<VeryComplexType>();
			serializer.Compile();

			//assert
			Assert.True(serializer.isTypeRegistered<VeryComplexType>());
		}

		/*[Test]
		public static void Test_Can_Serializer_Then_Deserialize_Most_Complex_Test_Possible()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<VeryComplexType>();
			serializer.Compile();

			VeryComplexType message = serializer.Deserialize<VeryComplexType>(serializer.Serialize(new VeryComplexType(5)));

			//assert
			Assert.NotNull(message);
		}*/

		[WireMessage]
		public class EmptyWireMessage
		{
			public EmptyWireMessage()
			{

			}
		}

		[WireMessage]
		public class BasicWireMessage
		{
			[WireMember(1)]
			public int a;

			public BasicWireMessage(int aVal)
			{
				a = aVal;
			}

			public BasicWireMessage()
			{

			}
		}

		[WireMessage]
		public class VeryComplexType
		{
			[WireMember(1)]
			public int a;

			[WireMember(5)]
			public BasicWireMessage b;

			[WireMember(7)]
			public TestEnum[] testEnums { get; private set; }

			[KnownSize(3)]
			[WireMember(8)]
			public TestEnum[] testEnumsAnother { get; private set; }

			public VeryComplexType(int aVal, BasicWireMessage nestedMessage, TestEnum[] enumsOne, TestEnum[] enumsTwo)
			{
				a = aVal;
				b = nestedMessage;

				testEnumsAnother = enumsTwo;
				testEnums = enumsOne;
			}

			public VeryComplexType()
			{

			}
		}

		public enum TestEnum
		{
			Zero,
			One,
			Two
		}
	}
}
