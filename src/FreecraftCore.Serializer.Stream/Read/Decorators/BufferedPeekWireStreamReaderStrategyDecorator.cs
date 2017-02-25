using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorates a <see cref="IWireStreamReaderStrategy"/> and provides the ability to peek
	/// and seek through a reader that doesn't support peeking by default.
	/// </summary>
	public class BufferedPeekWireStreamReaderStrategyDecorator<TReaderType> : WireMemberReaderStrategyDecorator<TReaderType>
		where TReaderType : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Enumeration of reader states.
		/// </summary>
		protected enum State
		{
			/// <summary>
			/// Indicates the default state.
			/// </summary>
			Default = 0,

			/// <summary>
			/// Indicates that the state is peeked.
			/// </summary>
			Peeked = 1,
		}

		/// <summary>
		/// Lazy collection of the buffered bytes.
		/// </summary>
		protected IEnumerable<byte> BufferedBytes { get; set; }

		//This is kept seperate for O(1) counting.
		//BufferedBytes.Count() may be an expensive operation
		protected int BufferedCount { get; set; } = 0;

		protected State ReaderState { get; set; }

		public BufferedPeekWireStreamReaderStrategyDecorator([NotNull] TReaderType decoratedReader)
			: base(decoratedReader)
		{
			//TODO: Examine how costly this is. We could just use null.
			BufferedBytes = Enumerable.Empty<byte>();
			ReaderState = State.Default;
		}

		public override byte ReadByte()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadByte();
			else
			{
				return ReadBytes(1)[0];
			}
		}

		public override byte PeekByte()
		{
			//More efficient to get the byte array
			//We need to buffer it anyway
			byte[] bytes = ReadBytes(1);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			//When we peek we must buffer the results.
			BufferedBytes = bytes.Concat(BufferedBytes);

			BufferedCount++;

			return bytes[0];
		}

		public override byte[] PeekBytes(int count)
		{
			//When we peek we must buffer the results.
			byte[] bytes = ReadBytes(count);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			BufferedBytes = bytes.Concat(BufferedBytes);
			BufferedCount += bytes.Length;

			return bytes;
		}

		public override byte[] ReadAllBytes()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadAllBytes();
			else
			{
				ReaderState = State.Default;
				BufferedCount = 0;

				byte[] bytes = BufferedBytes.Concat(DecoratedReader.ReadAllBytes()).ToArray();

				BufferedBytes = Enumerable.Empty<byte>();

				return bytes;
			}
		}

		public override byte[] ReadBytes(int count)
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadBytes(count);
			else
			{
				byte[] bytes = null;
				if (count == BufferedCount)
					bytes = BufferedBytes.ToArray();
				else if (count < BufferedCount)
				{
					bytes = BufferedBytes.Take(count).ToArray();

					//Skip the amount pulled out
					BufferedBytes = BufferedBytes.Skip(count);
				}
				else
					bytes = BufferedBytes.Concat(DecoratedReader.ReadBytes(count - BufferedCount)).ToArray();

				//Set the new size of the buffered byte chunk
				BufferedCount = Math.Max(0, BufferedCount - count);

				if (BufferedCount == 0)
				{
					ReaderState = State.Default;
					BufferedBytes = Enumerable.Empty<byte>();
				}

				return bytes;
			}
		}
	}
}
