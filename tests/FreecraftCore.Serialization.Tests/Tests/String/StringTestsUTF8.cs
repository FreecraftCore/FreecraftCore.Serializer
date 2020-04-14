using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsUTF8
	{
		public const int NULL_TERMINATOR_SIZE = 1;

		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.UTF8), Encoding.UTF8);

			stringSerializer.Write("hello", new DefaultStreamWriterStrategy());
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Fixed_String_Can_Write_Proper_Length(string input)
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(input.Length), new StringSerializerStrategy(Encoding.UTF8), Encoding.UTF8);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write(input, writer);

			//assert
			Assert.AreEqual(Encoding.UTF8.GetByteCount(input) + NULL_TERMINATOR_SIZE, writer.GetBytes().Length);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Fixed_String_Can_Read(string input)
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(Encoding.UTF8.GetByteCount(input)), new StringSerializerStrategy(Encoding.UTF8), Encoding.UTF8);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write(input, writer);
			string value = stringSerializer.Read(new DefaultStreamReaderStrategy(writer.GetBytes()));

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual(input, value);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Reverse_Decorator_Can_Reverse_Strings(string input)
		{
			//arrange
			ReverseStringSerializerDecorator serializer = new ReverseStringSerializerDecorator(new StringSerializerStrategy(Encoding.UTF8));
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write(input, writer);

			Assert.AreEqual(new string(input.Reverse().Concat(new char[1] { '\0' }).ToArray()), new string(Encoding.UTF8.GetChars(writer.GetBytes())));
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Send_With_Size_Can_Read(string input)
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new GenericTypePrimitiveSharedBufferSerializerStrategy<byte>(), true), new StringSerializerStrategy(Encoding.UTF8), Encoding.UTF8);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write(input, writer);
			string value = stringSerializer.Read(new DefaultStreamReaderStrategy(writer.GetBytes()));

			//assert
			Assert.NotNull(value);
			Assert.IsNotEmpty(value);
			Assert.AreEqual(input, value);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_DontTerminate_Serializer_Doesnt_Add_Terminator(string input)
		{
			//arrange
			DontTerminateStringSerializerDecorator serializer = new DontTerminateStringSerializerDecorator(new StringSerializerStrategy(Encoding.UTF8), Encoding.UTF8);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write(input, writer);
			byte[] bytes = writer.GetBytes();

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0);

			Assert.True(bytes.Length == Encoding.UTF8.GetBytes(input).Length);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_SerializerService_Can_Handle_DontTerminate_Marked_Data(string input)
		{
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestDontTerminateString>();

			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TestDontTerminateString(input));

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0);

			Assert.True(bytes.Length == Encoding.UTF8.GetBytes(input).Length);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_SerializerService_Can_Handle_Type_With_All_string_Types(string input)
		{
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<AllStringTypeStringObject>();
			serializer.Compile();
			AllStringTypeStringObject stringObject = new AllStringTypeStringObject(input);

			//act
			byte[] bytes = serializer.Serialize(stringObject);
			AllStringTypeStringObject deserializedObject = serializer.Deserialize<AllStringTypeStringObject>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
			
			//Assert.AreEqual(stringObject.Test, UnicodeEncoding.UTF8.GetString(bytes.Take(10).ToArray()), "Failed to unicode deserialize");
			//Assert.AreEqual(stringObject.Test2, UnicodeEncoding.ASCII.GetString(bytes.Skip(10).Take(2).ToArray()), "Failed to unicode deserialize");

			Assert.AreEqual(stringObject.Test, deserializedObject.Test, $"{nameof(stringObject.Test)}");
			Assert.AreEqual(stringObject.Test2, deserializedObject.Test2, $"{nameof(stringObject.Test2)}");
			Assert.AreEqual(stringObject.Test3, deserializedObject.Test3, $"{nameof(stringObject.Test3)}");
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Uf16_UsesNullTerminator_When_Not_Using_DontTerminate(string testString)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<UTF8StringTestWithTerminator>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new UTF8StringTestWithTerminator(testString));

			Assert.AreEqual(8 + Encoding.UTF8.GetBytes(testString).Length + NULL_TERMINATOR_SIZE, bytes.Length);
			Assert.AreEqual(0, bytes.Last(), "Last byte was not null terminator");
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("ҬЭSҐЇЍG ҐӉЇЙG")]
		[TestCase("‎先秦兩漢")]
		[TestCase("Östliche Königreiche")]
		public static void Test_Uf16_UsesNullTerminator_When_BeingReserialized_DontTerminate(string testString)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<UTF8StringTestWithTerminator>();
			serializer.Compile();

			//act
			byte[] reserialized = serializer.SerializeAsync(serializer.DeserializeAsync<UTF8StringTestWithTerminator>(serializer.SerializeAsync(new UTF8StringTestWithTerminator(testString)).Result).Result).Result;

			Assert.AreEqual(8 + Encoding.UTF8.GetBytes(testString).Length + NULL_TERMINATOR_SIZE, reserialized.Length);
			Assert.AreEqual(0, reserialized.Last());
		}

		[WireDataContract]
		public class UTF8StringTestWithTerminator
		{
			//TODO: What is this?
			[WireMember(1)]
			private long unusued { get; }

			[Encoding(EncodingType.UTF8)]
			[WireMember(2)]
			public string TestString { get; }

			/// <inheritdoc />
			public UTF8StringTestWithTerminator(string testString)
			{
				TestString = testString ?? throw new ArgumentNullException(nameof(testString));
			}

			private UTF8StringTestWithTerminator()
			{
				
			}
		}

		[WireDataContract]
		public class MultiByteNullTerminatorInKnownSizeTest
		{
			[WireMember(1)]
			public int i = 5;

			[Encoding(EncodingType.UTF8)]
			[KnownSize(10)]
			[WireMember(2)]
			public string TestString { get; }

			[WireMember(3)]
			public int j = 6;

			public MultiByteNullTerminatorInKnownSizeTest(string testString)
			{
				TestString = testString;
			}

			public MultiByteNullTerminatorInKnownSizeTest()
			{
				
			}
		}

		[WireDataContract]
		public class TestDontTerminateString
		{

			[Encoding(EncodingType.UTF8)]
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

		[WireDataContract]
		public class AllStringTypeStringObject
		{
			[Encoding(EncodingType.UTF8)]
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[DontTerminate]
			[WireMember(1)]
			public string Test;

			[KnownSize(8)]
			[Encoding(EncodingType.ASCII)]
			[WireMember(2)]
			public string Test2 { get; set; } = "Hi";

			[KnownSize(8)]
			[Encoding(EncodingType.ASCII)]
			[WireMember(3)]
			public string Test69 { get; set; } = "Hi56";

			[Encoding(EncodingType.UTF8)]
			[WireMember(4)]
			public string Test78 { get; set; } = "Hi";

			[Encoding(EncodingType.UTF32)]
			[WireMember(5)]
			public string Test3 { get; set; } = "Hi";

			public AllStringTypeStringObject(string test)
			{
				Test = test;
			}

			public AllStringTypeStringObject()
			{

			}
		}
	}
}
