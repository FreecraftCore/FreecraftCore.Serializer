using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class FixedSizePrimitiveArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic generic primitive array serializer
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TStaticTypedSizeValueType"></typeparam>
	public sealed class FixedSizePrimitiveArrayTypeSerializerStrategy<T, TStaticTypedSizeValueType> 
		: BaseArrayTypeSerializerStrategy<FixedSizePrimitiveArrayTypeSerializerStrategy<T, TStaticTypedSizeValueType>, T>
		where T : unmanaged
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		/// <summary>
		/// The fixed size value provider.
		/// </summary>
		private static TStaticTypedSizeValueType FixedSize { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new TStaticTypedSizeValueType();

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			int size = FixedSize.Value;
			int elementSize = MarshalSizeOf<T>.SizeOf;

			if(elementSize == 0 || size == 0)
				return Array.Empty<T>();

			//We must limit the size so that when we call the basic array serializer
			//it'll know and understand the array size implicitly.
			buffer = buffer.Slice(0, offset + elementSize * size);

			return PrimitiveArrayTypeSerializerStrategy<T>.Instance.Read(buffer, ref offset);
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			//Special handling for primitive null arrays.
			//write the full size of defaults
			//It's a useful optimization for no allocation.
			if (value == null || value.Length == 0 && FixedSize.Value != 0)
			{
				WriteEmptyArray(buffer, ref offset);
				return;
			}

			if(value.Length != FixedSize.Value)
				ThrowNotCorrectSize(value.Length, FixedSize.Value);

			PrimitiveArrayTypeSerializerStrategy<T>.Instance.Write(value, buffer, ref offset);
		}

		private static void WriteEmptyArray(Span<byte> buffer, ref int offset)
		{
			int elementsByteSize = MarshalSizeOf<T>.SizeOf * FixedSize.Value;

			//TODO: This is leaking how we write arrays into this decorator.
			//We could ArrayPool, initialize and then write but we opt for no allocation
			//We assume default is block of 0 values but this could be wrong, but right in
			//all primitive cases.
			for (int i = 0; i < elementsByteSize; i++)
				buffer[offset + i] = 0;

			offset += elementsByteSize;
		}

		private static void ThrowNotCorrectSize(int actualSize, int expectedSize)
		{
			throw new InvalidOperationException($"Incorrect ArraySize. Actual: {actualSize} Expected: {expectedSize}");
		}
	}
}
