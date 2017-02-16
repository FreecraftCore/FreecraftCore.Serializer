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
		public Task<byte> ReadByteAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return DecoratedReader.ReadByteAsync();
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			ThrowIfCantRead();
			CanRead = false;

			return DecoratedReader.ReadAllBytesAsync();
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			ThrowIfCantRead();
			CanRead = false;

			return DecoratedReader.ReadBytesAsync(count);
		}

		/// <inheritdoc />
		public Task<byte[]> PeakBytesAsync(int count)
		{
			ThrowIfCantRead();
			CanRead = false;

			return DecoratedReader.PeakBytesAsync(count);
		}
	}
}
