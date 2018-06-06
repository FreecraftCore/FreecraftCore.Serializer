using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsASCII
	{
		[Test]
		public static void Test_String_Serializer_Serializes()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.Compile();

			//act
			string value = serializer.Deserialize<string>(serializer.Serialize("Hello!"));

			//assert
			Assert.AreEqual(value, "Hello!");
		}

		[Test]
		public static void Test_String_Serializer_Can_Serialize_Empty_String()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.Compile();

			//act
			string value = serializer.Deserialize<string>(serializer.Serialize(""));

			//Change was made here that makes null strings empty strings
			//This seems preferable and easier to deal with. Nullrefs are bad
			//and also serializing null is harder than serializing empty
			//this is overall less error prone.
			//assert
			Assert.NotNull(value);
		}

		[Test]
		public static void Test_String_With_Null_Terminator_Is_Same()
		{
			//arrange
			Assert.AreEqual("Hello", "Hello".PadRight(5, '\0'));

			Assert.AreEqual("Hello".Length, "Hello".PadRight(5, '\0').Length);
		}

		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(), Encoding.ASCII);

			stringSerializer.Write("hello", new DefaultStreamWriterStrategy());
		}

		[Test]
		public static void Test_Fixed_String_Can_Write_Proper_Length()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(), Encoding.ASCII);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);

			//assert
			Assert.AreEqual(6, writer.GetBytes().Length);
		}

		[Test]
		public static void Test_Fixed_String_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(), Encoding.ASCII);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);
			string value = stringSerializer.Read(new DefaultStreamReaderStrategy(writer.GetBytes()));

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual("hello", value);
		}

		[Test]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings()
		{
			//arrange
			ReverseStringSerializerDecorator serializer = new ReverseStringSerializerDecorator(new StringSerializerStrategy());
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);

			Assert.AreEqual("olleH\0", new string(Encoding.ASCII.GetChars(writer.GetBytes())));
		}

		[Test]
		public static void Test_Send_With_Size_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new GenericTypePrimitiveSharedBufferSerializerStrategy<byte>(), true), new StringSerializerStrategy(), Encoding.ASCII);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write("Hello", writer);
			string value = stringSerializer.Read(new DefaultStreamReaderStrategy(writer.GetBytes()));

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual("Hello", value);
		}

		[Test]
		public static void Test_DontTerminate_Serializer_Doesnt_Add_Terminator()
		{
			//arrange
			DontTerminateStringSerializerDecorator serializer = new DontTerminateStringSerializerDecorator(new StringSerializerStrategy(), Encoding.ASCII);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);
			byte[] bytes = writer.GetBytes();

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0);
			Assert.True(bytes.Length == 5);
		}

		[Test]
		public static void Test_SerializerService_Can_Handle_DontTerminate_Marked_Data()
		{
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestDontTerminateString>();

			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestDontTerminateString("Hello"));

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0);
			Assert.True(bytes.Length == 5);
		}

		[Test]
		public static void Test_Can_Deserialize_To_String_With_Only_Null_Terminator()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestOnlyNullTerminatorString>();
			serializer.Compile();

			//act
			TestOnlyNullTerminatorString obj = serializer.Deserialize<TestOnlyNullTerminatorString>(new byte[1] {0});

			//assert
			Assert.NotNull(obj, "Object was null.");
			Assert.NotNull(obj.TestString, "String was null.");
			Assert.True(obj.TestString.Length == 0);
		}

		[Test]
		public static void Test_Can_Serialize_To_String_With_Only_Null_Terminator()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestOnlyNullTerminatorString>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestOnlyNullTerminatorString(""));

			//assert
			Assert.NotNull(bytes, "bytes array was null.");
			Assert.True(bytes.Length == 1);
			Assert.True(bytes[0] == 0);
		}

		[WireDataContract]
		public class TestOnlyNullTerminatorString
		{
			[Encoding(EncodingType.ASCII)]
			[WireMember(1)]
			public string TestString { get; }

			/// <inheritdoc />
			public TestOnlyNullTerminatorString(string testString)
			{
				TestString = testString;
			}

			public TestOnlyNullTerminatorString()
			{
				
			}
		}

		[WireDataContract]
		public class TestDontTerminateString
		{
			[DontTerminate]
			[WireMember(1)]
			public string Test;

			public TestDontTerminateString(string test)
			{
				Test = test;
			}

			public TestDontTerminateString()
			{
				
			}
		}
	}
}
