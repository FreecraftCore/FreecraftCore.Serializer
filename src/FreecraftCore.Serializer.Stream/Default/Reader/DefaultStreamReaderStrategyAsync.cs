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
	/// Default async implementation of the <see cref="IWireStreamReaderStrategy"/> that reads bytes from
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamReaderStrategyAsync : DefaultStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public DefaultStreamReaderStrategyAsync([NotNull] byte[] bytes) 
			: base(bytes)
		{

		}

		/// <inheritdoc />
		public DefaultStreamReaderStrategyAsync([NotNull] Stream stream) 
			: base(stream)
		{

		}

		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			byte[] singleByteBuffer = new byte[1];

			//would be -1 if it's invalid
			int readResult = await ManagedStream.ReadAsync(singleByteBuffer, 0, 1);

			if(readResult == 0)
				throw new InvalidOperationException($"Failed to read byte in {nameof(ReadByteAsync)}.");

			return singleByteBuffer[0];
		}

		/// <inheritdoc />
		public async Task<byte> PeekByteAsync()
		{
			byte resultByte = await ReadByteAsync();

			//Move it back one
			ManagedStream.Position = ManagedStream.Position - 1;
			return resultByte;
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAllBytesAsync()
		{
			return await ReadBytesAsync((int)(ManagedStream.Length - ManagedStream.Position));
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			byte[] bytes = new byte[count];

			int readCount = await ManagedStream.ReadAsync(bytes, 0, count);

			if (readCount != count)
				throw new InvalidOperationException($"Failed to read {count} bytes from the stream.");

			return bytes;
		}

		/// <inheritdoc />
		public async Task<byte[]> PeekBytesAsync(int count)
		{
			byte[] bytes = await ReadBytesAsync(count);

			//Now move the stream back
			ManagedStream.Position = ManagedStream.Position - count;

			return bytes;
		}
	}
}
