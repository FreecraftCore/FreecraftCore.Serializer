using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serializer type for terminated strings.
	/// </summary>
	/// <typeparam name="TStringSerializerType">The serializer type.</typeparam>
	/// <typeparam name="TStringTerminatorSerializerType">The null terminator type</typeparam>
	[KnownTypeSerializer]
	public sealed class TerminatedStringTypeSerializerStrategy<TStringSerializerType, TStringTerminatorSerializerType> : BaseStringTypeSerializerStrategy<TStringSerializerType>
		where TStringSerializerType : BaseStringTypeSerializerStrategy<TStringSerializerType>, new()
		where TStringTerminatorSerializerType : BaseStringTerminatorSerializerStrategy<TStringTerminatorSerializerType>, new()
	{
		private static TStringTerminatorSerializerType TerminatorSerializer { get; } = new TStringTerminatorSerializerType();

		//Don't remove
		static TerminatedStringTypeSerializerStrategy()
		{
			
		}

		public TerminatedStringTypeSerializerStrategy()
			: base(TerminatorSerializer.EncodingStrategy)
		{

		}

		public sealed override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			string value = base.Read(buffer, ref offset);
			TerminatorSerializer.Read(buffer, ref offset);
			return value;
		}

		public sealed override unsafe void Write(string value, Span<byte> buffer, ref int offset)
		{
			base.Write(value, buffer, ref offset);
			TerminatorSerializer.Write(value, buffer, ref offset);
		}
	}
}
