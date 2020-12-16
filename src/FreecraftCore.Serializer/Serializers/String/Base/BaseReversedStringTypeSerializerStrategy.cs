using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for base string serialization strategy that reverses the string.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseReversedStringTypeSerializerStrategy<TChildType> : BaseStringTypeSerializerStrategy<TChildType>, IStringTypeSerializerStrategy
		where TChildType : BaseReversedStringTypeSerializerStrategy<TChildType>, new()
	{
		protected BaseReversedStringTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{
			if (CharacterSize > 1)
				throw new InvalidOperationException($"TODO: Implement multi-byte non-fixed width grapheme support.");
		}

		/// <inheritdoc />
		protected override unsafe string ReadEncodedString(byte* bytes, int trueStringSize)
		{
			//This implementation will ONLY ever work for 1 byte ASCII
			//We reverse the binary data in the current span
			Span<byte> reversedSpan = new Span<byte>(bytes, trueStringSize);
			int srcOffset = 0;
			int dstOffset = 0;
			ReverseBinaryMutatorStrategy.Instance.Mutate(reversedSpan, ref srcOffset, reversedSpan, ref dstOffset);

			fixed (byte* reversedBytes = &reversedSpan.GetPinnableReference())
			{
				return base.ReadEncodedString(reversedBytes, trueStringSize);
			}
		}

		/// <inheritdoc />
		protected override unsafe int WriteEncodedString(char* chars, int charsLength, byte* bytes, int byteLength)
		{
			//This implementation will ONLY ever work for 1 byte ASCII
			//The encoded bytes have been written and we know byte length and start position
			//start pos is 0 since base slices
			int byteCount = base.WriteEncodedString(chars, charsLength, bytes, byteLength);

			//Unlike read, we must reverse in-place after the fact.
			Span<byte> reversedSpan = new Span<byte>(bytes, byteCount);
			int srcOffset = 0;
			int dstOffset = 0;
			ReverseBinaryMutatorStrategy.Instance.Mutate(reversedSpan, ref srcOffset, reversedSpan, ref dstOffset);

			return byteCount;
		}
	}
}
