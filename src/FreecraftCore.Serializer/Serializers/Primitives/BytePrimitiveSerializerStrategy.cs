using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	//TODO: Cannot support Char due to encoding differences.
	/// <summary>
	/// Simple byte type serializer implementation of <see cref="ITypeSerializerStrategy{T}"/>
	/// </summary>
	public sealed class BytePrimitiveSerializerStrategy : StatelessTypeSerializerStrategy<BytePrimitiveSerializerStrategy, byte>
	{
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override byte Read(Span<byte> buffer, ref int offset)
		{
			byte value = buffer[offset];
			offset++;
			return value;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(byte value, Span<byte> buffer, ref int offset)
		{
			buffer[offset] = value;
			offset++;
		}
	}
}
