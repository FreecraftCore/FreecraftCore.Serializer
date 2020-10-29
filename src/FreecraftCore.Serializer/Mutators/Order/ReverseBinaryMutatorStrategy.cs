using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Binary mutator strategy for reversing a binary chunk/stream.
	/// See: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/MemoryExtensions.cs
	/// </summary>
	public sealed class ReverseBinaryMutatorStrategy : StatelessBinaryMutatorStrategy<ReverseBinaryMutatorStrategy>
	{
		//WRITE
		public override unsafe void Mutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset)
		{
			//See: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/MemoryExtensions.cs
			Reverse(source, sourceOffset, destination, destinationOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe void Reverse(Span<byte> source, int sourceOffset, Span<byte> destination, int destinationOffset)
		{
			//See: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/MemoryExtensions.cs
			source = source.Slice(sourceOffset);
			destination = destination.Slice(destinationOffset);

			source.Reverse(); //Not LINQ Reverse, this is a efficient span reverse.

			fixed (byte* sourcePtr = &source.GetPinnableReference())
			fixed (byte* destPtr = &destination.GetPinnableReference())
			{
				//It's possible that the source is the same as the destination, let's not forget that possibility!!
				if (sourcePtr == destPtr)
					return;

				Unsafe.CopyBlock(destPtr, sourcePtr, (uint) source.Length);
			}
		}

		//READ
		public override void UnMutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset)
		{
			//See: https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/MemoryExtensions.cs
			Reverse(source, sourceOffset, destination, destinationOffset);
		}
	}
}
