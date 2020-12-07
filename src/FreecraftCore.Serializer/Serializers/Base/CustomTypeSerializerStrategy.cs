using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// This is the base type for any external custom serializers.
	/// Allowing users to hand-write serialization for specific members or types.
	/// </summary>
	/// <typeparam name="TChildType">The child serializer type. Should be the derived type of the serializer.</typeparam>
	/// <typeparam name="T">The type to be serialized.</typeparam>
	public abstract class CustomTypeSerializerStrategy<TChildType, T> 
		: StatelessTypeSerializerStrategy<TChildType, T>, ITargetedTypeSerializerStrategy<T>
		where TChildType : StatelessTypeSerializerStrategy<TChildType, T>, new()
		where T : new()
	{
		/// <summary>
		/// Auto-generated deserialization/read code.
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		/// <returns>Deserialized instance of the requested Type.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override T Read(Span<byte> buffer, ref int offset)
		{
			//This dispatches to the partial private method implemented in a simplier Roslyn generated form.
			T value = new T();
			InternalRead(value, buffer, ref offset);
			return value;
		}

		//Cannot do internal protected because we must reference it in generated WireMessages
		//Partial externally implemented deserialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void InternalRead(T value, Span<byte> buffer, ref int offset);

		/// <summary>
		/// Auto-generated serialization/write code.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write to.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(T value, Span<byte> buffer, ref int offset)
		{
			//This dispatches to the partial private method implemented in a simplier Roslyn generated form.
			InternalWrite(value, buffer, ref offset);
		}

		//Cannot do internal protected because we must reference it in generated WireMessages
		//Partial externally implemented serialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void InternalWrite(T value, Span<byte> buffer, ref int offset);
	}
}
