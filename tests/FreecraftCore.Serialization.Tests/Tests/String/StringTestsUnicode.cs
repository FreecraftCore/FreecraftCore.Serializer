using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class StringTestsUnicode
	{
		[Test]
		public static void Test_Fixed_String_Can_Write()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.Unicode), Encoding.Unicode);

			stringSerializer.Write("hello", new DefaultStreamWriterStrategy());
		}

		[Test]
		public static void Test_Fixed_String_Can_Write_Proper_Length()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.Unicode), Encoding.Unicode);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			stringSerializer.Write("hello", writer);

			//assert
			Assert.AreEqual(6 * 2, writer.GetBytes().Length);
		}

		[Test]
		public static void Test_Fixed_String_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(5), new StringSerializerStrategy(Encoding.Unicode), Encoding.Unicode);
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
			ReverseStringSerializerDecorator serializer = new ReverseStringSerializerDecorator(new StringSerializerStrategy(Encoding.Unicode));
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);

			Assert.AreEqual("olleH\0", new string(Encoding.Unicode.GetChars(writer.GetBytes())));
		}

		[Test]
		public static void Test_Send_With_Size_Can_Read()
		{
			//arrange
			SizeStringSerializerDecorator stringSerializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(new GenericTypePrimitiveSharedBufferSerializerStrategy<byte>(), true), new StringSerializerStrategy(Encoding.Unicode), Encoding.Unicode);
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
			DontTerminateStringSerializerDecorator serializer = new DontTerminateStringSerializerDecorator(new StringSerializerStrategy(Encoding.Unicode), Encoding.Unicode);
			DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy();

			//act
			serializer.Write("Hello", writer);
			byte[] bytes = writer.GetBytes();

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);

			Assert.False(bytes[bytes.Length - 1] == 0 && bytes[bytes.Length - 2] == 0);

			Assert.True(bytes.Length == 5 * 2);
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

			Assert.False(bytes[bytes.Length - 1] == 0 && bytes[bytes.Length - 2] == 0);

			Assert.True(bytes.Length == 5 * 2);
		}

		[Test]
		public static void Test_SerializerService_Can_Handle_Type_With_All_string_Types()
		{
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<AllStringTypeStringObject>();
			serializer.Compile();
			AllStringTypeStringObject stringObject = new AllStringTypeStringObject("Hello");

			//act
			byte[] bytes = serializer.Serialize(stringObject);
			AllStringTypeStringObject deserializedObject = serializer.Deserialize<AllStringTypeStringObject>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
			
			//Assert.AreEqual(stringObject.Test, UnicodeEncoding.Unicode.GetString(bytes.Take(10).ToArray()), "Failed to unicode deserialize");
			//Assert.AreEqual(stringObject.Test2, UnicodeEncoding.ASCII.GetString(bytes.Skip(10).Take(2).ToArray()), "Failed to unicode deserialize");

			Assert.AreEqual(stringObject.Test, deserializedObject.Test, $"{nameof(stringObject.Test)}");
			Assert.AreEqual(stringObject.Test2, deserializedObject.Test2, $"{nameof(stringObject.Test2)}");
			Assert.AreEqual(stringObject.Test3, deserializedObject.Test3, $"{nameof(stringObject.Test3)}");
		}

		[Test]
		public static void Test_Serializer_Will_Write_Multibyte_Nullterminator_If_We_Include_It_In_knownSize_String()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<MultiByteNullTerminatorInKnownSizeTest>();
			serializer.Compile();
			MultiByteNullTerminatorInKnownSizeTest t = new MultiByteNullTerminatorInKnownSizeTest("Gello\0hi\0");

			//act
			byte[] bytes = serializer.Serialize(t);
			MultiByteNullTerminatorInKnownSizeTest t2 = serializer.Deserialize<MultiByteNullTerminatorInKnownSizeTest>(bytes);

			//arrange
			Assert.NotNull(t);
			Assert.True(bytes.Length == 10 * 2 + sizeof(int) + sizeof(int), $"Lenght was {bytes.Length}");

			Assert.AreEqual(t.i, t2.i);
			Assert.AreEqual(t.j, t2.j);
			Assert.AreEqual(new string(t.TestString.Reverse().Skip(1).Reverse().ToArray()), t2.TestString);
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("t")]
		[TestCase("derp")]
		public static void Test_Uf16_UsesNullTerminator_When_Not_Using_DontTerminate(string testString)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<Utf16StringTestWithTerminator>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new Utf16StringTestWithTerminator(testString));

			Assert.AreEqual(8 + testString.Length * 2 + 2, bytes.Length);
			Assert.AreEqual(0, bytes.Last());
			Assert.AreEqual(0, bytes.Reverse().Skip(1).First());
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("t")]
		[TestCase("derp")]
		public static void Test_Uf16_UsesNullTerminator_When_BeingReserialized_DontTerminate(string testString)
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<Utf16StringTestWithTerminator>();
			serializer.Compile();

			//act
			byte[] reserialized = serializer.SerializeAsync(serializer.DeserializeAsync<Utf16StringTestWithTerminator>(serializer.SerializeAsync(new Utf16StringTestWithTerminator(testString)).Result).Result).Result;

			Assert.AreEqual(8 + testString.Length * 2 + 2, reserialized.Length);
			Assert.AreEqual(0, reserialized.Last());
			Assert.AreEqual(0, reserialized.Reverse().Skip(1).First());
		}

		[WireDataContract]
		public class Utf16StringTestWithTerminator
		{
			//TODO: What is this?
			[WireMember(1)]
			private long unusued { get; }

			[Encoding(EncodingType.UTF16)]
			[WireMember(2)]
			public string TestString { get; }

			/// <inheritdoc />
			public Utf16StringTestWithTerminator(string testString)
			{
				TestString = testString ?? throw new ArgumentNullException(nameof(testString));
			}

			private Utf16StringTestWithTerminator()
			{
				
			}
		}

		[WireDataContract]
		public class MultiByteNullTerminatorInKnownSizeTest
		{
			[WireMember(1)]
			public int i = 5;

			[Encoding(EncodingType.UTF16)]
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

			[Encoding(EncodingType.UTF16)]
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
			[Encoding(EncodingType.UTF16)]
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

			[Encoding(EncodingType.UTF16)]
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
