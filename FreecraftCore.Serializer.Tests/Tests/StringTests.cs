using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTests
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

			//assert
			Assert.Null(value);
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
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy());

			stringSerializer.Write("hello", new DefaultStreamWriterStrategy());
		}

		[Test]
		public static void Test_Fixed_String_Can_Write_Proper_Length()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy());
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
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy());
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
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new ByteSerializerStrategy(), true), new StringSerializerStrategy());
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
			DontTerminateStringSerializerDecorator serializer = new DontTerminateStringSerializerDecorator(new StringSerializerStrategy());
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
