using FreecraftCore.Serializer.KnownTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Strategy.Tests
{
	[TestFixture]
	public class DateTimeSerializerTests
	{
		[Test]
		public static void Test_DateTime_Serializes()
		{
			//arrange
			PackedDateTimeSerializerStrategyDecorator serializer = new PackedDateTimeSerializerStrategyDecorator(new Int32SerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();
			DateTime testValue = DateTime.Now;

			//act
			serializer.Write(testValue, writer);
			byte[] bytes = writer.GetBytes();

			//assert
			Assert.NotNull(bytes);
			Assert.False(bytes.Length == 0);
		}


		[Test]
		public static void Test_DateTime_Deserializes()
		{
			//arrange
			PackedDateTimeSerializerStrategyDecorator serializer = new PackedDateTimeSerializerStrategyDecorator(new Int32SerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();
			DateTime testValue = DateTime.Now;

			//act
			serializer.Write(testValue, writer);
			byte[] bytes = writer.GetBytes();
			DateTime deserializedInstance = serializer.Read(new DefaultWireMemberReaderStrategy(bytes));

			//assert
			Assert.NotNull(bytes);
		}

		[Test]
		public static void Test_DateTime_Deserializes_To_Correct_DateTime()
		{
			//arrange
			PackedDateTimeSerializerStrategyDecorator serializer = new PackedDateTimeSerializerStrategyDecorator(new Int32SerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();
			DateTime testValue = DateTime.Now;


			//act
			serializer.Write(testValue, writer);
			byte[] bytes = writer.GetBytes();
			DateTime deserializedInstance = serializer.Read(new DefaultWireMemberReaderStrategy(bytes));

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
			PackedDateTimeSerializerStrategyDecorator serializer = new PackedDateTimeSerializerStrategyDecorator(new Int32SerializerStrategy());
			DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy();

			new Int32SerializerStrategy().Write(168938967, writer);

			DateTime dateTime = serializer.Read(new DefaultWireMemberReaderStrategy(writer.GetBytes()));


			//Assert
			Assert.AreEqual(23, dateTime.Minute);
			Assert.AreEqual(23, dateTime.Hour);
			Assert.AreEqual(8, dateTime.Day);
			Assert.AreEqual(2010, dateTime.Year);
			Assert.AreEqual(2, dateTime.Month);
		}
	}
}
