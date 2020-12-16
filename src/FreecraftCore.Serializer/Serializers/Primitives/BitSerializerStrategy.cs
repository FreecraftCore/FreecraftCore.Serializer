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
	/// Known-type serializer for the Bit sized primitive type.
	/// See: <see cref="PrimitiveSizeType"/>.
	/// Specialized <see cref="byte"/> serializer that does not consume any offset.
	/// Meaning it just inspects the first bit and returns a <see cref="byte"/> value.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class BitSerializerStrategy : StatelessTypeSerializerStrategy<BitSerializerStrategy, byte>
	{
		public BitSerializerStrategy()
		{

		}

		/// <inheritdoc />
		public override byte Read(Span<byte> buffer, ref int offset)
		{
			//Idea here is we want to read the first bit in the stream (ignoring endian-ness)
			//and we consider the highest bit to be the first big in the stream. So basically imagine big endian for Bit serialization.
			//Therefore we must get the value of the top most bit and shift.
			return (byte) ((buffer[offset] & 0x80) > 0 ? 1 : 0); //0x80 is top level/MSB bit of 0xFF hex
		}

		/// <inheritdoc />
		public override void Write(byte value, Span<byte> buffer, ref int offset)
		{
			//Writing only the first bit in the value and not incrementing at all.
			//This is a specialized serializer and shouldn't really be used for anything other than
			//some legacy polymoprhic serialization.
			buffer[offset] = (byte) (value & 0x01);
		}
	}
}
