using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests.Tests
{
	public static class PolymorphicMultilevelSerializationTests
	{
		[Test]
		public static void Test_Can_Regiser_Multilevel_Polymorphic_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<TestBaseClass>());
		}

		[Test]
		public static void Test_Can_Serialize_Child()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<TestBaseClass>());
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ChildClass1() {i = 1, b = 2, c = 3});

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_With_Correct_Values()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<TestBaseClass>());
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ChildClass1() { i = 1, b = 2, c = 3 });
			ChildClass1 child = serializer.Deserialize<TestBaseClass>(bytes) as ChildClass1;

			//assert
			Assert.NotNull(child);

			Assert.AreEqual(3, child.c);
			Assert.AreEqual(2, child.b);
			Assert.AreEqual(1, child.i);
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_With_Correct_Values_Multi_Level()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<TestBaseClass>());
			Assert.DoesNotThrow(() => serializer.RegisterType<SecondLevelClass>());
			Assert.DoesNotThrow(() => serializer.RegisterType<ChildClass1>());
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new SecondLevelClass() { i = 4356, b = 75432, c = 88585, TestString = "This is a test string.!"});
			SecondLevelClass child = serializer.Deserialize<TestBaseClass>(bytes) as SecondLevelClass;

			//assert
			Assert.NotNull(child);

			Assert.AreEqual("This is a test string.!", child.TestString);
			Assert.AreEqual(88585, child.c);
			Assert.AreEqual(75432, child.b);
			Assert.AreEqual(4356, child.i);
		}

		[WireDataContract(WireDataContractAttribute.KeyType.Byte)]
		[WireDataContractBaseType(1, typeof(ChildClass1))]
		[WireDataContractBaseType(2, typeof(ChildClass2))]
		public abstract class TestBaseClass
		{
			[WireMember(1)]
			public int i;

			public TestBaseClass()
			{
					
			}
		}

		[WireDataContract]
		public class SecondLevelClass : ChildClass1
		{
			[Encoding(EncodingType.UTF32)]
			[WireMember(1)]
			public string TestString;

			public SecondLevelClass()
			{
				
			}
		}

		[WireDataContract(WireDataContractAttribute.KeyType.Int32)]
		[WireDataContractBaseType(1, typeof(SecondLevelClass))]
		public class ChildClass1 : TestBaseClass
		{
			[WireMember(1)]
			public int b;

			[WireMember(2)]
			public int c;

			public ChildClass1()
			{
					
			}
		}

		public class ChildClass2 : TestBaseClass
		{
			[WireMember(1)]
			public float d;

			public ChildClass2()
			{
					
			}
		}
	}
}
