using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class SubtypeTests
	{
		[Test]
		public static void Test_Can_Register_NoConsume_Subtype()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<TestBaseType>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize((TestBaseType)new ChildType() { i = 5 });

			//assert
			Assert.AreEqual(1 + sizeof(int), bytes.Length);
		}

		[Test]
		public static void Test_Can_Write_NoConsume_Subtype_Proper_Size()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<TestBaseType>();
			service.Compile();

			//act
			byte[] bytes = service.Serialize((TestBaseType)new ChildType() { i = 5 });

			//assert
			Assert.AreEqual(1 + sizeof(int), bytes.Length);
		}

		[Test]
		public static void Test_Can_Read_NoConsume_Subtype()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<TestBaseType>();
			service.Compile();

			//act
			TestBaseType data = service.Deserialize<TestBaseType>(service.Serialize((TestBaseType)new ChildType() { i = 5, s = 17 }));

			//assert
			Assert.NotNull(data);

			Assert.True(data.GetType() == typeof(ChildType));
			Assert.True(((ChildType)data).i == 5);
			Assert.True(((ChildType)data).s == 17);
		}

		[Test]
		public static void Test_NoCsume_Throws_On_Invalid_Key()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<TestBaseType>();
			service.Compile();

			//act
			Assert.Throws<InvalidOperationException>(() => service.Deserialize<TestBaseType>(service.Serialize((TestBaseType)new ChildType() { i = 2, s = 17 })));
		}

		[Test]
		public static void Can_Deserialize_With_Flags_Type_Information()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<BaseTypeByFlags>();
			service.Compile();

			//act
			BaseTypeByFlags flagsInstance = service.Deserialize<BaseTypeByFlags>(new byte[] { 5 | 6, 0, 0, 0, 7 });

			//assert
			Assert.NotNull(flagsInstance);
			Assert.AreEqual(typeof(ChildTypeByFlags), flagsInstance.GetType());
		}

		[WireDataContractBaseType(5, typeof(ChildType))]
		[WireDataContract(WireDataContractAttribute.KeyType.Byte, false)]
		public class TestBaseType
		{

		}

		public class ChildType : TestBaseType
		{
			[WireMember(1)]
			public byte i;

			[WireMember(2)]
			public int s;
		}

		[WireDataContractBaseTypeByFlags(5, typeof(ChildTypeByFlags))]
		[WireDataContract(WireDataContractAttribute.KeyType.Byte)]
		public class BaseTypeByFlags
		{

		}

		[WireDataContract]
		public class ChildTypeByFlags : BaseTypeByFlags
		{
			[WireMember(1)]
			public int i;
		}
	}
}
