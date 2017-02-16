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
		public Task<byte> ReadByteAsync()
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.PeekByteAsync();
			else
				return Task.FromResult(PrependedBytes[ByteCount++]); //always available because we check length
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.PeekByteAsync();
			else
			{
				return Task.FromResult(PrependedBytes[ByteCount]);
			}
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.ReadAllBytesAsync();
			else
			{
				return new Task<byte[]>(() => PrependedBytes.Skip(ByteCount)
					.Concat(new EnumerableAsyncBytes(DecoratedReader.ReadAllBytesAsync()))
					.ToArray());
			}
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.ReadBytesAsync(count);
			else
			{
				Task<byte[]> bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = Task.FromResult(PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray());
				else //We need to combine
					bytes = new Task<byte[]>(() => PrependedBytes.Skip(ByteCount)
						.Concat(new EnumerableAsyncBytes(DecoratedReader.ReadBytesAsync(count - (PrependedBytes.Length - ByteCount))))
						.ToArray());

				//Set the byte count as finished or forward as many as possible
				ByteCount = Math.Min(count + ByteCount, PrependedBytes.Length);

				return bytes;
			}
		}

		/// <inheritdoc />
		public Task<byte[]> PeakBytesAsync(int count)
		{
			if (isPrendedBytesFinished)
				return Task.FromResult(DecoratedReader.PeakBytes(count));
			else
			{
				Task<byte[]> bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = Task.FromResult(PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray());
				else //We need to combine
					bytes = new Task<byte[]>(() => PrependedBytes.Skip(ByteCount)
						.Concat(new EnumerableAsyncBytes(DecoratedReader.PeakBytesAsync(count - (PrependedBytes.Length - ByteCount))))
						.ToArray());

				//Peaking so don't move the bytecount forward
				return bytes;
			}
		}
	}
}
