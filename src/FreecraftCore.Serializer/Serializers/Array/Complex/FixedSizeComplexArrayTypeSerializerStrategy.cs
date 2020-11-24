using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class FixedSizeComplexArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic complex/custom array element type serializer.
	/// Includes a fixed/known statically typed size generic parameter.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TElementSerializerType">The element type serializer.</typeparam>
	/// <typeparam name="TStaticTypedSizeValueType">The statically typed numeric type provider.</typeparam>
	public sealed class FixedSizeComplexArrayTypeSerializerStrategy<TElementSerializerType, T, TStaticTypedSizeValueType> 
		: BaseArrayTypeSerializerStrategy<FixedSizeComplexArrayTypeSerializerStrategy<TElementSerializerType, T, TStaticTypedSizeValueType>, T>
		where T : class
		where TElementSerializerType : StatelessTypeSerializerStrategy<TElementSerializerType, T>, new()
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		/// <summary>
		/// The decorated element type serializer.
		/// </summary>
		private static TElementSerializerType DecoratedSerializer { get; } = new TElementSerializerType();

		/// <summary>
		/// The fixed size value provider.
		/// </summary>
		private static TStaticTypedSizeValueType FixedSize { get; } = new TStaticTypedSizeValueType();

		//WARNING: Don't remove!
		static FixedSizeComplexArrayTypeSerializerStrategy()
		{

		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			if(FixedSize.Value == 0)
				return Array.Empty<T>();

			//TODO: Find a way to reduce allocation. Reuse/pooling!
			T[] collection = new T[FixedSize.Value];

			for(int i = 0; i < FixedSize.Value; i++)
				collection[i] = DecoratedSerializer.Read(buffer, ref offset);

			return collection;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			if (value.Length != FixedSize.Value)
				ThrowNotCorrectSize(value.Length, FixedSize.Value);

			for (int i = 0; i < FixedSize.Value; i++)
				DecoratedSerializer.Write(value[i], buffer, ref offset);
		}

		private static void ThrowNotCorrectSize(int actualSize, int expectedSize)
		{
			throw new InvalidOperationException($"Incorrect ArraySize. Actual: {actualSize} Expected: {expectedSize}");
		}
	}
}
