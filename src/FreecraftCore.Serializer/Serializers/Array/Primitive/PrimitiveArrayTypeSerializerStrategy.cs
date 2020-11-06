using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class PrimitiveArrayTypeSerializerStrategy
	{

	}

	/// <summary>
	/// Contract for generic generic primitive array serializer
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TSizeType"></typeparam>
	public sealed class PrimitiveArrayTypeSerializerStrategy<T, TSizeType> : BaseArrayTypeSerializerStrategy<PrimitiveArrayTypeSerializerStrategy<T>, T>
		where T : unmanaged
		where TSizeType : unmanaged
	{
		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> source, ref int offset)
		{
			//Easier to shift here than later.
			source = source.Slice(offset);

			TSizeType size = GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Read(source, ref offset);
			int sizeInt = Unsafe.As<TSizeType, int>(ref size);
			int elementSize = MarshalSizeOf<T>.SizeOf;

			if(elementSize == 0 || sizeInt == 0)
				return Array.Empty<T>();

			//Slice it so that it's the entire buffer
			source = source.Slice(0, elementSize * sizeInt);

			//TODO: Can reduce allocations by somehow pooling these things??
			//The new element buffer should be equal to the span length divide by element size
			//conceptually this should fit all deserializable elements here.
			//We then want to PIN the binary chunk in the span and do an unsafe block copy
			//which we must also unsafely pin
			T[] elementArray = new T[source.Length / elementSize];
			fixed(byte* bytes = &source.GetPinnableReference())
			fixed(void* pinnedArray = &elementArray[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				Unsafe.CopyBlock(pinnedArray, bytes, (uint) source.Length);
			}

			offset += source.Length;
			return elementArray;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> destination, ref int offset)
		{
			//Size needs writing even if 0
			int valueLength = value.Length;
			GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Write(Unsafe.As<int, TSizeType>(ref valueLength), destination, ref offset);

			if(value.Length == 0)
				return;

			//Easier to shift here than later.
			//It's ok that we used this buffer AND offset above first to write into the size, don't move this.
			destination = destination.Slice(offset);
			int elementSize = MarshalSizeOf<T>.SizeOf;
			int elementsByteSize = elementSize * value.Length;

			if(elementSize == 0)
				return;

			fixed(byte* bytes = &destination.GetPinnableReference())
			fixed(void* pinnedArray = &value[0]) //This pin is VERY important, otherwise GC could maybe move it.
			{
				Unsafe.CopyBlock(bytes, pinnedArray, (uint)elementsByteSize);
			}

			offset += elementSize * value.Length;
		}
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
