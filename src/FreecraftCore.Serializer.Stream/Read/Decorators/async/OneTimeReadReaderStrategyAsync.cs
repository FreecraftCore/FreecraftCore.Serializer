using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class OneTimeReadReaderStrategyAsync<TReaderType> : OneTimeReadReaderStrategy<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public OneTimeReadReaderStrategyAsync([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return await DecoratedReader.ReadByteAsync();
		}

		/// <inheritdoc />
		public async Task<byte> PeekByteAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return await DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadAllBytesAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return await DecoratedReader.ReadAllBytesAsync();
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			ThrowIfCantRead();
			CanRead = false;

			return await DecoratedReader.ReadBytesAsync(count);
		}

		/// <inheritdoc />
		public async Task<byte[]> PeekBytesAsync(int count)
		{
			ThrowIfCantRead();
			CanRead = false;

			return await DecoratedReader.PeekBytesAsync(count);
		}
	}
}
