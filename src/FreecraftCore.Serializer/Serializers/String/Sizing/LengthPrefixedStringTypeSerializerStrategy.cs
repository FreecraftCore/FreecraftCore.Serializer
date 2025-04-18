﻿using System;
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

		public LengthPrefixedStringTypeSerializerStrategy() 
			: base(DecoratedSerializer.EncodingStrategy)
		{
			
		}

		/// <inheritdoc />
		public sealed override string Read(Span<byte> buffer, ref int offset)
		{
			int length = CalculateIncomingStringLength(buffer, ref offset);

			if(length == 0)
				return String.Empty;

			// TODO: This will fail for UTF8 because it's 2 and we can sometimes have 1 byte strings
			//Null terminator is missing??
			if (length < SizeInfo.MinimumCharacterSize)
			{
				DecoratedTerminatorStrategy.Read(buffer, ref offset);
				return String.Empty;
			}

			//Read until terminator is found, then we skip over terminator in the buffer.
			string value = DecoratedSerializer.Read(buffer, ref offset);
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

		/// <inheritdoc />
		public sealed override void Write(string value, Span<byte> buffer, ref int offset)
		{
			//WARNING: Doing use this calculated string length in ANYTHING but writing, it may contain adjusted sizes.
			//We must write the length prefix first into the buffer
			int stringLength = CalculateOutgoingStringLength(value);
			stringLength++; //add terminator character
			GenericTypePrimitiveSerializerStrategy<TLengthType>.Instance.Write(stringLength.Reinterpret<int, TLengthType>(), buffer, ref offset);

			DecoratedSerializer.Write(value, buffer, ref offset);

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
