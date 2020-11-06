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
		: BaseArrayTypeSerializerStrategy<PrimitiveArrayTypeSerializerStrategy<T>, T>
		where T : unmanaged
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		/// <summary>
		/// The fixed size value provider.
		/// </summary>
		private static TStaticTypedSizeValueType FixedSize { get; } = new TStaticTypedSizeValueType();

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> source, ref int offset)
		{
			int size = FixedSize.Value;
			int elementSize = MarshalSizeOf<T>.SizeOf;

			if(elementSize == 0 || size == 0)
				return Array.Empty<T>();

			//We must limit the size so that when we call the basic array serializer
			//it'll know and understand the array size implicitly.
			source = source.Slice(0, offset + elementSize * size);

			return PrimitiveArrayTypeSerializerStrategy<T>.Instance.Read(source, ref offset);
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> destination, ref int offset)
		{
			if(value.Length != FixedSize.Value)
				ThrowNotCorrectSize(value.Length, FixedSize.Value);

			PrimitiveArrayTypeSerializerStrategy<T>.Instance.Write(value, destination, ref offset);
		}

		private static void ThrowNotCorrectSize(int actualSize, int expectedSize)
		{
			throw new InvalidOperationException($"Incorrect ArraySize. Actual: {actualSize} Expected: {expectedSize}");
		}
	}
}
