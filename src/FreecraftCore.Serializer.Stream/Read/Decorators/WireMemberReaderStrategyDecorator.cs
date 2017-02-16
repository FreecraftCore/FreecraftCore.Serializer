using System;
using System.Collections.Generic;
using System.Linq;
#if !NET35
using System.Threading.Tasks;
#endif
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public abstract class WireMemberReaderStrategyDecorator : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Decorated reader.
		/// </summary>
		[NotNull]
		protected IWireStreamReaderStrategy DecoratedReader { get; }

		protected WireMemberReaderStrategyDecorator([NotNull] IWireStreamReaderStrategy decoratedReader)
		{
			if (decoratedReader == null) throw new ArgumentNullException(nameof(decoratedReader));

			DecoratedReader = decoratedReader;
		}

		/// <inheritdoc />
		public virtual void Dispose()
		{
			//We likely don't own the strategy so do nothing.
		}

		/// <inheritdoc />
		public abstract byte ReadByte();

		/// <inheritdoc />
		public abstract byte PeekByte();

		/// <inheritdoc />
		public abstract byte[] ReadAllBytes();

		/// <inheritdoc />
		public abstract byte[] ReadBytes(int count);

		/// <inheritdoc />
		public abstract byte[] PeakBytes(int count);

		//Async task methods
#if !NET35
		/// <inheritdoc />
		public abstract Task<byte> ReadByteAsync();

		/// <inheritdoc />
		public abstract Task<byte> PeekByteAsync();

		/// <inheritdoc />
		public abstract Task<byte[]> ReadAllBytesAsync();

		/// <inheritdoc />
		public abstract Task<byte[]> ReadBytesAsync(int count);

		/// <inheritdoc />
		public abstract Task<byte[]> PeakBytesAsync(int count);

#endif
	}
}
