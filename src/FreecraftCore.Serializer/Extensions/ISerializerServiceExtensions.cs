using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public static class ISerializerServiceExtensions
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the <see cref="bytes"/>.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="bytes">Binary data to read a copy of <typeparamref name="T"/> from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Deserialize<T>([NotNull] this ISerializerService serializer, byte[] bytes)
			where T : ITypeSerializerReadingStrategy<T>
		{
			return serializer.Read<T>(new Span<byte>(bytes), 0);
		}

		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="buffer"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="buffer">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Deserialize<T>([NotNull] this ISerializerService serializer, Span<byte> buffer, int offset)
			where T : ITypeSerializerReadingStrategy<T>
		{
			return serializer.Read<T>(buffer, ref offset);
		}

		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="buffer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Serialize<T>([NotNull] this ISerializerService serializer, T value, Span<byte> buffer, int offset)
			where T : ITypeSerializerWritingStrategy<T>
		{
			serializer.Write<T>(value, buffer, ref offset);
		}

		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="buffer"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="buffer">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Read<T>([NotNull] this ISerializerService serializer, Span<byte> buffer, int offset)
			where T : ITypeSerializerReadingStrategy<T>
		{
			return serializer.Read<T>(buffer, ref offset);
		}

		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="buffer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>([NotNull] this ISerializerService serializer, T value, Span<byte> buffer, int offset)
			where T : ITypeSerializerWritingStrategy<T>
		{
			serializer.Write<T>(value, buffer, ref offset);
		}
	}
}
