using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Default implementation of the <see cref="IWireStreamReaderStrategy"/> that reads bytes from
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamReaderStrategy : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Allocated stream with the byte buffer.
		/// </summary>
		[NotNull]
		private Stream ReaderStream { get; }

		/// <summary>
		/// Indicates if the stream should be disposed when the reader is disposed.
		/// </summary>
		private bool shouldDisposeStream { get; }

		//TODO: Overloads that take the byte buffer instead
		public DefaultStreamReaderStrategy([NotNull] byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes), $"Provided argument {nameof(bytes)} must not be null.");

			ReaderStream = new MemoryStream(bytes);
			shouldDisposeStream = true;
		}

		public DefaultStreamReaderStrategy([NotNull] Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream), $"Provided argument {nameof(stream)} must not be null.");

			ReaderStream = stream;
			shouldDisposeStream = false;
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
			if(shouldDisposeStream)
				ReaderStream?.Dispose();
		}


#if !NET35
		/// <inheritdoc />
		public Task<byte> ReadByteAsync()
		{
			byte[] singleByteBuffer = new byte[1];

			//would be -1 if it's invalid
			Task<int> readResult = ReaderStream.ReadAsync(singleByteBuffer, 0, 1);

			return new Task<byte>(() =>
			{
				int resultCount = readResult.Result;

				if(resultCount == 0)
					throw new InvalidOperationException($"Failed to read byte in {nameof(ReadByteAsync)}.");

				return singleByteBuffer[0];
			});
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			Task<byte> b = ReadByteAsync();

			return new Task<byte>(() =>
			{
				byte resultByte = b.Result;

				//Move it back one
				ReaderStream.Position = ReaderStream.Position - 1;
				return resultByte;
			});
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			return ReadBytesAsync((int)(ReaderStream.Length - ReaderStream.Position));
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			
			byte[] bytes = new byte[count];

			Task<int> readTask = ReaderStream.ReadAsync(bytes, 0, count);

			return new Task<byte[]>(() =>
			{
				int resultCount = readTask.Result;

				if (resultCount != count)
					throw new InvalidOperationException($"Failed to read {count} bytes from the stream.");

				return bytes;
			});
		}

		/// <inheritdoc />
		public Task<byte[]> PeakBytesAsync(int count)
		{
			Task<byte[]> bytes = ReadBytesAsync(count);

			return new Task<byte[]>(() =>
			{
				//Now move the stream back
				ReaderStream.Position = ReaderStream.Position - count;

				return bytes.Result;
			});
		}
#endif
	}
}
