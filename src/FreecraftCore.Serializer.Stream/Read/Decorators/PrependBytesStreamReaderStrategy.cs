using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if !NET35
using System.Threading.Tasks;
#endif
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class PrependBytesStreamReaderStrategy<TReaderType> : WireMemberReaderStrategyDecorator<TReaderType>
		where TReaderType : IWireStreamReaderStrategy
	{
		/// <summary>
		/// The prended bytes.
		/// </summary>
		[NotNull]
		protected byte[] PrependedBytes { get; set; }

		/// <summary>
		/// The prepended counter.
		/// </summary>
		protected int ByteCount { get; set; } = 0;

		/// <summary>
		/// Indicates if the pretended portion is finished reading.
		/// </summary>
		protected bool isPrendedBytesFinished => (PrependedBytes.Length == ByteCount);

		public PrependBytesStreamReaderStrategy([NotNull] TReaderType decoratedReader, [NotNull] byte[] bytes) 
			: base(decoratedReader)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			PrependedBytes = bytes;
		}

		/// <inheritdoc />
		public override byte ReadByte()
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.ReadByte();
			else
				return PrependedBytes[ByteCount++]; //always available because we check length
		}

		/// <inheritdoc />
		public override byte PeekByte()
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.PeekByte();
			else
			{
				return PrependedBytes[ByteCount];
			}
		}

		/// <inheritdoc />
		public override byte[] ReadAllBytes()
		{
			if(isPrendedBytesFinished)
				return DecoratedReader.ReadAllBytes();
			else
			{
				return PrependedBytes.Skip(ByteCount)
					.Concat(DecoratedReader.ReadAllBytes())
					.ToArray();
			}
		}

		/// <inheritdoc />
		public override byte[] ReadBytes(int count)
		{
			if(isPrendedBytesFinished)
				return DecoratedReader.ReadBytes(count);
			else
			{
				byte[] bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray();
				else //We need to combine
					bytes = PrependedBytes.Skip(ByteCount)
						.Concat(DecoratedReader.ReadBytes(count - (PrependedBytes.Length - ByteCount)))
						.ToArray();

				//Set the byte count as finished or forward as many as possible
				ByteCount = Math.Min(count + ByteCount, PrependedBytes.Length);

				return bytes;
			}
		}

		/// <inheritdoc />
		public override byte[] PeekBytes(int count)
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.PeekBytes(count);
			else
			{
				byte[] bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray();
				else //We need to combine
					bytes = PrependedBytes.Skip(ByteCount)
						.Concat(DecoratedReader.PeekBytes(count - (PrependedBytes.Length - ByteCount)))
						.ToArray();

				//Peeking so don't move the bytecount forward
				return bytes;
			}
		}
	}
}
