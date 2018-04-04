using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorates a <see cref="IWireStreamReaderStrategy"/> with the ability to only peek the underlying reader.
	/// </summary>
	/// <typeparam name="TReaderType">The reader type.</typeparam>
	public class PeekOnlyWireStreamReaderStrategyAsync<TReaderType> : PeekOnlyWireStreamReaderStrategy<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public PeekOnlyWireStreamReaderStrategyAsync([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public Task<byte> ReadByteAsync()
		{
			//Force a peek
			return DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			return DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			//TODO: Implement a peek method for all bytes.
			throw new NotImplementedException($"Cannot force a peek for all bytes.");
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			//force peeking
			return DecoratedReader.PeekBytesAsync(count);
		}

		/// <inheritdoc />
		public Task<byte[]> PeekBytesAsync(int count)
		{
			return DecoratedReader.PeekBytesAsync(count);
		}
	}
}
