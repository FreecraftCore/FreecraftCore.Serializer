using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
		where TEnumType : struct, Enum
		where TEncodedPrimitiveType : unmanaged
	{
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override TEnumType Read(Span<byte> source, ref int offset)
		{
			//Assume that the "decorated" primitive serializer handles offset
			TEncodedPrimitiveType value = GenericTypePrimitiveSerializerStrategy<TEncodedPrimitiveType>.Instance.Read(source, ref offset);
			return Unsafe.As<TEncodedPrimitiveType, TEnumType>(ref value);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(TEnumType value, Span<byte> destination, ref int offset)
		{
			//Assume that the "decorated" primitive serializer handles offset
			TEncodedPrimitiveType primitiveValue = Unsafe.As<TEnumType, TEncodedPrimitiveType>(ref value);
			GenericTypePrimitiveSerializerStrategy<TEncodedPrimitiveType>.Instance.Write(primitiveValue, destination, ref offset);
		}
	}
}
