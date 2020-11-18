using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	//So it's abit confusing but these are legacy interface types that used to exist and implement
	//the logic for serialization.
	/// <summary>
	/// Implementation of the serialization service.
	/// </summary>
	public interface ISerializerService
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="source"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="source">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		T Read<T>(Span<byte> source, ref int offset)
			where T : IWireMessage<T>, new();

		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="destination"/>.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="destination">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Write<T>(T value, Span<byte> destination, ref int offset)
			where T : IWireMessage<T>;
	}

	public sealed class SerializerService : ISerializerService
	{
		/// <inheritdoc />
		public T Read<T>(Span<byte> source, ref int offset) 
			where T : IWireMessage<T>, new()
		{
			T value = new T();
			value.Read(source, ref offset);
			return value;
		}

		/// <inheritdoc />
		public void Write<T>(T value, Span<byte> destination, ref int offset) 
			where T : IWireMessage<T>
		{
			value.Write(value, destination, ref offset);
		}
	}
}
