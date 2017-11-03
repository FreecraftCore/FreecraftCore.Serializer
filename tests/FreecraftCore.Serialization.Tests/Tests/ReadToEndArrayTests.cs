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
	public class ReadToEndArrayTests
	{
		public static string[] TestStrings = new[]
		{
			"Hello", "Goodbye", "LOLOLlodsfsdfdsfOLOLOLdfgdfgdfg", "Testiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiing"
		};

		[Test]
		public void Test_Can_Register_CustomClass_ReadToEnd()
		{
			//arrange
			SerializerService serializer = new SerializerService();

			//assert
			Assert.DoesNotThrow(() =>
			{
				serializer.RegisterType<ReadToEndCustomClassType>();
				serializer.Compile();
			});
		}

		[Test]
		public void Test_Can_Serializer_ReadToEnd_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReadToEndCustomClassType>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ReadToEndCustomClassType(5, TestStrings.Select(t => new TestReadToEndCustomClass(t, 5)).ToArray()));

			Assert.NotNull(bytes);
			Assert.IsNotEmpty(bytes);
			Assert.True(bytes.Length > 4);
		}

		[Test]
		public void Test_Can_Deserialize_ReadToEnd_String_Type()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<ReadToEndCustomClassType>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new ReadToEndCustomClassType(5, TestStrings.Select(t => new TestReadToEndCustomClass(t, 5)).ToArray()));
			ReadToEndCustomClassType deserializer = serializer.Deserialize<ReadToEndCustomClassType>(bytes);

			Assert.NotNull(deserializer);
			Assert.IsNotEmpty(deserializer.Strings);
			Assert.AreEqual(5, deserializer.I, "Int value wasn't the samne.");
			Assert.AreEqual(TestStrings.Length, deserializer.Strings.Count(), "Had more custom class instances than strings");

			//Check that there are null terminators
			Assert.AreEqual(TestStrings.Length * TestStrings.Length, bytes.Skip(4).Count(b => b == 0));
		}
	}

	[WireDataContract]
	public class TestReadToEndCustomClass
	{
		[WireMember(1)]
		public string T { get; }

		[WireMember(2)]
		public int I { get; }

		/// <inheritdoc />
		public TestReadToEndCustomClass(string t, int x)
		{
			T = t;
			I = x;
		}

		public TestReadToEndCustomClass()
		{
			
		}
	}

	[WireDataContract]
	public class ReadToEndCustomClassType
	{
		[WireMember(1)]
		public int I { get; }

		[ReadToEnd]
		[WireMember(2)]
		private TestReadToEndCustomClass[] _Strings { get; }

		public IEnumerable<TestReadToEndCustomClass> Strings => _Strings;

		/// <inheritdoc />
		public ReadToEndCustomClassType(int x, TestReadToEndCustomClass[] strings)
		{
			I = x;
			_Strings = strings;
		}

		//Serializer ctor
		public ReadToEndCustomClassType()
		{

		}
	}
}
