using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Reinterpret.Net;


namespace FreecraftCore.Serializer.Strategy.Tests
{
	[TestFixture]
	public class DateTimeSerializerTests
	{
		[Test]
		public static void Test_DateTime_Serializes()
		{
			//arrange
			var serializer = PackedDateTimeTypeSerializerStrategy.Instance;
			DateTime testValue = DateTime.Now;
			Span<byte> buffer = new Span<byte>(new byte[100]);
			int offset = 0;

			//act
			serializer.Write(testValue, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray(); //inefficient hehe

			//assert
			Assert.NotNull(bytes);
			Assert.False(bytes.Length == 0);
		}


		[Test]
		public static void Test_DateTime_Deserializes()
		{
			//arrange
			var serializer = PackedDateTimeTypeSerializerStrategy.Instance;
			DateTime testValue = DateTime.Now;
			Span<byte> buffer = new Span<byte>(new byte[100]);
			int offset = 0;

			//act
			serializer.Write(testValue, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray(); //inefficient hehe
			offset = 0;
			DateTime deserializedInstance = serializer.Read(buffer.Slice(0, offset), ref offset);

			//assert
			Assert.NotNull(bytes);
		}

		[Test]
		public static void Test_DateTime_Deserializes_To_Correct_DateTime()
		{
			//arrange
			var serializer = PackedDateTimeTypeSerializerStrategy.Instance;
			DateTime testValue = DateTime.Now;
			Span<byte> buffer = new Span<byte>(new byte[100]);
			int offset = 0;

			//act
			serializer.Write(testValue, buffer, ref offset);
			byte[] bytes = buffer.Slice(0, offset).ToArray(); //inefficient hehe
			offset = 0;
			DateTime deserializedInstance = serializer.Read(buffer.Slice(0, offset), ref offset);

			//assert
			Assert.NotNull(bytes);
			Assert.True(deserializedInstance != DateTime.MinValue);

			//Check values
			Assert.AreEqual(testValue.Minute, deserializedInstance.Minute);
			Assert.AreEqual(testValue.Day, deserializedInstance.Day);
			Assert.AreEqual(testValue.Year, deserializedInstance.Year);
			Assert.AreEqual(testValue.Month, deserializedInstance.Month);
			
		}

		[Test]
		public static void Test_DateTime_Against_WoWPacketParserTest()
		{
			//arrange
			var serializer = PackedDateTimeTypeSerializerStrategy.Instance;
			DateTime testValue = DateTime.Now;
			Span<byte> buffer = new Span<byte>(168938967.Reinterpret());
			int offset = 0; //It's technically sizeof(int);
			
			//Act
			DateTime dateTime = serializer.Read(buffer, ref offset);

			//Assert
			Assert.AreEqual(23, dateTime.Minute);
			Assert.AreEqual(23, dateTime.Hour);
			Assert.AreEqual(8, dateTime.Day);
			Assert.AreEqual(2010, dateTime.Year);
			Assert.AreEqual(2, dateTime.Month);
		}
	}
}
