using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	public class DefaultWireMemberReaderStrategy : IWireMemberReaderStrategy, IDisposable
	{
		public bool isDisposed { get; private set; } = false;

		/// <summary>
		/// Allocated stream with the byte buffer.
		/// </summary>
		private Stream ReaderStream { get; } = new MemoryStream();

		//TODO: Overloads that take the byte buffer instead
		public DefaultWireMemberReaderStrategy(byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes), $"Provided argument {nameof(bytes)} must not be null.");

			//Empty classes can produce empty byte buffers
			/*if (bytes.Length == 0)
				throw new ArgumentException($"Provided argument {nameof(bytes)} must not be null.", nameof(bytes));*/

			ReaderStream = new MemoryStream(bytes);
		}

		public void Dispose()
		{
			ReaderStream.Dispose();
			isDisposed = true;
		}

		public byte[] ReadAllBytes()
		{
			return ReadBytes((int)(ReaderStream.Length - ReaderStream.Position));
		}

		public byte ReadByte()
		{
			//would be -1 if it's invalid
			int b = ReaderStream.ReadByte();

			if (b == -1)
				throw new InvalidOperationException("Failed to read a desired byte from the stream.");

			return (byte)b;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] bytes = new byte[count];

			ReaderStream.Read(bytes, 0, count);

			return bytes;
		}

		public byte PeekByte()
		{
			byte b = ReadByte();

			//Move it back one
			ReaderStream.Position = ReaderStream.Position - 1;

			return b;
		}

		public byte[] PeakBytes(int count)
		{
			byte[] bytes = ReadBytes(count);

			//Now move the stream back
			ReaderStream.Position = ReaderStream.Position - count;

			return bytes;
		}
	}
}
