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
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Perform the steps necessary to serialize the UInt64.
		/// </summary>
		/// <param name="value">The uUInt64 to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(UInt64 value, IWireMemberWriterStrategy dest)
		{
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
		
		/// <summary>
		/// Perform the steps necessary to deserialize a UInt64.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A uUInt64 value from the reader.</returns>
		public unsafe override UInt64 Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (UInt64 size)
			byte[] bytes = source.ReadBytes(sizeof(UInt64));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((UInt64*)bytePtr);
		}
	}
}
