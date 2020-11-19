using System;
using System.Runtime.CompilerServices;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Implementation of the serialization service.
	/// </summary>
	public interface ISerializerService
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="buffer"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="buffer">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		T Read<T>(Span<byte> buffer, ref int offset)
			where T : IWireMessage<T>;

		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="buffer"/>.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Write<T>(T value, Span<byte> buffer, ref int offset)
			where T : IWireMessage<T>;
	}
}