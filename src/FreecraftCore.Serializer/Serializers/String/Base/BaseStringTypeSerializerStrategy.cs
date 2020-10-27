using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for base string serialization strategy.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseStringTypeSerializerStrategy<TChildType> : BaseEncodableTypeSerializerStrategy<TChildType>
		where TChildType : BaseStringTypeSerializerStrategy<TChildType>, new()
	{
		protected BaseStringTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{

		}

		public sealed override unsafe string Read(Span<byte> source, ref int offset)
		{
			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream

			//This is used to track larger than 1 char null terminators
			bool terminatorFound = false;

			//How many characters have been counted
			int currentCharCount = 0;

			//Read a byte from the stream; Stop when we find a 0
			//OR if we exceed the provided string length. MEANING that we found the end of a non-null terminated string.
			//Or a KNOWN SIZE string.
			for(int index = offset; index < source.Length && !terminatorFound; index += CharacterSize)
			{
				currentCharCount += CharacterSize;

				//If all are 0 (we found null terminator) and terminator will be TRUE and we break out.
				terminatorFound = true;
				for (int i = 0; i < CharacterSize; i++)
					if (source[offset + i] != 0)
					{
						terminatorFound = false;
						break;
					}
			}

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//I have decided to support empty strings instead of null
			if(currentCharCount == CharacterSize || currentCharCount == 0)
			{
				//Important to include null terminator bytes!!
				offset += currentCharCount;
				return String.Empty;
			}

			//To access the underlying memory to convert it to a string we must
			fixed(byte* bytes = &source.GetPinnableReference())
			{
				//Shift forward by offset, otherwise we read wrong data!!
				byte* offsetBytes = bytes + offset;
				offset += currentCharCount;
				return EncodingStrategy.GetString(offsetBytes, currentCharCount - CharacterSize);
			}
		}

		public sealed override unsafe void Write(string value, Span<byte> destination, ref int offset)
		{
			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream

			//We should check this so we don't try to decode
			//or write null. Null should be considered empty.
			if(!String.IsNullOrEmpty(value))
			{
				fixed(char* chars = value)
				fixed(byte* bytes = &destination.GetPinnableReference())
				{
					//Shift forward by offset, otherwise we read wrong data!!
					byte* offsetBytes = bytes + offset;
					EncodingStrategy.GetBytes(chars, value.Length, offsetBytes, CharacterSize * value.Length);
				}

				//Wrote string so NOW we need to shift the offset
				offset += CharacterSize * value.Length;
			}
		}
	}
}
