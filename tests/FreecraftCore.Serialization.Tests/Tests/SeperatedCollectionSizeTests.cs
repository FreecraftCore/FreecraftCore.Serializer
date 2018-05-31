using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests.Tests
{
	[TestFixture]
	public class SeperatedCollectionSizeTests
	{
		[Test]
		public static void Test_Can_Register_SeperatedCollection_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<TestSeperatedCollection>());
		}

		[Test]
		public static void Test_Can_Serializer_SeperatedCollection_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSeperatedCollection>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSeperatedCollection("Hello meep!", 55, new[] {55523, 90, 2445, 63432}));

			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
		}

		//TODO: Fix feature and test
		[Test]
		public static void Test_Can_Deserialize_SeperatedCollection_Type()
		{
			Assert.Warn("TODO You must fix and reimplement seperated collection. It's not working fully as intended.");
			return;
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSeperatedCollection>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSeperatedCollection("Hello meep56!", 123456, new[] { 55523, 90, 2445, 63432, 6969 }));
			TestSeperatedCollection deserialized = serializer.Deserialize<TestSeperatedCollection>(bytes);

			//assert
			Assert.NotNull(deserialized);
			Assert.NotNull(deserialized.IntsChars);

			Assert.AreEqual(5, deserialized.Size, "Expected the size to be the original collection size");
			Assert.AreEqual(5, deserialized.IntsChars.Length, $"Expected the length of the collection to be the original length");

			Assert.AreEqual("Hello meep56!", deserialized.TestString);
			Assert.AreEqual(123456, deserialized.AnotherValue);
		}

		[Test]
		public static void Test_Can_Serializer_SeperatedCollectionThroughInternal_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSeperatedCollectionThroughInternal>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSeperatedCollectionThroughInternal("Hello meep!", 55, new[] { 55523, 90, 2445, 63432 }, 55));

			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
		}

		[Test]
		public static void Test_Can_Deserialize_SeperatedCollectionThroughInternal_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSeperatedCollectionThroughInternal>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestSeperatedCollectionThroughInternal("Hello meep56!", 123456, new[] { 55523, 90, 2445, 63432, 6969 }, 55));
			TestSeperatedCollectionThroughInternal deserialized = serializer.Deserialize<TestSeperatedCollectionThroughInternal>(bytes);

			//assert
			Assert.NotNull(deserialized);
			Assert.NotNull(deserialized.IntsChars);

			Assert.AreEqual(5, deserialized.Size, "Expected the size to be the original collection size");
			Assert.AreEqual(5, deserialized.IntsChars.Length, $"Expected the length of the collection to be the original length");

			Assert.AreEqual("Hello meep56!", deserialized.TestString);
			Assert.AreEqual(123456, deserialized.AnotherValue);
			Assert.AreEqual(55, deserialized.ValueAfterArray);
		}

		[Test]
		public static void Test_Seperated_Collection_Size_Does_Not_Extend_TypeSize_For_Unwritten_Members()
		{
			Assert.Warn("TODO You must fix and reimplement seperated collection. It's not working fully as intended.");
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSeperatedCollectionSerializedSizeShouldBeZero>();
			serializer.Compile();
			TestSeperatedCollectionSerializedSizeShouldBeZero testValue = new TestSeperatedCollectionSerializedSizeShouldBeZero();

			//act
			byte[] bytes = serializer.Serialize(testValue);

			//assert
			Assert.AreEqual(0, bytes.Length, "Should not serializer any data.");
		}
	}

	[SeperatedCollectionSize(nameof(TestSeperatedCollection.IntsChars), nameof(TestSeperatedCollection.InternalSize))]
	[WireDataContract]
	public class TestSeperatedCollection
	{
		[WireMember(1)]
		public ushort Size { get; private set; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(2)]
		public string TestString { get; }

		[WireMember(3)]
		public int AnotherValue { get; }

		public int InternalSize
		{
			get { return Size; }
			set { Size = (ushort)value; }
		}

		//[SendSize(SendSizeAttribute.SizeType.Int32)]
		[WireMember(4)]
		public int[] IntsChars { get; }

		/// <inheritdoc />
		public TestSeperatedCollection(string testString, int anotherValue, int[] intsChars)
		{
			TestString = testString;
			AnotherValue = anotherValue;
			IntsChars = intsChars;
		}

		public TestSeperatedCollection()
		{
			
		}
	}

	[SeperatedCollectionSize(nameof(TestSeperatedCollectionThroughInternal.Size), nameof(TestSeperatedCollectionSerializedSizeShouldBeZero.CollectionSize))]
	[WireDataContract]
	public class TestSeperatedCollectionSerializedSizeShouldBeZero
	{
		public int CollectionSize { get; } = 0;

		[WireMember(1)]
		public int[] Collection { get; } = Array.Empty<int>();

		public TestSeperatedCollectionSerializedSizeShouldBeZero()
		{
			
		}
	}

	[SeperatedCollectionSize(nameof(TestSeperatedCollectionThroughInternal.IntsChars), nameof(TestSeperatedCollectionThroughInternal.Size))]
	[WireDataContract]
	public class TestSeperatedCollectionThroughInternal
	{
		[WireMember(1)]
		public ushort Size { get; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(2)]
		public string TestString { get; }

		[WireMember(3)]
		public int AnotherValue { get; }

		//[SendSize(SendSizeAttribute.SizeType.Int32)]
		[WireMember(4)]
		public int[] IntsChars { get; }

		[WireMember(5)]
		public short ValueAfterArray { get; }

		/// <inheritdoc />
		public TestSeperatedCollectionThroughInternal(string testString, int anotherValue, int[] intsChars, short valueAfterArray)
		{
			TestString = testString;
			AnotherValue = anotherValue;
			IntsChars = intsChars;
			ValueAfterArray = valueAfterArray;
		}

		public TestSeperatedCollectionThroughInternal()
		{
		}
	}
}
