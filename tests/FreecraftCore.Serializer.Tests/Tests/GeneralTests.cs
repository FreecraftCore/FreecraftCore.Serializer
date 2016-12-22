using FreecraftCore.Serializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class GeneralTests
	{
		public static void Test_Serialization_Hack_Works()
		{
			int[] intArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 567, 234, 6225, 732164, 50, 245662, 36542, };

			
		}

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

		[Test]
		public static void Test_Serializer_Throws_On_Non_Wiretype_Registerations()
		{
			//arrange
			SerializerService service = new SerializerService();

			//assert
			Assert.Throws<InvalidOperationException>(() => service.RegisterType<TestClassUnmarked>());
		}

		[Test]
		public static void Test_Serializer_Can_Register_WireType_Empty()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<TestClassMarked>());
			Assert.True(service.isTypeRegistered(typeof(TestClassMarked)));
		}

		[Test]
		public static void Test_Serializer_Can_Register_WireType_With_Nested_Complex_Type_Empty()
		{
			//arrange
			SerializerService service = new SerializerService();

			//act
			Assert.DoesNotThrow(() => service.RegisterType<TestClassMarkedWithComplexMember>());
			Assert.True(service.isTypeRegistered<TestClassMarkedWithComplexMember>());
		}

		public class TestClassUnmarked
		{
			public TestClassUnmarked()
			{

			}
		}

		
		[WireDataContract]
		public class TestClassMarked
		{
			public TestClassMarked()
			{

			}
		}

		[WireDataContract]
		public class TestClassMarkedWithComplexMember
		{
			[WireMember(1)]
			public TestClassUnmarked unmarkedClass;

			public TestClassMarkedWithComplexMember()
			{

			}
		}
	}
}
