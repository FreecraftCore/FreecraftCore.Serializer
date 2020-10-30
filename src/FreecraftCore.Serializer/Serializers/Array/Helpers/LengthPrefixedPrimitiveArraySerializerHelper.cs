using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class LengthPrefixedPrimitiveArraySerializerHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TElementType[] Read<TElementType, TSizeType>(Span<byte> source, ref int offset)
			where TElementType : unmanaged
			where TSizeType : unmanaged
		{
			TSizeType size = GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Read(source, ref offset);
			int elementSize = MarshalSizeOf<TElementType>.SizeOf;
			int arrayByteSize = elementSize * Unsafe.As<TSizeType, int>(ref size);

			return PrimitiveArrayTypeSerializerStrategy<TElementType>.Instance.Read(source.Slice(0, offset + arrayByteSize), ref offset);
		}

		//Overload that exists for externally read size
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TElementType[] Read<TElementType, TSizeType>(Span<byte> source, ref int offset, TSizeType size)
			where TElementType : unmanaged
			where TSizeType : unmanaged
		{
			int elementSize = MarshalSizeOf<TElementType>.SizeOf;
			int arrayByteSize = elementSize * Unsafe.As<TSizeType, int>(ref size);

			return PrimitiveArrayTypeSerializerStrategy<TElementType>.Instance.Read(source.Slice(0, offset + arrayByteSize), ref offset);
		}

		//externalize the size so we can support seperate size and
		//also the cases where size is added or subtracted by some value (offset)
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<TElementType, TSizeType>(TElementType[] value, Span<byte> destination, ref int offset, TSizeType size, bool shouldWriteSize)
			where TElementType : unmanaged
			where TSizeType : unmanaged
		{
			//There are cases where SIZE is basically a pointer to somewhere else
			//so we don't *always* write it.
			if (shouldWriteSize)
				GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Write(size, destination, ref offset);

			PrimitiveArrayTypeSerializerStrategy<TElementType>.Instance.Write(value, destination, ref offset);
		}
	}
}
