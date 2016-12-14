using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			stringSerializer.Write("hello", new DefaultWireMemberWriterStrategy());
		}

		[Test]
		public static void Test_Fixed_String_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);
			string value = stringSerializer.Read(new DefaultWireMemberReaderStrategy(writer.GetBytes()));

			//assert
			Assert.IsNotNullOrEmpty(value);
			Assert.AreEqual("hello", value);
		}

		[Test]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings()
		{
			//arrange
			ReverseStringSerializerDecorator serializer = new ReverseStringSerializerDecorator(new StringSerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();

			//act
			serializer.Write("Hello", writer);

			Assert.AreEqual("olleH\0", new string(Encoding.ASCII.GetChars(writer.GetBytes())));
		}

		[Test]
		public static void Test_Send_With_Size_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new ByteSerializerStrategy()), new StringSerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);
			string value = stringSerializer.Read(new DefaultWireMemberReaderStrategy(writer.GetBytes()));

			//assert
			Assert.IsNotNullOrEmpty(value);
			Assert.AreEqual("hello", value);
		}
	}
}
