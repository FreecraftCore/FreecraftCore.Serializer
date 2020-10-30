using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Generic serialization strategy for generic length type prefixed strings
	/// of generic encoding.
	/// </summary>
	/// <typeparam name="TStringSerializerStrategy">The decorated string serializer type.</typeparam>
	/// <typeparam name="TLengthType"></typeparam>
	public sealed class LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TLengthType> 
		: BaseEncodableTypeSerializerStrategy<LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TLengthType>>
		where TStringSerializerStrategy : BaseStringTypeSerializerStrategy<TStringSerializerStrategy>, IFixedLengthCharacterSerializerStrategy, new() //Includ fixed-length metadata marker.
		where TLengthType : unmanaged 
	{
		private static TStringSerializerStrategy DecoratedSerializer { get; } = new TStringSerializerStrategy();

		public LengthPrefixedStringTypeSerializerStrategy() 
			: base(DecoratedSerializer.EncodingStrategy)
		{
			
		}

		public sealed override string Read(Span<byte> source, ref int offset)
		{
			//This is complicated, we generically deserialize the primitive length BUT we run into the issue
			//where we NEED a true non-generic primitive to add (without generic math dependency) so we must
			//then do an Unsafe cast. We cannot realistically have a buffer bigger than 2 billion bytes anyway, no matter how it's
			//encoded so this is probably safe to do.
			TLengthType length = GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Read(source, ref offset);
			int stringLength = Unsafe.As<TLengthType, int>(ref length);

			//Easier to slice towards the end, and keep the offset, than to manage ref offset separate.
			return DecoratedSerializer.Read(source.Slice(0, stringLength + offset), ref offset);
		}

		public sealed override void Write(string value, Span<byte> destination, ref int offset)
		{
			//We must write the length prefix first into the buffer
			int stringLength = value.Length;
			GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Write(Unsafe.As<int, TLengthType>(ref stringLength), destination, ref offset);

			DecoratedSerializer.Write(value, destination, ref offset);
		}
	}
}
