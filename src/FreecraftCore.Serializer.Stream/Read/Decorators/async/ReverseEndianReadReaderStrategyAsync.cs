using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class ReverseEndianReadReaderStrategyAsync<TReaderType> : ReverseEndianReadReaderStrategy<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public ReverseEndianReadReaderStrategyAsync([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			return await DecoratedReader.ReadByteAsync();
		}

		/// <inheritdoc />
		public async Task<byte> PeekByteAsync()
		{
			return await DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAllBytesAsync()
		{
			byte[] bytes = await DecoratedReader.ReadAllBytesAsync();

			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			byte[] bytes = await DecoratedReader.ReadBytesAsync(count);

			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public async Task<byte[]> PeekBytesAsync(int count)
		{
			//Blocks until the bytes are loaded
			byte[] bytes = await DecoratedReader.PeekBytesAsync(count);

			Array.Reverse(bytes);

			return bytes;
		}
	}
}
