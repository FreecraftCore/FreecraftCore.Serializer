using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Simple sbyte type serializer implementation of <see cref="ITypeSerializerStrategy{T}"/>
	/// </summary>
	public sealed class SBytePrimitiveSerializerStrategy 
		: StatelessTypeSerializerStrategy<SBytePrimitiveSerializerStrategy, sbyte>
	{
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override sbyte Read(Span<byte> buffer, ref int offset)
		{
			sbyte value = buffer.Reinterpret<sbyte>(offset);
			offset++;
			return value;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(sbyte value, Span<byte> buffer, ref int offset)
		{
			value.Reinterpret(buffer, offset);
			offset++;
		}
	}
}
