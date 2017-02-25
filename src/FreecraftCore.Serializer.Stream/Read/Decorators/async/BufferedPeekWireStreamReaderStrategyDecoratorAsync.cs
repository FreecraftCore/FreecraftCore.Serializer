using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class BufferedPeekWireStreamReaderStrategyDecoratorAsync<TReaderType> : BufferedPeekWireStreamReaderStrategyDecorator<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public BufferedPeekWireStreamReaderStrategyDecoratorAsync([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			if (ReaderState == State.Default)
				return await DecoratedReader.ReadByteAsync();
			else
			{
				return (await ReadBytesAsync(1))[0];
			}
		}

		/// <inheritdoc />
		public async Task<byte> PeekByteAsync()
		{
			//More efficient to get the byte array
			//We need to buffer it anyway
			byte[] bytes = await ReadBytesAsync(1);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			//When we peek we must buffer the results.
			BufferedBytes = bytes.Concat(BufferedBytes);

			BufferedCount++;

			return bytes[0];
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAllBytesAsync()
		{
			if (ReaderState == State.Default)
				return await DecoratedReader.ReadAllBytesAsync();
			else
			{
				//Make sure to read outside the return task
				byte[] bytes = await DecoratedReader.ReadAllBytesAsync();

				ReaderState = State.Default;
				BufferedCount = 0;

				byte[] array = BufferedBytes.Concat(bytes).ToArray();
				BufferedBytes = Enumerable.Empty<byte>();
				return array;
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			if (ReaderState == State.Default)
				return await DecoratedReader.ReadBytesAsync(count);
			else
			{
				byte[] bytes = null;

				if (count == BufferedCount)
					bytes = BufferedBytes.ToArray();
				else if (count <= BufferedCount)
				{
					bytes = BufferedBytes.Take(count).ToArray();

					//Skip the amount pulled out
					BufferedBytes = BufferedBytes.Skip(count);
				}
				else
				{
					byte[] streamReadBytes = await DecoratedReader.ReadBytesAsync(count - BufferedCount);

					bytes = BufferedBytes.Concat(streamReadBytes).ToArray();
				}

				//Set the new size of the buffered byte chunk
				BufferedCount = Math.Max(0, BufferedCount - count);

				if (BufferedCount == 0)
				{
					ReaderState = State.Default;
					BufferedBytes = Enumerable.Empty<byte>();
				}
				return bytes;
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> PeekBytesAsync(int count)
		{
			//When we peek we must buffer the results.
			byte[] bytes = await ReadBytesAsync(count);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			BufferedBytes = bytes.Concat(BufferedBytes);
			BufferedCount += bytes.Length;

			return bytes;
		}
	}
}
