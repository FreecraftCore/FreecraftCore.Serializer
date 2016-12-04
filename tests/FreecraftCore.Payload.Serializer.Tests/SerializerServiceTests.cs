using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	[TestFixture]
	public static class SerializerServiceTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new SerializerService());
		}

		[Test]
		public static void Test_Can_Register_Complex_Type()
		{
			//arrange
			ISerializerService service = new SerializerService();

			//act
			bool result = service.RegisterType<TestTypeClass>();

			//assert
			Assert.True(result);

			//compile before last check
			service.Compile();

			Assert.True(service.isTypeRegistered(typeof(TestTypeClass)));
		}

		[Test]
		public static void Test_Registered_Serializer_Can_Serialize()
		{
			//arrange
			ISerializerService service = new SerializerService();

			//act
			service.RegisterType<TestTypeClass>();
			service.Compile();
			TestTypeClass deserializeInstance = service.Deserialize<TestTypeClass>(service.Serialize(new TestTypeClass(1000, 200)));

			//assert
			Assert.NotNull(deserializeInstance);
			Assert.AreEqual(deserializeInstance.a, 1000);
			Assert.AreEqual(deserializeInstance.b, 200);
		}

		[Test]
		public static void Test_Can_Register_Complex_Nested_Type()
		{
			//arrange
			ISerializerService service = new SerializerService();

			//act
			bool result = service.RegisterType<NestedComplexClass>();

			//assert
			Assert.True(result);

			//compile before last check
			service.Compile();

			Assert.True(service.isTypeRegistered(typeof(NestedComplexClass)));
		}

		[Test]
		public static void Test_Registered_Serializer_Can_Serialize_Complex_Nested_Type()
		{
			//arrange
			ISerializerService service = new SerializerService();

			//act
			service.RegisterType<NestedComplexClass>();
			service.Compile();
			NestedComplexClass deserializeInstance = service.Deserialize<NestedComplexClass>(service.Serialize(new NestedComplexClass(new TestTypeClass(1000, 200), 50632)));

			//assert
			Assert.NotNull(deserializeInstance);
			Assert.AreEqual(deserializeInstance.a.a, 1000);
			Assert.AreEqual(deserializeInstance.a.b, 200);
			Assert.AreEqual(deserializeInstance.b, 50632);
		}

		[WireMessage]
		public class NestedComplexClass
		{
			[WireMember(1)]
			public TestTypeClass a;

			[WireMember(2)]
			public uint b;

			public NestedComplexClass(TestTypeClass aValue, uint bValue)
			{
				a = aValue;
				b = bValue;
			}

			public NestedComplexClass()
			{

			}
		}

		[WireMessage]
		public class TestTypeClass
		{
			[WireMember(1)]
			public uint a;

			[WireMember(2)]
			public byte b;

			[WireMember(3)]
			public UInt16 c { get; private set; }

			public TestTypeClass(uint aVal, byte bVal)
			{
				a = aVal;
				b = bVal;
				c = (ushort)((uint)a + (uint)b);
			}

			public TestTypeClass()
			{
			}
		}
	}
}
