using System;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's UInt6432
	/// <summary>
	/// UInt64 serializer.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt64SerializerStrategy : SharedBufferTypeSerializer<UInt64>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public unsafe override void Write(UInt64 value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((UInt64*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <inheritdoc />
		public unsafe override UInt64 Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read 4 bytes (UInt64 size)
			byte[] bytes = source.ReadBytes(sizeof(UInt64));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((UInt64*)bytePtr);
		}
	}
}
