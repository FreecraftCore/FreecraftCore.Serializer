using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for strategies that mutate a binary chunk/buffer using some defined pattern or strategy.
	/// </summary>
	public interface IBinaryMutatorStrategy
	{
		/// <summary>
		/// Mutates the <see cref="source"/> using the strategy into the <see cref="destination"/> based on provided offsets.
		/// Mutate can be thought as WRITE.
		/// </summary>
		/// <param name="source">The source buffer to mutate.</param>
		/// <param name="sourceOffset">The offset to start at.</param>
		/// <param name="destination">The destination buffer to copy to.</param>
		/// <param name="destinationOffset">The destination buffer offset.</param>
		void Mutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset);

		/// <summary>
		/// UnMutates (reversed <see cref="Mutate"/>) the <see cref="source"/> using the reverse of the strategy into the <see cref="destination"/> based on provided offsets.
		/// UnMutate can be thought of as READ.
		/// </summary>
		/// <param name="source">The source buffer to unmutate.</param>
		/// <param name="sourceOffset">The offset to start at.</param>
		/// <param name="destination">The destination buffer to copy to.</param>
		/// <param name="destinationOffset">The destination buffer offset.</param>
		void UnMutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset);
	}
}
