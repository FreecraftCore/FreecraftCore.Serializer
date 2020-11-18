using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	public static class ITypeSerializerStrategyExtensions
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="source"/>
		/// starting from <paramref name="offset"/>
		/// Specialized extension that won't move the offset forward.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="source">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Read<T>(this ITypeSerializerStrategy<T> serializer, Span<byte> source, int offset)
		{
			return serializer.Read(source, ref offset);
		}

		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="destination"/>.
		/// Specialized extension that won't move the offset forward.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="value">The value to write.</param>
		/// <param name="destination">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>(this ITypeSerializerStrategy<T> serializer, T value, Span<byte> destination, int offset)
		{
			serializer.Write(value, destination, ref offset);
		}
	}
}
