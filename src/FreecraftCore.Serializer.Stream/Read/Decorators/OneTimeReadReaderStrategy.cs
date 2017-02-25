using System;
using System.Collections.Generic;
using System.Linq;
#if !NET35
using System.Threading.Tasks;
#endif
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorates a stream the semantics of only being able to read from the stream a single time.
	/// </summary>
	public class OneTimeReadReaderStrategy<TReaderType> : WireMemberReaderStrategyDecorator<TReaderType>
		where TReaderType : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Indicates if the strategy has read yet.
		/// True if the strategy has yet to be read from.
		/// </summary>
		protected bool CanRead { get; set; }

		public OneTimeReadReaderStrategy([NotNull] TReaderType decoratedReader)
			: base(decoratedReader)
		{
			CanRead = true;
		}

		public override byte ReadByte()
		{
			ThrowIfCantRead();
			CanRead = false;
			return DecoratedReader.ReadByte();
		}

		/// <inheritdoc />
		public override byte PeekByte()
		{
			ThrowIfCantRead();
			CanRead = false;
			return DecoratedReader.PeekByte();
		}

		/// <inheritdoc />
		public override byte[] ReadAllBytes()
		{
			ThrowIfCantRead();
			CanRead = false;
			return DecoratedReader.ReadAllBytes();
		}

		/// <inheritdoc />
		public override byte[] ReadBytes(int count)
		{
			ThrowIfCantRead();
			CanRead = false;
			return DecoratedReader.ReadBytes(count);
		}

		/// <inheritdoc />
		public override byte[] PeekBytes(int count)
		{
			ThrowIfCantRead();
			CanRead = false;
			return DecoratedReader.PeekBytes(count);
		}

		protected void ThrowIfCantRead()
		{
			if(!CanRead)
				throw new InvalidOperationException($"Cannot read multiple times with {nameof(OneTimeReadReaderStrategy<TReaderType>)}.");
		}
	}
}
