using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class PrimitiveArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic generic primitive array serializer
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class PrimitiveArrayTypeSerializerStrategy<T> : BaseArrayTypeSerializerStrategy<PrimitiveArrayTypeSerializerStrategy<T>, T>
		where T : unmanaged
	{
		static PrimitiveArrayTypeSerializerStrategy()
		{
			if (!typeof(T).IsPrimitive)
				throw new InvalidOperationException($"Type {typeof(T).Name} is not a primitive type. Cannot use {nameof(PrimitiveArrayTypeSerializerStrategy<T>)} on non-primitive types.");
		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			//Easier to shift here than later.
			buffer = buffer.Slice(offset);

			int elementSize = MarshalSizeOf<T>.SizeOf;

			// Check this first to avoid div by zero
			if (elementSize == 0)
				return Array.Empty<T>();

			int elementCount = (buffer.Length) / elementSize;

			if (elementCount == 0)
				return Array.Empty<T>();

			//TODO: Can reduce allocations by somehow pooling these things??
			//The new element buffer should be equal to the span length divide by element size
			//conceptually this should fit all deserializable elements here.
			//We then want to PIN the binary chunk in the span and do an unsafe block copy
			//which we must also unsafely pin
			// Ensure buffer is correctly aligned for element size
			int arraySize = buffer.Length / elementSize;
			T[] elementArray = new T[arraySize];
			fixed (byte* bytes = &buffer.GetPinnableReference())
			fixed (void* pinnedArray = &elementArray[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				// GPT Code change: (uint)(elementCount * elementSize) or arraySize * elementSize
				// Unsafe.CopyBlock: The amount of data copied is now (elementCount * elementSize) bytes, which exactly matches the size of the array being populated.
				// This prevents any excess data from being copied, ensuring that only the relevant portion of the buffer is used.
				Unsafe.CopyBlockUnaligned(pinnedArray, bytes, (uint)(arraySize * elementSize));
			}

			offset += elementSize * elementCount;
			return elementArray;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			if (value.Length == 0)
				return;

			//Easier to shift here than later.
			buffer = buffer.Slice(offset);
			int elementSize = MarshalSizeOf<T>.SizeOf;
			int elementsByteSize = elementSize * value.Length;

			if (elementSize == 0)
				return;

			if (buffer.Length < elementsByteSize)
				ThrowBufferTooSmall(buffer.Length, elementsByteSize);

			fixed(byte* bytes = &buffer.GetPinnableReference())
			fixed(void* pinnedArray = &value[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				Unsafe.CopyBlockUnaligned(bytes, pinnedArray, (uint)elementsByteSize);
			}

			offset += elementsByteSize;
		}

		private void ThrowBufferTooSmall(int bufferLength, int elementsByteSize)
		{
			throw new InvalidOperationException($"Buffer too small to write to. Length: {bufferLength} Requested: {elementsByteSize}");
		}
	}
}
