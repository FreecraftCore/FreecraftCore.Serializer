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
	/// Decorator that reverses byte-order of the read byte chunks.
	/// Has no affect on single bytes. Doesn't reorder stream. Only reverses read chunks.
	/// </summary>
	public class ReverseEndianReadReaderStrategy<TReaderType> : WireMemberReaderStrategyDecorator<TReaderType>
		where TReaderType : IWireStreamReaderStrategy
	{
		public ReverseEndianReadReaderStrategy([NotNull] TReaderType decoratedReader) 
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
		public override byte[] PeekBytes(int count)
		{
			byte[] bytes = DecoratedReader.PeekBytes(count);
			Array.Reverse(bytes);

			return bytes;
		}
	}
}
