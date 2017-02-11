using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
