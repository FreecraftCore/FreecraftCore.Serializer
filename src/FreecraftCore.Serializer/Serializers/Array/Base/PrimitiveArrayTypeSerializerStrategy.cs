using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class PrimitiveArrayTypeSerializerStrategy<T> : BaseArrayTypeSerializerStrategy<PrimitiveArrayTypeSerializerStrategy<T>, T>
		where T : unmanaged //closest to primitive constraints we can get
	{
		static PrimitiveArrayTypeSerializerStrategy()
		{
			if (!typeof(T).IsPrimitive)
				throw new InvalidOperationException($"Type {typeof(T).Name} is not a primitive type. Cannot use {nameof(PrimitiveArrayTypeSerializerStrategy<T>)} on non-primitive types.");
		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> source, ref int offset)
		{
			//Easier to shift here than later.
			source = source.Slice(offset);

			int elementSize = MarshalSizeOf<T>.SizeOf;
			int elementCount = (source.Length) / elementSize;

			if (elementSize == 0 || elementCount == 0)
				return Array.Empty<T>();

			//TODO: Can reduce allocations by somehow pooling these things??
			//The new element buffer should be equal to the span length divide by element size
			//conceptually this should fit all deserializable elements here.
			//We then want to PIN the binary chunk in the span and do an unsafe block copy
			//which we must also unsafely pin
			T[] elementArray = new T[source.Length / elementSize];
			fixed (byte* bytes = &source.GetPinnableReference())
			fixed (void* pinnedArray = &elementArray[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				Unsafe.CopyBlock(pinnedArray, bytes, (uint) source.Length);
			}

			offset += elementSize * elementCount;
			return elementArray;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> destination, ref int offset)
		{
			if (value.Length == 0)
				return;

			//Easier to shift here than later.
			destination = destination.Slice(offset);
			int elementSize = MarshalSizeOf<T>.SizeOf;
			int elementsByteSize = elementSize * value.Length;

			if (elementSize == 0)
				return;

			fixed(byte* bytes = &destination.GetPinnableReference())
			fixed(void* pinnedArray = &value[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				Unsafe.CopyBlock(bytes, pinnedArray, (uint)elementsByteSize);
			}

			offset += elementSize * value.Length;
		}
	}
}
