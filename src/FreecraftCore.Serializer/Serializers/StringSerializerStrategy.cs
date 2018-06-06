using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="string"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class StringSerializerStrategy : BaseStringSerializerStrategy
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public StringSerializerStrategy()
			: base(Encoding.ASCII)
		{
			//This is the default that WoW uses
			//however now that the serializer is being used for other projects
			//we need to have the option to use different ones.
		}

		public StringSerializerStrategy(Encoding encodingStrategy, SerializationContextRequirement contextRequirement = SerializationContextRequirement.Contextless)
			: base(encodingStrategy)
		{
			//We may need to override this as contextual
			ContextRequirement = contextRequirement;
		}

		/// <inheritdoc />
		public override void Write(string value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream
			
			//We should check this so we don't try to decode
			//or write null. Null should be considered empty.
			if(!String.IsNullOrEmpty(value))
			{
				//TODO: Pointer hack for speed
				//Convert the string to bytes
				//Not sure about encoding yet
				byte[] stringBytes = EncodingStrategy.GetBytes(value);

				dest.Write(stringBytes);
			}
			
			//Write the null terminator; Client expects it.
			for(int i = 0; i < CharacterSize; i++)
				dest.Write(0);
		}
		
		/// <summary>
		/// Perform the steps necessary to deserialize a string.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A string value from the reader.</returns>
		public override string Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream
			
			//TODO: We can likely do some fancy pointer stuff to make this much cheaper.
			//Read a byte from the stream; Stop when we find a 0
			List<byte> stringBytes = new List<byte>();

			//This is used to track larger than 1 char null terminators
			int zeroByteCountFound = 0;

			//How many characters have been counted
			int currentCharCount = 0;

			do
			{
				byte currentByte = source.ReadByte();
				currentCharCount++;

				stringBytes.Add(currentByte);

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

			} while(zeroByteCountFound < CharacterSize);

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//I have decided to support empty strings instead of null
			return stringBytes.Count - CharacterSize <= 0 ? "" : EncodingStrategy.GetString(stringBytes.ToArray(), 0, Math.Max(0, stringBytes.Count - CharacterSize));
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//We should check this so we don't try to decode
			//or write null. Null should be considered empty.
			if(!String.IsNullOrEmpty(value))
			{
				//See sync method for doc
				byte[] stringBytes = EncodingStrategy.GetBytes(value);

				await dest.WriteAsync(stringBytes)
					.ConfigureAwait(false);
			}

			//TODO: Make this more efficient instead of multiple wrtie calls
			//Write the null terminator; Client expects it.
			for(int i = 0; i < CharacterSize; i++)
				await dest.WriteAsync(0)
					.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<string> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			//See sync method for doc

			//TODO: Find an average size for WoW strings
			List<byte> stringBytes = new List<byte>();

			//This is used to track larger than 1 char null terminators
			int zeroByteCountFound = 0;

			//How many characters have been counted
			int currentCharCount = 0;

			do
			{
				byte currentByte = await source.ReadByteAsync()
					.ConfigureAwait(false);

				currentCharCount++;

				stringBytes.Add(currentByte);

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

			} while(zeroByteCountFound < CharacterSize);

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//I have decided to support empty strings instead of null
			return stringBytes.Count - CharacterSize <= 0 ? "" : EncodingStrategy.GetString(stringBytes.ToArray(), 0, Math.Max(0, stringBytes.Count - CharacterSize));
		}
	}
}
