using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	//Trinitycore just casts floats to bytes (append((uint8 *)&value, sizeof(value));)
	/// <summary>
	/// A known-type serializer for <see cref="float"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class FloatSerializerStrategy : SharedBufferTypeSerializer<float>
	{
		//All primitive serializer stragies are contextless
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Perform the steps necessary to serialize the float.
		/// </summary>
		/// <param name="value">The ufloat to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(float value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.sharedByteBuffer[0])
					*((float*)bytePtr) = value;

				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <summary>
		/// Perform the steps necessary to deserialize a float.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A ufloat value from the reader.</returns>
		public unsafe override float Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (float size)
			byte[] bytes = source.ReadBytes(sizeof(float));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((float*)bytePtr);
		}
	}
}
