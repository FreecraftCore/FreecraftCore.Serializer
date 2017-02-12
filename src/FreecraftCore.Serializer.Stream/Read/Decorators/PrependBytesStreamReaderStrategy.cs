using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class PrependBytesStreamReaderStrategy : WireMemberReaderStrategyDecorator
	{
		/// <summary>
		/// The prended bytes.
		/// </summary>
		[NotNull]
		private byte[] PrependedBytes { get; }

		/// <summary>
		/// The prepended counter.
		/// </summary>
		private int ByteCount { get; set; } = 0;

		/// <summary>
		/// Indicates if the pretended portion is finished reading.
		/// </summary>
		private bool isPrendedBytesFinished => (PrependedBytes.Length == ByteCount);

		public PrependBytesStreamReaderStrategy([NotNull] IWireStreamReaderStrategy decoratedReader, [NotNull] byte[] bytes) 
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
		public override byte[] PeakBytes(int count)
		{
			if (isPrendedBytesFinished)
				return DecoratedReader.PeakBytes(count);
			else
			{
				byte[] bytes = null;

				if (count <= PrependedBytes.Length - ByteCount)
					bytes = PrependedBytes.Skip(ByteCount)
						.Take(count)
						.ToArray();
				else //We need to combine
					bytes = PrependedBytes.Skip(ByteCount)
						.Concat(DecoratedReader.PeakBytes(count - (PrependedBytes.Length - ByteCount)))
						.ToArray();

				//Peaking so don't move the bytecount forward
				return bytes;
			}
		}
	}
}
