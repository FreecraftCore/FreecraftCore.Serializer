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

		[Test]
		public static void Test_Can_Deserialize_SeperatedCollection_Type()
		{
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
	}

	[SeperatedCollectionSize(nameof(TestSeperatedCollection.IntsChars), nameof(TestSeperatedCollection.Size))]
	[WireDataContract]
	public class TestSeperatedCollection
	{
		[WireMember(1)]
		public int Size { get; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(2)]
		public string TestString { get; }

		[WireMember(3)]
		public int AnotherValue { get; }

		[SendSize(SendSizeAttribute.SizeType.Int32)]
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
}
