using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serializer strategy for <see cref="Enum"/> serialization that maps enumeration types to an underlying
	/// primitive (Ex. Int32) type.
	/// </summary>
	/// <typeparam name="TEnumType">Enumeration type to convert between.</typeparam>
	/// <typeparam name="TEncodedPrimitiveType">Primitive to encode the enumeration value as.</typeparam>
	public sealed class GenericPrimitiveEnumTypeSerializerStrategy<TEnumType, TEncodedPrimitiveType> 
		: BaseEnumTypeSerializerStrategy<GenericPrimitiveEnumTypeSerializerStrategy<TEnumType, TEncodedPrimitiveType>, TEnumType> 
		where TEnumType : unmanaged, Enum
		where TEncodedPrimitiveType : unmanaged
	{
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override TEnumType Read(Span<byte> buffer, ref int offset)
		{
			//Assume that the "decorated" primitive serializer handles offset
			TEncodedPrimitiveType value = GenericTypePrimitiveSerializerStrategy<TEncodedPrimitiveType>.Instance.Read(buffer, ref offset);
			return value.Reinterpret<TEncodedPrimitiveType, TEnumType>();
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(TEnumType value, Span<byte> buffer, ref int offset)
		{
			//Assume that the "decorated" primitive serializer handles offset
			TEncodedPrimitiveType primitiveValue = value.Reinterpret<TEnumType, TEncodedPrimitiveType>();
			GenericTypePrimitiveSerializerStrategy<TEncodedPrimitiveType>.Instance.Write(primitiveValue, buffer, ref offset);
		}
	}
}
