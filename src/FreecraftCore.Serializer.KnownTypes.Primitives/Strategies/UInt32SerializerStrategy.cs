using System;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's int32
	/// <summary>
	/// UInt32 serializer.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt32SerializerStrategy : SharedBufferTypeSerializer<uint>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public unsafe override void Write(uint value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((uint*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <inheritdoc />
		public unsafe override uint Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read 4 bytes (int size)
			byte[] bytes = source.ReadBytes(sizeof(uint));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((uint*)bytePtr);
		}
	}
}
