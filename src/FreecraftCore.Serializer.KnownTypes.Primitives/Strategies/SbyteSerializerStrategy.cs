using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's sbyte (or sbyte which we're going to use sbytes for)
	/// <summary>
	/// Description of sbyteTypeSerializerStrategy.
	/// </summary>
	[KnownTypeSerializer]
	public class SByteSerializerStrategy : SharedBufferTypeSerializer<sbyte>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public unsafe override void Write(sbyte value, IWireStreamWriterStrategy dest)
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

		/// <inheritdoc />
		public unsafe override sbyte Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read 2 bytes (sbyte size)
			byte[] bytes = source.ReadBytes(sizeof(sbyte));
			
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed(byte* bytePtr = &bytes[0])
				return *((sbyte*)bytePtr);
		}
	}
}
