﻿using JetBrains.Annotations;
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
	public abstract class BaseStringTypeSerializerStrategy<TChildType> : StatelessTypeSerializer<TChildType, string> 
		where TChildType : StatelessTypeSerializer<TChildType, string>, new()
	{
		/// <summary>
		/// The encoding strategy to use for the string serialization.
		/// </summary>
		protected Encoding EncodingStrategy { get; }

		/// <summary>
		/// Size of the individual chars.
		/// </summary>
		protected int CharacterSize { get; }

		protected BaseStringTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
		{
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			EncodingStrategy = encodingStrategy;

			//Due to how coreclr/core works we need to support potential child Types of the encoding types
			//See: https://github.com/dotnet/coreclr/blob/f31097f14560b193e76a7b2e1e61af9870b5356b/src/System.Private.CoreLib/shared/System/Text/ASCIIEncoding.cs#L24
			//We cannot trust .NET to give us correct sizes
			if(CheckEncodingIsOfType<ASCIIEncoding>(encodingStrategy.GetType()))
				CharacterSize = 1;
			else if(CheckEncodingIsOfType<UnicodeEncoding>(encodingStrategy.GetType()))
				CharacterSize = 2;
			else if(CheckEncodingIsOfType<UTF32Encoding>(encodingStrategy.GetType()))
				CharacterSize = 4;
			else if(CheckEncodingIsOfType<UTF8Encoding>(encodingStrategy.GetType()))
				CharacterSize = 1; //In WoW DBC UTF8 strings are null terminated with a single 0 byte.
			else
				throw new InvalidOperationException($"Encounted unknown Encoding: {encodingStrategy.GetType().Name}. Due to .NET behavior we cannot trust anything but manual char size.");
		}

		/// <summary>
		/// Indicates if an encoding type is the same as a provided encoding type.
		/// </summary>
		/// <typeparam name="TEncodingType">The encoding type to check.</typeparam>
		/// <param name="encodingTypeToCheck">The type to compare to.</param>
		/// <returns>True if the types match.</returns>
		protected static bool CheckEncodingIsOfType<TEncodingType>([NotNull] Type encodingTypeToCheck)
			where TEncodingType : Encoding
		{
			if(encodingTypeToCheck == null) throw new ArgumentNullException(nameof(encodingTypeToCheck));

			return encodingTypeToCheck == typeof(TEncodingType) || typeof(TEncodingType).GetTypeInfo().IsAssignableFrom(encodingTypeToCheck);
		}

		public override unsafe string Read(Span<byte> source, ref int offset)
		{
			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream

			//This is used to track larger than 1 char null terminators
			int zeroByteCountFound = 0;

			//How many characters have been counted
			int currentCharCount = 0;

			//Read a byte from the stream; Stop when we find a 0
			for(int index = offset; index < source.Length && zeroByteCountFound < CharacterSize; index++)
			{
				byte currentByte = source[offset];
				currentCharCount++;

				//If we find a 0 byte we need to track it
				//char could be 1 byte or even 4 so we need to reset
				//if we encounter an actual character before we find all
				//null terminator bytes
				if(currentByte == 0)
					zeroByteCountFound++;
				else
					zeroByteCountFound = 0;

				//if we found 4 0 bytes in arow but we didn't find a full char set of CharacterSize
				//then we should reset. This can happen if you have {5 0 0 0} {0 0 0 0} it will stop after the first 4
				//But with this is will read the whole {0 0 0 0}.
				if(currentCharCount % CharacterSize == 0 && zeroByteCountFound < CharacterSize)
					zeroByteCountFound = 0;
			}

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//I have decided to support empty strings instead of null
			int finalCharCount = Math.Max(0, currentCharCount - CharacterSize);
			if(currentCharCount - CharacterSize <= 0 || finalCharCount == 0)
			{
				//Important to include null terminator bytes!!
				offset += zeroByteCountFound;
				return String.Empty;
			}

			//To access the underlying memory to convert it to a string we must
			fixed(byte* bytes = &source.GetPinnableReference())
			{
				//Shift forward by offset, otherwise we read wrong data!!
				byte* offsetBytes = bytes + offset;
				offset += currentCharCount;
				return EncodingStrategy.GetString(offsetBytes, finalCharCount);
			}
		}

		public override unsafe void Write(string value, Span<byte> destination, ref int offset)
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

			//Write a null terminator now
			for(int i = 0; i < CharacterSize; i++)
				destination[offset + i] = 0;

			//Add the terminator size.
			offset += CharacterSize;
		}
	}
}
