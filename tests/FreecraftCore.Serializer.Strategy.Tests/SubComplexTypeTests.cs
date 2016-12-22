using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Payload.Serializer
{
	/*[TestFixture]
	public class SubComplexTypeTests 
	{
		[Test]
		public static void Test_Can_Serialize_Complex_Type_With_Subtype()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<TestWithSubType>();
			service.Compile();

			//act
			TestWithSubType instance = service.Deserialize<TestWithSubType>(service.Serialize(new TestWithSubType()));
		}

		[WireDataContract]
		public class TestWithSubType
		{
			[WireMember(1)]
			public BaseType test;
		}

		[WireDataContract]
		[WireDataContractBaseType(1, typeof(ChildType))]
		public class BaseType
		{
			public BaseType()
			{

			}
		}

		[WireDataContract]
		public class ChildType
		{
			[WireMember(1)]
			public int b;

			[WireMember(2)]
			public int c;

			public ChildType()
			{

			}
		}
	}*/
}
