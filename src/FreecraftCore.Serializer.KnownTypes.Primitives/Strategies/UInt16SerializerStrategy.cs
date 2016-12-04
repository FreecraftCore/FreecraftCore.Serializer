using System;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's Int16 (or uint16 which we're going to use Int16s for)
	/// <summary>
	/// Description of Int16TypeSerializerStrategy.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt16SerializerStrategy : SharedBufferTypeSerializer<UInt16>
	{
		//All primitive serializer stragies are contextless
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Perform the steps necessary to serialize the int16.
		/// </summary>
		/// <param name="value">The int16 to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(UInt16 value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((UInt16*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}
		
		/// <summary>
		/// Perform the steps necessary to deserialize a int16.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A int16 value from the reader.</returns>
		public unsafe override UInt16 Read(IWireMemberReaderStrategy source)
		{
			//Read 2 bytes (int16 size)
			byte[] bytes = source.ReadBytes(sizeof(UInt16));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((UInt16*)bytePtr);
		}
	}
}
