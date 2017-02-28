using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class BufferedPeekStreamTests
	{
		[Test]
		[TestCase((byte) 1)]
		[TestCase((byte) 55)]
		[TestCase((byte) 105)]
		[TestCase((byte) 0)]
		[TestCase((byte) 240)]
		public static void Test_Buffered_Peeking_Can_Peek_Byte(byte b)
		{
			//arrange
			DefaultStreamReaderStrategy reader = new DefaultStreamReaderStrategy(new byte[] {b});

			//act
			IWireStreamReaderStrategy peekedBufferReader = reader.PeekWithBuffering();

			//assert
			for (int i = 0; i < 5; i++)
				Assert.AreEqual(b, peekedBufferReader.PeekByte());

			Assert.AreEqual(b, peekedBufferReader.ReadByte());

			Assert.Throws<InvalidOperationException>(() => peekedBufferReader.ReadByte());
		}

		[Test]
		public static void Test_Buffered_Peeking_Can_Peek_Bytes()
		{
			//arrange
			DefaultStreamReaderStrategy reader = new DefaultStreamReaderStrategy(new byte[] {5, 6, 7, 5});

			//act
			IWireStreamReaderStrategy peekedBufferReader = reader.PeekWithBuffering();

			//assert
			for (int i = 0; i < 5; i++)
				Assert.AreEqual(5, peekedBufferReader.PeekByte());

			Assert.AreEqual(5, peekedBufferReader.ReadByte());
			Assert.AreEqual(6, peekedBufferReader.ReadByte());

			byte[] readBytes = peekedBufferReader.ReadAllBytes();

			Assert.AreEqual(7, readBytes[0]);
			Assert.AreEqual(5, readBytes[1]);

			Assert.Throws<InvalidOperationException>(() => peekedBufferReader.ReadByte());
		}

		[Test]
		public static void Test_Buffered_Peeking_Can_Peek_Counted_Bytes()
		{
			//arrange
			DefaultStreamReaderStrategy reader = new DefaultStreamReaderStrategy(new byte[] {5, 6, 7, 5});

			//act
			IWireStreamReaderStrategy peekedBufferReader = reader.PeekWithBuffering();
			byte[] peekedBytes = peekedBufferReader.PeekBytes(4);
			byte[] peekedBytesAgain = peekedBufferReader.PeekBytes(2);
			byte[] readBytes = peekedBufferReader.ReadBytes(4);


			//assert
			for (int i = 0; i < peekedBytes.Length; i++)
			{
				Assert.AreEqual(peekedBytes[i], readBytes[i]);
			}

			for (int i = 0; i < peekedBytesAgain.Length; i++)
				Assert.AreEqual(peekedBytes[i], peekedBytesAgain[i]);

			Assert.Throws<InvalidOperationException>(() => peekedBufferReader.ReadByte());
		}

		[Test]
		public static async Task Test_Buffered_Peeking_Can_Peek_Counted_Bytes_Async()
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] { 5, 6, 7, 5 });

			//act
			IWireStreamReaderStrategyAsync peekedBufferReader = reader.PeekWithBufferingAsync();
			byte[] peekedBytes = await peekedBufferReader.PeekBytesAsync(4);
			byte[] peekedBytesAgain = await peekedBufferReader.PeekBytesAsync(2);
			byte[] readBytes = await peekedBufferReader.ReadBytesAsync(4);


			//assert
			Assert.AreEqual(4, readBytes.Length);
			for (int i = 0; i < peekedBytes.Length; i++)
			{
				Assert.AreEqual(peekedBytes[i], readBytes[i]);
			}

			for (int i = 0; i < peekedBytesAgain.Length; i++)
				Assert.AreEqual(peekedBytes[i], peekedBytesAgain[i]);

			Assert.Throws<InvalidOperationException>(() => peekedBufferReader.ReadByte());
		}

		[Test]
		public static async Task Test_Buffered_Peeking_Can_Peek_Counted_Bytes_With_ReadAllAsync()
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] { 5, 6, 7, 5 });

			//act
			IWireStreamReaderStrategyAsync peekedBufferReader = reader.PeekWithBufferingAsync();
			byte[] peekedBytes = await peekedBufferReader.PeekBytesAsync(4);
			byte[] peekedBytesAgain = await peekedBufferReader.PeekBytesAsync(2);
			byte b = peekedBufferReader.ReadByte();
			byte[] readBytes = await peekedBufferReader.ReadAllBytesAsync();

			//assert
			Assert.AreEqual(3, readBytes.Length);
			for (int i = 0; i < readBytes.Length; i++)
			{
				Assert.AreEqual(peekedBytes[i + 1], readBytes[i]);
			}

			for (int i = 0; i < peekedBytesAgain.Length; i++)
				Assert.AreEqual(peekedBytes[i], peekedBytesAgain[i]);

			Assert.Throws<InvalidOperationException>(() => peekedBufferReader.ReadByte());
		}

		[Test]
		[TestCase((byte) 1)]
		[TestCase((byte) 55)]
		[TestCase((byte) 105)]
		[TestCase((byte) 0)]
		[TestCase((byte) 240)]
		public static async Task Test_Buffered_Peeking_Can_Peek_Byte_Async(byte b)
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] {b});

			//act
			IWireStreamReaderStrategyAsync peekedBufferReader = reader.PeekWithBufferingAsync();

			//assert
			for (int i = 0; i < 5; i++)
				Assert.AreEqual(b, await peekedBufferReader.PeekByteAsync());

			Assert.AreEqual(b, await peekedBufferReader.ReadByteAsync());

			Assert.Throws<AggregateException>(() =>
			{
				byte sdsdf = peekedBufferReader.ReadByteAsync().Result;
			});
		}

		[Test]
		public static async Task Test_Buffered_Peeking_Can_Peek_Bytes_Async()
		{
			//arrange
			DefaultStreamReaderStrategyAsync reader = new DefaultStreamReaderStrategyAsync(new byte[] {5, 6, 7, 5});

			//act
			IWireStreamReaderStrategyAsync peekedBufferReader = reader.PeekWithBufferingAsync();

			//assert
			for (int i = 0; i < 5; i++)
				Assert.AreEqual(5, await peekedBufferReader.PeekByteAsync());

			Assert.AreEqual(5, await peekedBufferReader.ReadByteAsync());
			Assert.AreEqual(6, await peekedBufferReader.ReadByteAsync());

			byte[] readBytes = await peekedBufferReader.ReadAllBytesAsync();

			Assert.AreEqual(7, readBytes[0]);
			Assert.AreEqual(5, readBytes[1]);

			Assert.Throws<AggregateException>(() =>
			{
				byte b = peekedBufferReader.ReadByteAsync().Result;
			});
		}
	}
}
