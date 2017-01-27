using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's Int16 (or Int16 which we're going to use Int16s for)
	/// <summary>
	/// Description of Int16TypeSerializerStrategy.
	/// </summary>
	[KnownTypeSerializer]
	public class Int16SerializerStrategy : SharedBufferTypeSerializer<Int16>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public unsafe override void Write(Int16 value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((Int16*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <inheritdoc />
		public unsafe override Int16 Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read 2 bytes (int16 size)
			byte[] bytes = source.ReadBytes(sizeof(Int16));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((Int16*)bytePtr);
		}
	}
}
