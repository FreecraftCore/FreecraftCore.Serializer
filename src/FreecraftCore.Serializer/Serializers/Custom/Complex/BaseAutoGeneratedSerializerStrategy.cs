﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's base-type serializer for AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <typeparamref name="T"/>
	/// </summary>
	public abstract class BaseAutoGeneratedSerializerStrategy<TChildType, T> : StatelessTypeSerializerStrategy<TChildType, T> 
		where TChildType : StatelessTypeSerializerStrategy<TChildType, T>, new()
		where T : new()
	{
		/// <summary>
		/// Auto-generated deserialization/read code.
		/// </summary>
		/// <param name="source">The buffer to read from.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		/// <returns>Deserialized instance of the requested Type.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override T Read(Span<byte> source, ref int offset)
		{
			//This dispatches to the partial private method implemented in a simplier Roslyn generated form.
			T value = new T();
			InternalRead(value, source, ref offset);
			return value;
		}

		//Partial externally implemented deserialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal abstract void InternalRead(T value, Span<byte> source, ref int offset);

		/// <summary>
		/// Auto-generated serialization/write code.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="destination">The destination buffer to write to.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(T value, Span<byte> destination, ref int offset)
		{
			//This dispatches to the partial private method implemented in a simplier Roslyn generated form.
			InternalWrite(value, destination, ref offset);
		}

		//Partial externally implemented serialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal abstract void InternalWrite(T value, Span<byte> destination, ref int offset);
	}
}
