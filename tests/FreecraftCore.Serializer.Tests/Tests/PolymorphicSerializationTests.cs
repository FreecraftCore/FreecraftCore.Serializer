using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class PolymorphicSerializationTests
	{
		[Test]
		public static void Test_Can_Register_Type_With_Child_Types()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<WireMessageTest>();
			serializer.Compile();

			//assert
			Assert.True(serializer.isTypeRegistered<WireMessageTest>());
			Assert.True(serializer.isTypeRegistered<ChildTypeOne>());
			Assert.True(serializer.isTypeRegistered<BaseTypeField>());
		}

		[Test]
		public static void Test_Can_Serialize_Child_Then_Deserialize_To_Base()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<WireMessageTest>();
			serializer.Compile();

			WireMessageTest message = serializer.Deserialize<WireMessageTest>(serializer.Serialize(new WireMessageTest(new ChildTypeThree(), new ChildTypeThree())));

			//assert
			Assert.NotNull(message);
			Assert.True(message.test.GetType() == typeof(ChildTypeThree));
		}

		[Test]
		public static void Test_Can_Serialize_Child_Then_Deserialize_To_Base_With_Correct_Data_Intact()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<WireMessageTest>();
			serializer.Compile();

			WireMessageTest message = serializer.Deserialize<WireMessageTest>(serializer.Serialize(new WireMessageTest(new ChildTypeTwo(20, uint.MaxValue - 1000), new ChildTypeThree())));

			//assert
			Assert.NotNull(message);
			Assert.True(message.test.GetType() == typeof(ChildTypeTwo));

			Assert.AreEqual((message.test as ChildTypeTwo).b, 20);
		}

		//Right now we dont support serializing null
		[Test]
		public static void Test_Cant_Polymorphic_Serialize_Null()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<WireMessageTest>();
			serializer.Compile();

			Assert.Throws<InvalidOperationException>(() => serializer.Deserialize<WireMessageTest>(serializer.Serialize(new WireMessageTest(null, null))));
		}

		[WireMessage]
		public class WireMessageTest
		{
			[WireMember(1)]
			public BaseTypeField test;

			[WireMember(2)]
			public BaseTypeField anotherTest;

			public WireMessageTest(BaseTypeField field, BaseTypeField another)
			{
				test = field;
				anotherTest = another;
			}

			public WireMessageTest()
			{

			}
		}

		public class AnotherTest
		{
			[WireMember(1)]
			public int a;

			public AnotherTest()
			{

			}
		}

		[WireMessageBaseType(1, typeof(ChildTypeOne))]
		[WireMessageBaseType(2, typeof(ChildTypeTwo))]
		[WireMessageBaseType(3, typeof(ChildTypeThree))]
		public class BaseTypeField
		{

		}

		public class ChildTypeOne : BaseTypeField
		{
			[WireMember(1)]
			public int a;

			[WireMember(2)]
			public int f;

			public ChildTypeOne()
			{

			}
		}

		public class ChildTypeTwo : BaseTypeField
		{
			[WireMember(1)]
			public sbyte b;

			[WireMember(3)]
			public uint g;

			public ChildTypeTwo(sbyte bVal, uint gVal)
			{
				b = bVal;
				g = gVal;
			}

			public ChildTypeTwo()
			{

			}
		}

		public class ChildTypeThree : BaseTypeField
		{
			[WireMember(1)]
			public int c;

			public ChildTypeThree()
			{

			}
		}
	}
}
