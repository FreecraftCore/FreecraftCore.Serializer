using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[WireDataContract(WireDataContractAttribute.KeyType.Byte, InformationHandlingFlags.Default, true)]
	public class TestBaseType
	{
		[WireMember(1)]
		public int i = 5;

		[KnownSize(4)]
		[WireMember(4)]
		public byte[] bytes { get; } = new byte[4];

		public TestBaseType()
		{
			
		}
	}

	[WireDataContract]
	[WireDataContractBaseTypeRuntimeLink(5)]
	public class ChildType : TestBaseType
	{
		[WireMember(1)]
		public int j;

		public ChildType()
		{
			
		}
	}

	[TestFixture]
	public static class PolymorphicRuntimeLinkTests
	{
		[Test]
		public static void Test_Can_Register_Expected_Runtime_BaseType()
		{
			//arrange
			SerializerService serivce = new SerializerService();
			Assert.DoesNotThrow(() => serivce.RegisterType<TestBaseType>());
		}

		[Test]
		public static void Test_Can_Register_Child_of_Expected_Runtime_BaseType()
		{
			//arrange
			SerializerService serivce = new SerializerService();
			Assert.DoesNotThrow(() => serivce.RegisterType<ChildType>());
		}

		[Test]
		public static void Test_Can_Link_Child_Type()
		{
			//arrange
			SerializerService serivce = new SerializerService();
			Assert.DoesNotThrow(() => serivce.RegisterType<ChildType>());

			//act
			bool result = serivce.Link<ChildType, TestBaseType>();

			//assert
			Assert.True(result);
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_With_Linked_Type()
		{
			//arrange
			SerializerService serivce = new SerializerService();
			Assert.DoesNotThrow(() => serivce.RegisterType<ChildType>());
			serivce.Link<ChildType, TestBaseType>();
			serivce.Compile();

			//act
			ChildType child = serivce.Deserialize<TestBaseType>(serivce.Serialize(new ChildType())) as ChildType;

			Console.WriteLine(child.i + 5);

			//assert
			Assert.NotNull(child);
			Assert.True(child.GetType() == typeof(ChildType));
		}
	}
}
