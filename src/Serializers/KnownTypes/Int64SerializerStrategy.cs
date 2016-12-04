using System;

namespace FreecraftCore.Payload.Serializer
{
	//This is the same as C++/Trinitycore's Int6432
	/// <summary>
	/// Int64 serializer.
	/// </summary>
	[KnownTypeSerializer]
	public class Int64SerializerStrategy : SharedBufferTypeSerializer<Int64>
	{
		/// <summary>
		/// Perform the steps necessary to serialize the Int64.
		/// </summary>
		/// <param name="value">The uInt64 to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(Int64 value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((Int64*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}
		
		/// <summary>
		/// Perform the steps necessary to deserialize a Int64.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A uInt64 value from the reader.</returns>
		public unsafe override Int64 Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (Int64 size)
			byte[] bytes = source.ReadBytes(sizeof(Int64));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((Int64*)bytePtr);
		}
	}
}
