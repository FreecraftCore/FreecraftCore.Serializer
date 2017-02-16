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
		public Task<byte> ReadByteAsync()
		{
			return DecoratedReader.ReadByteAsync();
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			return DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.ReadAllBytesAsync().Result;

				Array.Reverse(bytes);

				return bytes;
			});
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.ReadBytesAsync(count).Result;

				Array.Reverse(bytes);

				return bytes;
			});
		}

		/// <inheritdoc />
		public Task<byte[]> PeakBytesAsync(int count)
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.PeakBytesAsync(count).Result;

				Array.Reverse(bytes);

				return bytes;
			});
		}
	}
}
