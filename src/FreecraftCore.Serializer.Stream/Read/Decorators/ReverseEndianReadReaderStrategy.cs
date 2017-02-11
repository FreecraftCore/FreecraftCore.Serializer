using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorator that reverses byte-order of the read byte chunks.
	/// Has no affect on single bytes. Doesn't reorder stream. Only reverses read chunks.
	/// </summary>
	public class ReverseEndianReadReaderStrategy : WireMemberReaderStrategyDecorator
	{
		public ReverseEndianReadReaderStrategy([NotNull] IWireStreamReaderStrategy decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public override byte ReadByte()
		{
			return DecoratedReader.ReadByte();
		}

		/// <inheritdoc />
		public override byte PeekByte()
		{
			return DecoratedReader.PeekByte();
		}

		/// <inheritdoc />
		public override byte[] ReadAllBytes()
		{
			byte[] bytes = DecoratedReader.ReadAllBytes();
			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public override byte[] ReadBytes(int count)
		{
			byte[] bytes = DecoratedReader.ReadBytes(count);
			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public override byte[] PeakBytes(int count)
		{
			byte[] bytes = DecoratedReader.PeakBytes(count);
			Array.Reverse(bytes);

			return bytes;
		}
	}
}
