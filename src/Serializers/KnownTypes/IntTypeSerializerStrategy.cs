using System;

namespace FreecraftCore.Payload.Serializer.Serializers.KnownTypes
{
	/// <summary>
	/// Description of IntTypeSerializerStrategy.
	/// </summary>
	public class IntTypeSerializerStrategy : SharedBufferTypeSerializer<int>
	{
		/// <summary>
        /// Perform the steps necessary to serialize the int.
        /// </summary>
        /// <param name="value">The int to be serialized.</param>
        /// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(int value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((int*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}
		
		/// <summary>
        /// Perform the steps necessary to deserialize a int.
        /// </summary>
        /// <param name="source">The reader providing the input data.</param>
        /// <returns>A int value from the reader.</returns>
		public unsafe override int Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (int size)
			byte[] bytes = source.ReadBytes(sizeof(int));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((int*)bytePtr);
		}
	}
}
