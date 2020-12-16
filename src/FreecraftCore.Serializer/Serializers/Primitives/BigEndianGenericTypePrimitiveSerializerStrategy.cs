using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	//TODO: Cannot support Char due to encoding differences.
	/// <summary>
	/// Contract for type serializer that is a primitive and generic and read/written Big Endian (reversed from default .NET endianness).
	/// Big Endian implementation of <see cref="GenericTypePrimitiveSerializerStrategy{TType}"/>
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	public sealed class BigEndianGenericTypePrimitiveSerializerStrategy<TType> : StatelessTypeSerializerStrategy<BigEndianGenericTypePrimitiveSerializerStrategy<TType>, TType>
		where TType : struct
	{
		static BigEndianGenericTypePrimitiveSerializerStrategy()
		{
			if(!typeof(TType).IsPrimitive)
				throw new InvalidOperationException($"Cannot use {nameof(GenericTypePrimitiveSerializerStrategy<TType>)} for Type: {typeof(TType).Name} as it's not a primitive type.");
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override TType Read(Span<byte> buffer, ref int offset)
		{
			//This is a pretty efficient stack allocation way to reverse a chunk
			//of the span without modifying, even temporarily, the original buffer.
			Span<byte> tempSpan = stackalloc byte[MarshalSizeOf<TType>.SizeOf];
			buffer.Slice(offset, MarshalSizeOf<TType>.SizeOf)
				.CopyTo(tempSpan);
			tempSpan.Reverse();

			offset += MarshalSizeOf<TType>.SizeOf;
			return GenericTypePrimitiveSerializerStrategy<TType>.Instance.Read(tempSpan, 0);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(TType value, Span<byte> buffer, ref int offset)
		{
			//This is a pretty efficient stack allocation way to reverse a chunk
			//of the span without modifying, even temporarily, the original buffer.
			Span<byte> tempSpan = stackalloc byte[MarshalSizeOf<TType>.SizeOf];
			GenericTypePrimitiveSerializerStrategy<TType>.Instance.Write(value, tempSpan, 0);

			//Push the temp reversed bytes into the existing buffer
			tempSpan.Reverse();
			tempSpan.CopyTo(buffer.Slice(offset, MarshalSizeOf<TType>.SizeOf));
			offset += MarshalSizeOf<TType>.SizeOf;
		}
	}
}