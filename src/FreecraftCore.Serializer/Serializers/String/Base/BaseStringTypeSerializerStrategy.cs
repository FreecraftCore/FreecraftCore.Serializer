using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for base string serialization strategy.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseStringTypeSerializerStrategy<TChildType> : BaseEncodableTypeSerializerStrategy<TChildType>, IStringTypeSerializerStrategy
		where TChildType : BaseStringTypeSerializerStrategy<TChildType>, new()
	{
		protected BaseStringTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{

		}

		public override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_buffer.html
			//They use 0 byte to terminate the string in the stream
			buffer = buffer.Slice(offset);

			//This is used to track larger than 1 char null terminators
			bool terminatorFound = false;

			//How many character bytes have been counted
			int currentByteCount = 0;

			//Read a byte from the stream; Stop when we find a 0
			//OR if we exceed the provided string length. MEANING that we found the end of a non-null terminated string.
			//Or a KNOWN SIZE string.
			for(int index = 0; index < buffer.Length && !terminatorFound; index += CharacterSize)
			{
				currentByteCount += CharacterSize;

				//If all are 0 (we found null terminator) and terminator will be TRUE and we break out.
				terminatorFound = true;
				for (int i = 0; i < CharacterSize && index + i < buffer.Length; i++) //important to make sure we don't go outside the bounds
					if (buffer[index + i] != 0)
					{
						terminatorFound = false;
						break;
					}
			}

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//I have decided to support empty strings instead of null
			if(currentByteCount == 0 || currentByteCount == CharacterSize) //found only null terminator
			{
				//Found nothing, so we don't add anything (parsing/rading the terminator is NOT the job of this serializer
				//So DON'T change the offset on empty nullterminated strings.
				return String.Empty;
			}

			//To access the underlying memory to convert it to a string we must
			fixed(byte* bytes = &buffer.GetPinnableReference())
			{
				//This serializer DOESN'T discard the null terminator so don't offset by it if found.
				int trueStringSize = terminatorFound ? currentByteCount - CharacterSize : currentByteCount;

				//Shift forward by offset, otherwise we read wrong data!!
				offset += trueStringSize;
				return ReadEncodedString(bytes, trueStringSize);
			}
		}

		/// <summary>
		/// Gets an allocated string from the encoding strategy.
		/// Reads <see cref="trueStringSize"/> many bytes into the unsafe <see cref="bytes"/> buffer.
		/// </summary>
		/// <param name="bytes">Unsafe pointer buffer.</param>
		/// <param name="trueStringSize">The true string size in bytes./</param>
		/// <returns>An allocated string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual unsafe string ReadEncodedString(byte* bytes, int trueStringSize)
		{
			return EncodingStrategy.GetString(bytes, trueStringSize);
		}

		public override unsafe void Write(string value, Span<byte> buffer, ref int offset)
		{
			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_buffer.html
			//They use 0 byte to terminate the string in the stream
			buffer = buffer.Slice(offset);

			//We should check this so we don't try to decode
			//or write null. Null should be considered empty.
			if(!String.IsNullOrEmpty(value))
			{
				fixed(char* chars = value)
				fixed(byte* bytes = &buffer.GetPinnableReference())
				{
					//Shift forward by offset, otherwise we read wrong data!!
					//Wrote string so NOW we need to shift the offset
					offset += WriteEncodedString(chars, value.Length, bytes, buffer.Length);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual unsafe int WriteEncodedString(char* chars, int charsLength, byte* bytes, int byteLength)
		{
			return EncodingStrategy.GetBytes(chars, charsLength, bytes, byteLength);
		}
	}
}
