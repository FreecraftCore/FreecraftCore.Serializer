using System;

namespace FreecraftCore.Payload.Serializer
{
	//This is the same as C++/Trinitycore's int32
	/// <summary>
	/// UInt32 serializer.
	/// </summary>
	public class UInt32SerializerStrategy : SharedBufferTypeSerializer<uint>
	{
		/// <summary>
		/// Perform the steps necessary to serialize the int.
		/// </summary>
		/// <param name="value">The uint to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(uint value, IWireMemberWriterStrategy dest)
		{
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
		
		/// <summary>
		/// Perform the steps necessary to deserialize a int.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A uint value from the reader.</returns>
		public unsafe override uint Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (int size)
			byte[] bytes = source.ReadBytes(sizeof(uint));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((uint*)bytePtr);
		}
	}
}
