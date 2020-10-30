using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//Originally TStringSerializerStrategy was constrained to IFixedLengthCharacterSerializerStrategy but I decided to support
	//the hacky UTF16 of PSOBB which is fixed-length. We should probably WARN consumers of the library that this is invalid to the specification.
	//Also should be warned that the default behavior READ behavior of the 
	/// <summary>
	/// Generic serialization strategy for generic length type prefixed strings
	/// of generic encoding.
	/// </summary>
	/// <typeparam name="TStringSerializerStrategy">The decorated string serializer type.</typeparam>
	/// <typeparam name="TLengthType"></typeparam>
	public class LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TLengthType> 
		: BaseEncodableTypeSerializerStrategy<LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TLengthType>>
		where TStringSerializerStrategy : BaseStringTypeSerializerStrategy<TStringSerializerStrategy>, new()
		where TLengthType : unmanaged 
	{
		//Pointless allocation but C# doesn't provide a way to access static members of generic types yet.
		private static TStringSerializerStrategy DecoratedSerializer { get; } = new TStringSerializerStrategy();

		/// <summary>
		/// The maximum length of a character in byte-size.
		/// Same as CharacterSize except for the variable length encodings which we *partially* support
		/// fixed-length sending.
		/// </summary>
		private static int MaximumCharacterSize { get; } = DecoratedSerializer.EncodingStrategy.GetMaxByteCount(1);

		public LengthPrefixedStringTypeSerializerStrategy() 
			: base(DecoratedSerializer.EncodingStrategy)
		{
			
		}

		public sealed override string Read(Span<byte> source, ref int offset)
		{
			int length = CalculateIncomingStringLength(source, ref offset);
			int expectedByteLength = length * MaximumCharacterSize;

			//Even though some encodings are VARIABLE LENGTH we assume when encoding these we'll send max character size.
			//Easier to slice towards the end, and keep the offset, than to manage ref offset separate.
			int lastOffset = offset;
			string value = DecoratedSerializer.Read(source.Slice(0, expectedByteLength + offset), ref offset);

			//We must override offset because we EXPECTED to read this many. This helps with nullterminator issues
			//in existing projects that assume KnownSize can INCLUDE the nullterminator.
			offset = lastOffset + expectedByteLength;
			return value;
		}

		protected virtual int CalculateIncomingStringLength(Span<byte> source, ref int offset)
		{
			//This is complicated, we generically deserialize the primitive length BUT we run into the issue
			//where we NEED a true non-generic primitive to add (without generic math dependency) so we must
			//then do an Unsafe cast. We cannot realistically have a buffer bigger than 2 billion bytes anyway, no matter how it's
			//encoded so this is probably safe to do.
			TLengthType length = GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Read(source, ref offset);
			int stringLength = Unsafe.As<TLengthType, int>(ref length);
			return stringLength;
		}

		public sealed override void Write(string value, Span<byte> destination, ref int offset)
		{
			//WARNING: Doing use this calculated string length in ANYTHING but writing, it may contain adjusted sizes.
			//We must write the length prefix first into the buffer
			int stringLength = CalculateOutgoingStringLength(value);
			GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Write(Unsafe.As<int, TLengthType>(ref stringLength), destination, ref offset);

			int expectedByteLength = value.Length * MaximumCharacterSize;
			int lastOffset = offset;
			DecoratedSerializer.Write(value, destination, ref offset);

			//TODO: This is a COMPLETE hack that should be toggleable honestly.
			//This isn't *really* how we should handle variable length encodings and stuff, but PSOBB does fixed-length UTF16 for fixed/known size
			//So to compensate for this we adjust the buffer offset to pretend we're fixed-length
			if (offset != lastOffset + expectedByteLength)
				while(offset < lastOffset + expectedByteLength)
					GenericTypePrimitiveSerializerStrategy<byte>.Instance.Write(0, destination, ref offset);
		}

		private static int CalculateOutgoingStringLength(string value)
		{
			int stringLength = value.Length;
			return stringLength;
		}
	}
}
