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
	/// Contract for type serializer that is a primitive and generic.
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	public sealed class GenericTypePrimitiveSerializerStrategy<TType> : StatelessTypeSerializerStrategy<GenericTypePrimitiveSerializerStrategy<TType>, TType>
		where TType : struct
	{
		static GenericTypePrimitiveSerializerStrategy()
		{
			if (!typeof(TType).IsPrimitive)
				throw new InvalidOperationException($"Cannot use {nameof(GenericTypePrimitiveSerializerStrategy<TType>)} for Type: {typeof(TType).Name} as it's not a primitive type.");
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override TType Read(Span<byte> buffer, ref int offset)
		{
			//Don't want to read outside the bounds
			if(buffer.Length - offset < MarshalSizeOf<TType>.SizeOf)
				ThrowBufferTooSmall();

			TType value = Unsafe.ReadUnaligned<TType>(ref buffer[offset]);
			offset += MarshalSizeOf<TType>.SizeOf;
			return value;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(TType value, Span<byte> buffer, ref int offset)
		{
			//Don't want to write outside the bounds
			if ((buffer.Length - offset) < MarshalSizeOf<TType>.SizeOf)
				ThrowBufferTooSmall();

			// GPT code update for portability
			// On platforms where unaligned access is not allowed, the old Unsafe.As code may have
			// potentially cause undefined behavior, including crashes.
			Unsafe.WriteUnaligned(ref buffer[offset], value);
			offset += MarshalSizeOf<TType>.SizeOf;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowBufferTooSmall()
		{
			throw new InvalidOperationException($"Buffer too small to write Type: {typeof(TType).Name}");
		}
	}
}
