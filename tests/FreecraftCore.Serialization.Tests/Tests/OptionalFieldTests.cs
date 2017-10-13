using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests.Tests
{
	[TestFixture]
	public class OptionalFieldTests
	{
		[Test]
		[TestCase(0b0000_0000_0000_0000)]
		[TestCase(0b0000_0000_0000_1000)]
		[TestCase(0b0000_0000_1000_0000)]
		[TestCase(0b0000_1000_0000_0000)]
		[TestCase(0b1000_0000_0000_0000)]
		public void Test_Can_Serialize_Type_With_OptionalField(int i)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSimpleOptionalField>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSimpleOptionalField(i));
			TestSimpleOptionalField deserializerData = serializer.Deserialize<TestSimpleOptionalField>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.NotNull(deserializerData);

			Assert.AreEqual(i, deserializerData.X);
		}

		[Test]
		[TestCase(0b0000_0000_0000_0000)]
		[TestCase(0b0000_0000_0000_1000)]
		[TestCase(0b0000_0000_1000_0000)]
		[TestCase(0b0000_1000_0000_0000)]
		[TestCase(0b1000_0000_0000_0000)]
		public void Test_Can_Serialize_Type_With_OptionalField_But_With_False(int i)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSimpleOptionalFieldWithDisable>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSimpleOptionalFieldWithDisable(i));
			TestSimpleOptionalFieldWithDisable deserializerData = serializer.Deserialize<TestSimpleOptionalFieldWithDisable>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.NotNull(deserializerData);

			Assert.AreEqual(0, deserializerData.X);
		}

		//Test hack from: https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
		[Test]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7}, TestName = "Test_Can_Serialize_Type_With_Complex_Array_Ignored_And_SendSize_Array_Succedding_1")]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 5, 2, 566, 7, 234, 3466}, TestName = "Test_Can_Serialize_Type_With_Complex_Array_Ignored_And_SendSize_Array_Succedding_2")]
		[TestCase(new int[] { 0, 0, 3, 4, 5, 6, Int32.MaxValue }, TestName = "Test_Can_Serialize_Type_With_Complex_Array_Ignored_And_SendSize_Array_Succedding_3")]
		public void Test_Can_Serialize_Type_With_Complex_Array_Ignored_And_SendSize_Array_Succedding(int[] intArray)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestComplexOptionalFieldWithDisable>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestComplexOptionalFieldWithDisable(intArray));
			TestComplexOptionalFieldWithDisable deserializerData = serializer.Deserialize<TestComplexOptionalFieldWithDisable>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length != 0);

			//Check it's sizeof the element * length + the sendsize
			Assert.True(bytes.Length == intArray.Length * sizeof(int) + sizeof(int));

			Assert.NotNull(deserializerData);
			
			for(int i = 0; i < intArray.Length; i++)
				Assert.AreEqual(intArray[i], deserializerData.TestInts[i]);
		}

		//Test hack from: https://stackoverflow.com/questions/17925916/nunit-cannot-recognise-a-testcase-when-it-contains-an-array
		[Test]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7 }, TestName = "Test_Can_Serialize_Type_With_Complex_Array_NotIgnored_And_SendSize_Array_Succedding_1")]
		[TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 5, 2, 566, 7, 234, 3466 }, TestName = "Test_Can_Serialize_Type_With_Complex_Array_NotIgnored_And_SendSize_Array_Succedding_2")]
		[TestCase(new int[] { 0, 0, 3, 4, 5, 6, Int32.MaxValue }, TestName = "Test_Can_Serialize_Type_With_Complex_Array_NotIgnored_And_SendSize_Array_Succedding_3")]
		public void Test_Can_Serialize_Type_With_Complex_Array_NotIgnored_And_SendSize_Array_Succedding(int[] intArray)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestComplexOptionalFieldWithEnable>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestComplexOptionalFieldWithEnable(intArray));
			TestComplexOptionalFieldWithEnable deserializerData = serializer.Deserialize<TestComplexOptionalFieldWithEnable>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.True(bytes.Length != 0);

			//Check it's sizeof the element * length + the sendsize
			Assert.True(bytes.Length == intArray.Length * sizeof(int) + sizeof(int) + 4); //4 bytes from the knownsize array

			Assert.NotNull(deserializerData);

			for(int i = 0; i < intArray.Length; i++)
				Assert.AreEqual(intArray[i], deserializerData.TestInts[i]);
		}
	}

	[WireDataContract]
	public class TestSimpleOptionalField
	{
		public bool isSerialized { get; } = true;

		[Optional(nameof(isSerialized))]
		[WireMember(1)]
		public int X { get; }

		/// <inheritdoc />
		public TestSimpleOptionalField(int x)
		{
			X = x;
		}

		public TestSimpleOptionalField()
		{
			
		}
	}

	[WireDataContract]
	public class TestSimpleOptionalFieldWithDisable
	{
		public bool isSerialized => false;

		[Optional(nameof(isSerialized))]
		[WireMember(1)]
		public int X { get; }

		/// <inheritdoc />
		public TestSimpleOptionalFieldWithDisable(int x)
		{
			X = x;
		}

		public TestSimpleOptionalFieldWithDisable()
		{

		}
	}

	[WireDataContract]
	public class TestComplexOptionalFieldWithDisable
	{
		public bool isSerialized => false; //default to true

		[Optional(nameof(isSerialized))]
		[KnownSize(4)]
		[WireMember(1)]
		public byte[] Bytes { get; } = new byte[4];

		[SendSize(SendSizeAttribute.SizeType.Int32)]
		[WireMember(2)]
		public int[] TestInts { get; }

		/// <inheritdoc />
		public TestComplexOptionalFieldWithDisable(int[] testInts)
		{
			if(testInts == null) throw new ArgumentNullException(nameof(testInts));

			TestInts = testInts;
		}

		public TestComplexOptionalFieldWithDisable()
		{
			
		}
	}

	[WireDataContract]
	public class TestComplexOptionalFieldWithEnable
	{
		public bool isSerialized => true; //default to true

		[Optional(nameof(isSerialized))]
		[KnownSize(4)]
		[WireMember(1)]
		public byte[] Bytes { get; } = new byte[4];

		[SendSize(SendSizeAttribute.SizeType.Int32)]
		[WireMember(2)]
		public int[] TestInts { get; }

		/// <inheritdoc />
		public TestComplexOptionalFieldWithEnable(int[] testInts)
		{
			if(testInts == null) throw new ArgumentNullException(nameof(testInts));

			TestInts = testInts;
		}

		public TestComplexOptionalFieldWithEnable()
		{

		}
	}
}
