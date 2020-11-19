using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class SendSizeComplexArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic complex/custom array element type serializer.
	/// Includes a length-prefixed size.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TElementSerializerType">The element type serializer.</typeparam>
	/// <typeparam name="TSizeType">The type of the length-perfixed size.</typeparam>
	public sealed class SendSizeComplexArrayTypeSerializerStrategy<TElementSerializerType, T, TSizeType> : BaseArrayTypeSerializerStrategy<ComplexArrayTypeSerializerStrategy<TElementSerializerType, T>, T>
		where T : class //closest to primitive constraints we can get
		where TElementSerializerType : StatelessTypeSerializerStrategy<TElementSerializerType, T>, new()
		where TSizeType : unmanaged
	{
		/// <summary>
		/// The decorated element type serializer.
		/// </summary>
		private static TElementSerializerType DecoratedSerializer { get; } = new TElementSerializerType();

		//WARNING: Don't remove!
		static SendSizeComplexArrayTypeSerializerStrategy()
		{

		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			TSizeType size = GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Read(buffer, ref offset);
			int sizeInt = Unsafe.As<TSizeType, int>(ref size);

			if(sizeInt == 0)
				return System.Array.Empty<T>();

			//TODO: Find a way to reduce allocation. Reuse/pooling!
			T[] collection = new T[sizeInt];

			for(int i = 0; i < sizeInt; i++)
				collection[i] = DecoratedSerializer.Read(buffer, ref offset);

			return collection;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			//Decided to support null as empty/0 size.
			int size = value != null ? value.Length : 0;
			GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Write(Unsafe.As<int, TSizeType>(ref size), buffer, ref offset);

			if(size == 0)
				return;

			for(int i = 0; i < value.Length; i++)
				DecoratedSerializer.Write(value[i], buffer, ref offset);
		}
	}
}
