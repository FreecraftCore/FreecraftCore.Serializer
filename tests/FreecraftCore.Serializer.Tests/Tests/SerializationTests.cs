using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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

		[Test]
		public static void Test_Can_Serializer_Then_Deserialize_Most_Complex_Test_Possible()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			TestEnum[] arrayOne = new TestEnum[] { TestEnum.Zero, TestEnum.One };
			TestEnum[] arrayTwo = new TestEnum[3] { TestEnum.Two, TestEnum.One, TestEnum.Zero };
			BasicWireMessage wireMessageNested = new BasicWireMessage(8);

			//act
			serializer.RegisterType<VeryComplexType>();
			serializer.Compile();

			byte[] bytes = serializer.Serialize(new VeryComplexType(6, wireMessageNested, arrayOne, arrayTwo));
			Assert.NotNull(bytes);
			Assert.False(bytes.Length == 0 );
			VeryComplexType message = serializer.Deserialize<VeryComplexType>(bytes);

			//assert
			Assert.NotNull(message);

			//check fields
			for (int i = 0; i < arrayOne.Length; i++)
				Assert.AreEqual(arrayOne[i], message.testEnums[i], $"Failed for index {i}.");

			for (int i = 0; i < arrayTwo.Length; i++)
				Assert.AreEqual(arrayTwo[i], message.testEnumsAnother[i], $"Failed for index {i}.");
		}

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
