using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Reinterpret.Net;

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
	/// <typeparam name="TStringTerminatorSerializerStrategy"></typeparam>
	public sealed class LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TStringTerminatorSerializerStrategy, TLengthType> 
		: BaseEncodableTypeSerializerStrategy<LengthPrefixedStringTypeSerializerStrategy<TStringSerializerStrategy, TStringTerminatorSerializerStrategy, TLengthType>>
		where TStringSerializerStrategy : BaseStringTypeSerializerStrategy<TStringSerializerStrategy>, new()
		where TStringTerminatorSerializerStrategy : BaseStringTerminatorSerializerStrategy<TStringTerminatorSerializerStrategy>, new()
		where TLengthType : unmanaged 
	{
		//Pointless allocation but C# doesn't provide a way to access static members of generic types yet.
		private static TStringSerializerStrategy DecoratedSerializer { get; } = new TStringSerializerStrategy();

		private static TStringTerminatorSerializerStrategy DecoratedTerminatorStrategy { get; } = new TStringTerminatorSerializerStrategy();

		/// <summary>
		/// The maximum length of a character in byte-size.
		/// Same as CharacterSize except for the variable length encodings which we *partially* support
		/// fixed-length sending.
		/// </summary>
		private static int MaximumCharacterSize { get; } = DecoratedSerializer.CharacterSize;

		public LengthPrefixedStringTypeSerializerStrategy() 
			: base(DecoratedSerializer.EncodingStrategy)
		{
			
		}

		public sealed override string Read(Span<byte> buffer, ref int offset)
		{
			int length = CalculateIncomingStringLength(buffer, ref offset);

			if(length == 0)
				return String.Empty;

			//Null terminator is missing??
			if (length < MaximumCharacterSize)
			{
				DecoratedTerminatorStrategy.Read(buffer, ref offset);
				return String.Empty;
			}

			//Read until terminator is found, then we skip over terminator in the buffer.
			//Slice just incase invalid data and terminator isn't there.
			string value = DecoratedSerializer.Read(buffer.Slice(0, (length - 1) * MaximumCharacterSize + offset), ref offset);
			DecoratedTerminatorStrategy.Read(buffer, ref offset);

			return value;
		}

		private int CalculateIncomingStringLength(Span<byte> buffer, ref int offset)
		{
			//This is complicated, we generically deserialize the primitive length BUT we run into the issue
			//where we NEED a true non-generic primitive to add (without generic math dependency) so we must
			//then do an Unsafe cast. We cannot realistically have a buffer bigger than 2 billion bytes anyway, no matter how it's
			//encoded so this is probably safe to do.
			TLengthType length = GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Read(buffer, ref offset);
			int stringLength = length.Reinterpret<TLengthType, int>();
			return stringLength;
		}

		public sealed override void Write(string value, Span<byte> buffer, ref int offset)
		{
			//WARNING: Doing use this calculated string length in ANYTHING but writing, it may contain adjusted sizes.
			//We must write the length prefix first into the buffer
			int stringLength = CalculateOutgoingStringLength(value);
			stringLength++; //add terminator character
			GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Write(stringLength.Reinterpret<int, TLengthType>(), buffer, ref offset);

			int expectedByteLength = value.Length * MaximumCharacterSize;
			int lastOffset = offset;
			DecoratedSerializer.Write(value, buffer.Slice(0, expectedByteLength + offset), ref offset);

			//TODO: This is a COMPLETE hack that should be toggleable honestly.
			//This isn't *really* how we should handle variable length encodings and stuff, but PSOBB does fixed-length UTF16 for fixed/known size
			//So to compensate for this we adjust the buffer offset to pretend we're fixed-length
			if (offset != lastOffset + expectedByteLength)
				while(offset < lastOffset + expectedByteLength)
					GenericTypePrimitiveSerializerStrategy<byte>.Instance.Write(0, buffer, ref offset);

			//Now we can write terminator
			DecoratedTerminatorStrategy.Write(value, buffer, ref offset);
		}

		private static int CalculateOutgoingStringLength(string value)
		{
			int stringLength = value.Length;
			return stringLength;
		}
	}
}
