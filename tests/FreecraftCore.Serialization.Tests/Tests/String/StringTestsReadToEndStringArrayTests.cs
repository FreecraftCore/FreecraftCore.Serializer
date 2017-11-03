using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests.Tests.String
{
	[TestFixture]
	public class StringTestsReadToEndStringArrayTests
	{
		public static string[] TestStrings = new[]
		{
			"Hello", "Goodbye", "LOLOLlodsfsdfdsfOLOLOLdfgdfgdfg", "Testiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiing"
		};

		[Test]
		public void Test_Can_Register_ReadToEnd_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//assert
			Assert.DoesNotThrow(() =>
			{
				serializer.RegisterType<ReadToEndStringType>();
				serializer.Compile();
			});
		}

		[Test]
		public void Test_Can_Serializer_ReadToEnd_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReadToEndStringType>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ReadToEndStringType(5, TestStrings));

			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
			Assert.True(bytes.Length > 4);
		}

		[Test]
		public void Test_Can_Deserialize_ReadToEnd_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReadToEndStringType>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ReadToEndStringType(5, TestStrings));
			ReadToEndStringType deserializer = serializer.Deserialize<ReadToEndStringType>(bytes);

			Assert.NotNull(deserializer);
			Assert.IsNotEmpty(deserializer.Strings);
			Assert.AreEqual(5, deserializer.I);
			Assert.AreEqual(TestStrings.Length, deserializer.Strings.Length);

			//Check that there are null terminators
			Assert.AreEqual(TestStrings.Length, bytes.Skip(4).Count(b => b == 0));
			
			for(int i = 0; i < TestStrings.Length; i++)
				Assert.AreEqual(TestStrings[i], deserializer.Strings[i]);
		}
	}

	[WireDataContract]
	public class ReadToEndStringType
	{
		[WireMember(1)]
		public int I { get; }

		[ReadToEnd]
		[WireMember(2)]
		public string[] Strings { get; }

		/// <inheritdoc />
		public ReadToEndStringType(int x, string[] strings)
		{
			I = x;
			Strings = strings;
		}

		//Serializer ctor
		public ReadToEndStringType()
		{
			
		}
	}
}
