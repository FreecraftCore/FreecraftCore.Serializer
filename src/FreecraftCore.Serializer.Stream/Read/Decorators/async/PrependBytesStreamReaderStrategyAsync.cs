using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class PrependBytesStreamReaderStrategyAsync<TReaderType> : PrependBytesStreamReaderStrategy<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public PrependBytesStreamReaderStrategyAsync([NotNull] TReaderType decoratedReader, [NotNull] byte[] bytes) 
			: base(decoratedReader, bytes)
		{

		}

		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			if (isPrendedBytesFinished)
				return await DecoratedReader.ReadByteAsync();
			else
				return PrependedBytes[ByteCount++]; //always available because we check length
		}

		/// <inheritdoc />
		public async Task<byte> PeekByteAsync()
		{
			if (isPrendedBytesFinished)
				return await DecoratedReader.PeekByteAsync();
			else
			{
				return PrependedBytes[ByteCount];
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAllBytesAsync()
		{
			if (isPrendedBytesFinished)
				return await DecoratedReader.ReadAllBytesAsync();
			else
			{
				return PrependedBytes.Skip(ByteCount)
					.Concat(await DecoratedReader.ReadAllBytesAsync())
					.ToArray();
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			if (isPrendedBytesFinished)
				return await DecoratedReader.ReadBytesAsync(count);
			else
			{
				byte[] bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray();
				else //We need to combine
					bytes = PrependedBytes.Skip(ByteCount)
						.Concat(await DecoratedReader.ReadBytesAsync(count - (PrependedBytes.Length - ByteCount)))
						.ToArray();

				//Set the byte count as finished or forward as many as possible
				ByteCount = Math.Min(count + ByteCount, PrependedBytes.Length);

				return bytes;
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> PeekBytesAsync(int count)
		{
			if (isPrendedBytesFinished)
				return await DecoratedReader.PeekBytesAsync(count);
			else
			{
				byte[] bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray();
				else //We need to combine
					bytes = PrependedBytes.Skip(ByteCount)
						.Concat(await DecoratedReader.PeekBytesAsync(count - (PrependedBytes.Length - ByteCount)))
						.ToArray();

				//Peeking so don't move the bytecount forward
				return bytes;
			}
		}
	}
}
