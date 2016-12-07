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
	}
}
