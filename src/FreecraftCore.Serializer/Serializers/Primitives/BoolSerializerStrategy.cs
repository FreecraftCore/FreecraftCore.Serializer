using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Known-type serializer for the <see cref="bool"/> value-type.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class BoolSerializerStrategy : StatelessTypeSerializerStrategy<BoolSerializerStrategy, bool>
	{
		public BoolSerializerStrategy()
		{
		}

		/// <summary>
		/// Converts a byte to the boolean representation.
		/// (based on Trinitycore's ByteBuffer.h)
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool ConvertFromByte(byte b)
		{
			return b > 0;
		}

		/// <summary>
		/// Converts a boolean value to the byte representation.
		/// (based on Trinitycore's ByteBuffer.h)
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private byte ConvertFromBool(bool b)
		{
			return (byte) (b ? 1 : 0);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Read(Span<byte> source, ref int offset)
		{
			bool value = ConvertFromByte(source[offset]);
			offset++;
			return value;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(bool value, Span<byte> destination, ref int offset)
		{
			destination[offset] = ConvertFromBool(value);
			offset++;
		}
	}
}
