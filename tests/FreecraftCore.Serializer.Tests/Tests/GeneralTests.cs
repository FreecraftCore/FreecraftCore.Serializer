using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class GeneralTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new SerializerService());
		}

		[Test]
		public static void Test_Doesnt_Throw_On_Empty_Compile()
		{
			//arrange
			SerializerService service = new SerializerService();

			//assert
			Assert.DoesNotThrow(() => service.Compile());
		}

		[Test]
		[TestCase(typeof(int))]
		[TestCase(typeof(uint))]
		[TestCase(typeof(byte))]
		[TestCase(typeof(sbyte))]
		[TestCase(typeof(UInt64))]
		[TestCase(typeof(Int64))]
		[TestCase(typeof(short))]
		[TestCase(typeof(ushort))]
		public static void Test_Known_Serializer_Stragies_Are_Registered(Type type)
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			bool result = service.isTypeRegistered(type);

			//assert
			Assert.True(result);
		}

		[Test]
		[TestCase(typeof(Tuple))]
		[TestCase(typeof(MemberAccessException))]
		[TestCase(typeof(MissingMemberException))]
		[TestCase(typeof(Version))]
		public static void Test_Serializer_Doesnt_Know_About_Random_Types(Type type)
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			bool result = service.isTypeRegistered(type);

			//assert
			Assert.False(result);
		}
	}
}
