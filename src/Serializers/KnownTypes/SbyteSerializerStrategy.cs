using System;

namespace FreecraftCore.Payload.Serializer
{
	//This is the same as C++/Trinitycore's sbyte (or sbyte which we're going to use sbytes for)
	/// <summary>
	/// Description of sbyteTypeSerializerStrategy.
	/// </summary>
	[KnownTypeSerializer]
	public class SByteSerializerStrategy : SharedBufferTypeSerializer<sbyte>
	{
		/// <summary>
		/// Perform the steps necessary to serialize the sbyte.
		/// </summary>
		/// <param name="value">The sbyte to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(sbyte value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock(syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed(byte* bytePtr = &this.sharedByteBuffer[0])
					*((sbyte*)bytePtr) = value;
				
				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}
		
		/// <summary>
		/// Perform the steps necessary to deserialize a sbyte.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A sbyte value from the reader.</returns>
		public unsafe override sbyte Read(IWireMemberReaderStrategy source)
		{
			//Read 2 bytes (sbyte size)
			byte[] bytes = source.ReadBytes(sizeof(sbyte));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((sbyte*)bytePtr);
		}
	}
}
