using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for generic complex/custom array element type serializer.
	/// Includes a length-prefixed size.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TElementSerializerType">The element type serializer.</typeparam>
	/// <typeparam name="TSizeType">The type of the length-perfixed size.</typeparam>
	public sealed class ComplexArrayTypeSerializerStrategy<TElementSerializerType, T, TSizeType> : BaseArrayTypeSerializerStrategy<ComplexArrayTypeSerializerStrategy<TElementSerializerType, T>, T>
		where T : class //closest to primitive constraints we can get
		where TElementSerializerType : StatelessTypeSerializerStrategy<TElementSerializerType, T>, new()
		where TSizeType : unmanaged
	{
		/// <summary>
		/// The decorated element type serializer.
		/// </summary>
		private static TElementSerializerType DecoratedSerializer { get; } = new TElementSerializerType();

		//WARNING: Don't remove!
		static ComplexArrayTypeSerializerStrategy()
		{

		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> source, ref int offset)
		{
			TSizeType size = GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Read(source, ref offset);
			int sizeInt = Unsafe.As<TSizeType, int>(ref size);

			if (sizeInt == 0)
				return Array.Empty<T>();

			//TODO: Find a way to reduce allocation. Reuse/pooling!
			T[] collection = new T[sizeInt];

			for (int i = 0; i < sizeInt; i++)
				collection[i] = DecoratedSerializer.Read(source, ref offset);

			return collection;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> destination, ref int offset)
		{
			//Decided to support null as empty/0 size.
			int size = value != null ? value.Length : 0;
			GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Write(Unsafe.As<int, TSizeType>(ref size), destination, ref offset);

			if(size == 0)
				return;

			for(int i = 0; i < value.Length; i++)
				DecoratedSerializer.Write(value[i], destination, ref offset);
		}
	}

	/// <summary>
	/// Contract for generic complex/custom array element type serializer.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TElementSerializerType">The element type serializer.</typeparam>
	public sealed class ComplexArrayTypeSerializerStrategy<TElementSerializerType, T> : BaseArrayTypeSerializerStrategy<ComplexArrayTypeSerializerStrategy<TElementSerializerType, T>, T>
		where T : class //closest to primitive constraints we can get
		where TElementSerializerType : StatelessTypeSerializerStrategy<TElementSerializerType, T>, new()
	{
		/// <summary>
		/// The decorated element type serializer.
		/// </summary>
		private static TElementSerializerType DecoratedSerializer { get; } = new TElementSerializerType();

		static ComplexArrayTypeSerializerStrategy()
		{
			
		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> source, ref int offset)
		{
			//This is SUPER inefficient, causing MANY allocations and a BIG allocation and copy at the end
			//This should ONLY ever be used if we absolutely do not and cannot know how many elements there are.
			List<T> tempList = new List<T>();

			while(offset < source.Length)
				tempList.Add(DecoratedSerializer.Read(source, ref offset));

			return tempList.ToArray();
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> destination, ref int offset)
		{
			if (value.Length == 0)
				return;

			for (int i = 0; i < value.Length; i++)
				DecoratedSerializer.Write(value[i], destination, ref offset);
		}
	}
}
