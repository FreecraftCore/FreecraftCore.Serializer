using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class IStringTypeSerializerStrategyExtensions
	{
		//We need this to aid in easily calling string serializers with enu,
		/// <summary>
		/// Writes a copy of <typeparamref name="TEnumValue"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="buffer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<TEnumValue>(this IStringTypeSerializerStrategy serializer, TEnumValue value, Span<byte> buffer, ref int offset)
			where TEnumValue : Enum
		{
			//For inlinine purposes, don't check args
			//TODO: this is slow.
			serializer.Write(value.ToString(), buffer, ref offset);
		}
	}
}
