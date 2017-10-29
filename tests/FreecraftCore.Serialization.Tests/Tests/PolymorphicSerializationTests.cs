using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
			serializer.RegisterType<WireDataContractTest>();
			serializer.Compile();

			//assert
			Assert.True(serializer.isTypeRegistered<WireDataContractTest>());
			Assert.True(serializer.isTypeRegistered<ChildTypeOne>());
			Assert.True(serializer.isTypeRegistered<BaseTypeField>());
		}

		[Test]
		public static void Test_Can_Serialize_Child_Then_Deserialize_To_Base()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<WireDataContractTest>();
			serializer.Compile();

			WireDataContractTest message = serializer.Deserialize<WireDataContractTest>(serializer.Serialize(new WireDataContractTest(new ChildTypeThree(), new ChildTypeThree())));

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
			serializer.RegisterType<WireDataContractTest>();
			serializer.Compile();

			WireDataContractTest message = serializer.Deserialize<WireDataContractTest>(serializer.Serialize(new WireDataContractTest(new ChildTypeTwo(20, uint.MaxValue - 1000), new ChildTypeThree())));

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
			serializer.RegisterType<WireDataContractTest>();
			serializer.Compile();

			Assert.Throws<InvalidOperationException>(() => serializer.Deserialize<WireDataContractTest>(serializer.Serialize(new WireDataContractTest(null, null))));
		}

		[Test]
		public static void Test_Can_Serialize_Deserialize_Child_Through_Interface()
		{
			Assert.Warn("We don't support Interface types as the root object. This is not a supported feature at the moment.");
			return;

			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<IBaseInterface>();
			serializer.Compile();

			IBaseInterface instance = serializer.Deserialize<IBaseInterface>(serializer.Serialize<IBaseInterface>(new ChildOfInterfaceTwo(5, 7)));

			//assert
			Assert.NotNull(instance);
			Assert.AreEqual(5, ((ChildOfInterfaceTwo)instance).b);
			Assert.AreEqual(7, ((ChildOfInterfaceTwo)instance).c);
		}

		[Test]
		public static void Test_Can_Serialize_Deserialize_Contains_Interface()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//act
			serializer.RegisterType<TestContainsInterfaceField>();
			serializer.Compile();

			TestContainsInterfaceField instance = serializer.Deserialize<TestContainsInterfaceField>(serializer.Serialize(new TestContainsInterfaceField(new ChildOfInterfaceTwo(5, 7))));

			//assert
			Assert.NotNull(instance);
			Assert.True(instance.Value is ChildOfInterfaceTwo);
			Assert.AreEqual(5, (instance.Value as ChildOfInterfaceTwo).b);
			Assert.AreEqual(7, (instance.Value as ChildOfInterfaceTwo).c);
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_DefaultChild_From_Non_Default_Child()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<WireBaseWithDefault>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new WireChildNotDefault());

			//fiddle with type info
			bytes[0] = 55;

			Assert.False(bytes.Length == 0);

			WireBaseWithDefault result =
				serializer.Deserialize<WireBaseWithDefault>(bytes);

			//assert
			Assert.True(result.GetType() == typeof(WireChildWithDefault));
			Assert.AreEqual(((WireChildWithDefault)result).i, 55);
		}

		[Test]
		public static void Test_Can_Serialize_SystemObject_Typed_DefaultChild_From_Non_Default_Child()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<WireBaseWithDefault>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize((object)new WireChildNotDefault());

			//fiddle with type info
			bytes[0] = 55;

			Assert.False(bytes.Length == 0);

			WireBaseWithDefault result =
				serializer.Deserialize<WireBaseWithDefault>(bytes);

			//assert
			Assert.True(result.GetType() == typeof(WireChildWithDefault));
			Assert.AreEqual(((WireChildWithDefault)result).i, 55);
		}

		[WireDataContract]
		public class TestContainsInterfaceField
		{
			[WireMember(1)]
			public IBaseInterface Value { get; }

			public TestContainsInterfaceField(IBaseInterface value)
			{
				if(value == null) throw new ArgumentNullException(nameof(value));

				Value = value;
			}

			public TestContainsInterfaceField()
			{
				
			}
		}

		[DefaultChild(typeof(WireChildWithDefault))]
		[WireDataContract(WireDataContractAttribute.KeyType.Byte, InformationHandlingFlags.DontConsumeRead)]
		[WireDataContractBaseType(1, typeof(WireChildNotDefault))]
		public class WireBaseWithDefault
		{
			
		}

		public class WireChildNotDefault : WireBaseWithDefault
		{
			
		}

		public class WireChildWithDefault : WireBaseWithDefault
		{
			[WireMember(1)]
			public byte i;
		}

		[WireDataContract]
		public class WireDataContractTest
		{
			[WireMember(1)]
			public BaseTypeField test;

			[WireMember(2)]
			public BaseTypeField anotherTest;

			public WireDataContractTest(BaseTypeField field, BaseTypeField another)
			{
				test = field;
				anotherTest = another;
			}

			public WireDataContractTest()
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

		[WireDataContract(WireDataContractAttribute.KeyType.Byte)]
		[WireDataContractBaseType(1, typeof(ChildTypeOne))]
		[WireDataContractBaseType(2, typeof(ChildTypeTwo))]
		[WireDataContractBaseType(3, typeof(ChildTypeThree))]
		public interface BaseTypeField
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

		[WireDataContract(WireDataContractAttribute.KeyType.Byte)]
		[WireDataContractBaseType(1, typeof(ChildOfInterfaceOne))]
		[WireDataContractBaseType(2, typeof(ChildOfInterfaceTwo))]
		public interface IBaseInterface
		{
			int c { get; }
		}

		[WireDataContract]
		public class ChildOfInterfaceOne : IBaseInterface
		{
			[WireMember(1)]
			public int a;

			[WireMember(2)]
			public int c { get; private set; }

			public ChildOfInterfaceOne()
			{

			}
		}

		[WireDataContract]
		public class ChildOfInterfaceTwo : IBaseInterface
		{
			[WireMember(1)]
			public int b;

			[WireMember(2)]
			public int c { get; private set; }

			public ChildOfInterfaceTwo(int bValue, int cValue)
			{
				b = bValue;
				c = cValue;
			}

			public ChildOfInterfaceTwo()
			{

			}
		}
	}
}
