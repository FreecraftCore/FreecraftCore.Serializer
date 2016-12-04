using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	//Trinitycore just casts doubles to bytes (append((uint8 *)&value, sizeof(value));)
	/// <summary>
	/// A known-type serializer for <see cref="double"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class DoubleSerializerStrategy : SharedBufferTypeSerializer<double>
	{
		/// <summary>
		/// Perform the steps necessary to serialize the double.
		/// </summary>
		/// <param name="value">The udouble to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe override void Write(double value, IWireMemberWriterStrategy dest)
		{
			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.sharedByteBuffer[0])
					*((double*)bytePtr) = value;

				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <summary>
		/// Perform the steps necessary to deserialize a double.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A udouble value from the reader.</returns>
		public unsafe override double Read(IWireMemberReaderStrategy source)
		{
			//Read 4 bytes (double size)
			byte[] bytes = source.ReadBytes(sizeof(double));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((double*)bytePtr);
		}
	}
}
