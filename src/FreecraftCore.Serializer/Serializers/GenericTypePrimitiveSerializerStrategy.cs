﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for type serializer that is a primitive and generic.
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	public sealed class GenericTypePrimitiveSerializerStrategy<TType> : StatelessTypeSerializer<GenericTypePrimitiveSerializerStrategy<TType>, TType>
		where TType : struct
	{
		static GenericTypePrimitiveSerializerStrategy()
		{
			if (!typeof(TType).IsPrimitive)
				throw new InvalidOperationException($"Cannot use {nameof(GenericTypePrimitiveSerializerStrategy<TType>)} for Type: {typeof(TType).Name} as it's not a primitive type.");
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override TType Read(Span<byte> source, int offset)
		{
			return Unsafe.ReadUnaligned<TType>(ref source[offset]);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(TType value, Span<byte> destination, int offset)
		{
			Unsafe.As<byte, TType>(ref destination[offset]) = value;
		}
	}
}