using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Default implementation of the <see cref="IWireStreamReaderStrategy"/> that reads bytes from
	/// an internally managed stream.
	/// </summary>
	public class DefaultWireMemberReaderStrategy : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Allocated stream with the byte buffer.
		/// </summary>
		[NotNull]
		private Stream ReaderStream { get; }

		//TODO: Overloads that take the byte buffer instead
		public DefaultWireMemberReaderStrategy([NotNull] byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes), $"Provided argument {nameof(bytes)} must not be null.");

			ReaderStream = new MemoryStream(bytes);
		}

		public byte[] ReadAllBytes()
		{
			return ReadBytes((int)(ReaderStream.Length - ReaderStream.Position));
		}

		public byte ReadByte()
		{
			//would be -1 if it's invalid
			int b = ReaderStream.ReadByte();

			//TODO: Contract interface doesn't mention throwing in this case. Should we throw?
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

		public void Dispose()
		{
			ReaderStream?.Dispose();
		}
	}
}
