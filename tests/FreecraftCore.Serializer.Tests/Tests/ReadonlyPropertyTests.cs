using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests.Tests
{
	[TestFixture]
	public static class ReadonlyPropertyTests
	{
		[Test]
		public static void Test_Can_Register_Readonly_Property()
		{
			SerializerService serializer = new SerializerService();

			serializer.RegisterType<TestWithReadonlyProperty>();
		}

		[Test]
		public static void Test_Can_Serialize_Readonly_Property()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestWithReadonlyProperty>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestWithReadonlyProperty(5));
		}

		[Test]
		public static void Test_Can_Deserialize_Readonly_Property()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestWithReadonlyProperty>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestWithReadonlyProperty(5));
			TestWithReadonlyProperty obj = serializer.Deserialize<TestWithReadonlyProperty>(bytes);

			//assert
			Assert.NotNull(obj);
			Assert.AreEqual(5, obj.i);
		}

		[WireDataContract]
		public class TestWithReadonlyProperty
		{
			[WireMember(1)]
			public int i { get; }

			public TestWithReadonlyProperty(int iValue)
			{
				i = iValue;
			}

			public TestWithReadonlyProperty()
			{
				
			}
		}
	}
}
