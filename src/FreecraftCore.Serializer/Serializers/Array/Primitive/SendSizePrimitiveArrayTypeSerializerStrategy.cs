using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class SendSizePrimitiveArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic generic primitive array serializer
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TSizeType"></typeparam>
	public sealed class SendSizePrimitiveArrayTypeSerializerStrategy<T, TSizeType> : BaseArrayTypeSerializerStrategy<PrimitiveArrayTypeSerializerStrategy<T>, T>
		where T : unmanaged
		where TSizeType : unmanaged
	{
		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			TSizeType size = GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Read(buffer, ref offset);
			int sizeInt = Unsafe.As<TSizeType, int>(ref size);
			int elementSize = MarshalSizeOf<T>.SizeOf;

			if(elementSize == 0 || sizeInt == 0)
				return Array.Empty<T>();

			//We must limit the size so that when we call the basic array serializer
			//it'll know and understand the array size implicitly.
			buffer = buffer.Slice(0, offset + elementSize * sizeInt);

			return PrimitiveArrayTypeSerializerStrategy<T>.Instance.Read(buffer, ref offset);
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			//Size needs writing even if 0
			int valueLength = value.Length;
			GenericTypePrimitiveSerializerStrategy<TSizeType>.Instance.Write(Unsafe.As<int, TSizeType>(ref valueLength), buffer, ref offset);

			if(value.Length == 0)
				return;

			PrimitiveArrayTypeSerializerStrategy<T>.Instance.Write(value, buffer, ref offset);
		}
	}
}
