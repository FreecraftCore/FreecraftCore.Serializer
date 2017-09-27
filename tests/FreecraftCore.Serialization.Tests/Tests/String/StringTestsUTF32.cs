using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsUTF32
	{
		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.UTF32), Encoding.UTF32);

			stringSerializer.Write("hello", new DefaultStreamWriterStrategy());
		}

		[Test]
		public static void Test_Fixed_String_Can_Write_Proper_Length()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.UTF32), Encoding.UTF32);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);

			//assert
			Assert.AreEqual(6 * 4, writer.GetBytes().Length);
		}

		[Test]
		public static void Test_Fixed_String_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.UTF32), Encoding.UTF32);
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
			ReverseStringSerializerDecorator serializer = new ReverseStringSerializerDecorator(new StringSerializerStrategy(Encoding.UTF32));
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);

			Assert.AreEqual("olleH\0", new string(Encoding.UTF32.GetChars(writer.GetBytes())));
		}

		[Test]
		public static void Test_Send_With_Size_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new GenericTypePrimitiveSharedBufferSerializerStrategy<byte>(), true), new StringSerializerStrategy(Encoding.UTF32), Encoding.UTF32);
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
			DontTerminateStringSerializerDecorator serializer = new DontTerminateStringSerializerDecorator(new StringSerializerStrategy(Encoding.UTF32), Encoding.UTF32);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);
			byte[] bytes = writer.GetBytes();

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0 && bytes[bytes.Length - 2] == 0 && bytes[bytes.Length - 3] == 0 && bytes[bytes.Length - 4] == 0);

			Assert.True(bytes.Length == 5 * 4);
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

			Assert.False(bytes[bytes.Length - 1] == 0 && bytes[bytes.Length - 2] == 0 && bytes[bytes.Length - 3] == 0 && bytes[bytes.Length - 4] == 0);

			Assert.True(bytes.Length == 5 * 4);
		}

		[WireDataContract]
		public class TestDontTerminateString
		{
			[Encoding(EncodingType.UTF32)]
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
