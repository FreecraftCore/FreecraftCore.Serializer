using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FreecraftCore.Serialization
{
	[TestFixture]
	public class AddedSizeSendSizeTests
	{
		[Test]
		public void Test_Can_Register_AddedSize_Array_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestAddedSizeArrayType>();
			serializer.Compile();
		}

		[Test]
		public void Test_Can_Serialize_AddedSize_Array_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestAddedSizeArrayType>();
			serializer.Compile();
			int[] values = new int[] {5, 5, 5, 6, 7, 8};

			//act
			byte[] bytes = serializer.Serialize(new TestAddedSizeArrayType(values));
			TestAddedSizeArrayType deserialized = serializer.Deserialize<TestAddedSizeArrayType>(bytes);

			//assert
			Assert.AreEqual(values.Length * sizeof(int) + sizeof(ushort), bytes.Length);
			Assert.AreEqual(bytes[0], values.Length - 2);

			for(int i = 0; i < values.Length; i++)
				Assert.AreEqual(values[i], deserialized.Values[i]);
		}

		[Test]
		public void Test_Can_Register_AddedSize_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestAddedSizeStringType>();
			serializer.Compile();
		}

		[Test]
		public void Test_Can_Serialize_AddedSize_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestAddedSizeStringType>();
			serializer.Compile();
			string value = "sega made me have to do this";

			//act
			byte[] bytes = serializer.Serialize(new TestAddedSizeStringType(value));
			TestAddedSizeStringType deserialized = serializer.Deserialize<TestAddedSizeStringType>(bytes);

			//assert
			Assert.AreEqual(value.Length * 1 + sizeof(ushort) + 1 /*for null*/, bytes.Length); //use 1 instead of sizeof(char) due to ASCII
			Assert.AreEqual(bytes[0], value.Length - 2 + 1 /*for null*/);

			Assert.AreEqual(value, deserialized.Value);
		}
	}

	[WireDataContract]
	public class TestAddedSizeArrayType
	{
		[SendSize(SendSizeAttribute.SizeType.UShort, 2)]
		[WireMember(1)]
		public int[] Values { get; }

		public TestAddedSizeArrayType(int[] values)
		{
			if(values == null) throw new ArgumentNullException(nameof(values));

			Values = values;
		}

		public TestAddedSizeArrayType()
		{
			
		}
	}

	[WireDataContract]
	public class TestAddedSizeStringType
	{
		[SendSize(SendSizeAttribute.SizeType.UShort, 2)]
		[WireMember(1)]
		public string Value { get; }

		public TestAddedSizeStringType(string value)
		{
			if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

			Value = value;
		}

		public TestAddedSizeStringType()
		{

		}
	}
}
