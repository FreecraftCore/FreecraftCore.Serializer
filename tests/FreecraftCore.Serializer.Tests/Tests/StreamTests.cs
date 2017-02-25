using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class StreamTests
	{
		[Test]
		//[TestCase(new byte[] { 1, 100, 42, 53, 63, 56 })]
		//[TestCase(new byte[] { 56, 1, 42, 53, 63, 1 })]
		//[TestCase(new byte[] { 5, 100, 42, 53, 78 })]
		[TestCase(new byte[] { 89, 45, 67, 53, 28 })]
		public static void Test_That_Prepended_Reader_Contains_Bytes(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategy reader = new DefaultStreamReaderStrategy(new byte[0]);

			//act
			IWireStreamReaderStrategy bufferedReader = reader.PreprendWithBytes(bytes);

			//assert
			for(int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(bufferedReader.ReadByte(), bytes[i]);
		}

		[Test]
		//[TestCase(new byte[] { 5, 100, 42, 53, 63, 77 })]
		//[TestCase(new byte[] { 78, 1, 42, 53, 63, 1, 5 })]
		//[TestCase(new byte[] { 12, 100, 42, 53, 0, 1, 1, 1, 1 })]
		[TestCase(new byte[] { 1, 45, 67, 53, 63, 5, 6, 7, 4 })]
		public static async Task Test_That_Prepended_Reader_Contains_Bytes_Async(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[0]);

			//act
			IWireStreamReaderStrategyAsync bufferedReader = reader.PreprendWithBytesAsync(bytes);

			//assert
			for (int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(await bufferedReader.ReadByteAsync(), bytes[i]);
		}

		[Test]
		//[TestCase(new byte[] { 123, 100, 42, 53, 63 })]
		//[TestCase(new byte[] { 12, 1, 42, 53, 63, 1 })]
		//[TestCase(new byte[] { 5, 100, 42, 53, 0, 55, 22 })]
		[TestCase(new byte[] { 23, 45, 67, 53, 63, 1, 5 })]
		public static async Task Test_That_Prepended_Reader_Contains_Bytes_All_At_Once_Async(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[0]);

			//act
			IWireStreamReaderStrategyAsync bufferedReader = reader.PreprendWithBytesAsync(bytes);
			byte[] readBytes = await bufferedReader.ReadAllBytesAsync();

			//assert
			for (int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(readBytes[i], bytes[i]);
		}

		[Test]
		//[TestCase(new byte[] { 123, 100, 42, 53, 63 })]
		//[TestCase(new byte[] { 12, 1, 42, 53, 63, 1 })]
		//[TestCase(new byte[] { 5, 100, 42, 53, 0, 55, 22 })]
		[TestCase(new byte[] { 23, 45, 67, 53, 63, 1, 5 })]
		public static async Task Test_That_Prepended_Reader_Contains_Bytes_Specific_Amount_Async(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[0]);

			//act
			IWireStreamReaderStrategyAsync bufferedReader = reader.PreprendWithBytesAsync(bytes);
			byte[] readBytes = await bufferedReader.ReadBytesAsync(bytes.Length);

			//assert
			for (int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(readBytes[i], bytes[i]);
		}

		[Test]
		//[TestCase(new byte[] { 123, 100, 42, 53, 63 })]
		//[TestCase(new byte[] { 12, 1, 42, 53, 63, 1 })]
		//[TestCase(new byte[] { 5, 100, 42, 53, 0, 55, 22 })]
		[TestCase(new byte[] { 23, 45, 67, 53, 63, 1, 5 })]
		public static async Task Test_That_Prepended_Reader_Contains_Bytes_After_The_Prended_Portion_Async(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] { 5, 7 });

			//act
			IWireStreamReaderStrategyAsync bufferedReader = reader.PreprendWithBytesAsync(bytes);
			byte[] readBytes = await bufferedReader.ReadBytesAsync(bytes.Length);

			//assert
			for (int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(readBytes[i], bytes[i]);

			Assert.AreEqual(5, await bufferedReader.ReadByteAsync());
			Assert.AreEqual(7, await bufferedReader.ReadByteAsync());
		}

		[Test]
		//[TestCase(new byte[] { 123, 100, 42, 53, 63 })]
		//[TestCase(new byte[] { 12, 1, 42, 53, 63, 1 })]
		//[TestCase(new byte[] { 5, 100, 42, 53, 0, 55, 22 })]
		[TestCase(new byte[] { 23, 45, 67, 53, 63, 1, 5 })]
		public static void Test_That_Prepended_Reader_Contains_Bytes_After_The_Prended_Portion(byte[] bytes)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] { 5, 7 });

			//act
			IWireStreamReaderStrategyAsync bufferedReader = reader.PreprendWithBytesAsync(bytes);
			byte[] readBytes = bufferedReader.ReadBytes(bytes.Length);

			//assert
			for (int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(readBytes[i], bytes[i]);

			Assert.AreEqual(5, bufferedReader.ReadByte());
			Assert.AreEqual(7, bufferedReader.ReadByte());
		}
	}
}
