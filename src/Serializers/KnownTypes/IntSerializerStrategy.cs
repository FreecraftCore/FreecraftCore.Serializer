using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="int"/>.
	/// </summary>
	public class intSerializerStrategy : ITypeSerializerStrategy<int>
	{
		/// <summary>
        /// Perform the steps necessary to serialize the int.
        /// </summary>
        /// <param name="value">The int to be serialized.</param>
        /// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe void Write(int value, IWireMemberWriterStrategy dest)
		{
			//Review the source for Trinitycore's int reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for int): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They cast the int to uint8* which is basically a byte array. They serialize it that way
			//We're going to do something similar with unsafe C# code
		
			byte[] intBytes = null;
			
			//Get the memory address of the value
			//We're going to do
			fixed(int* intPtr = &value) //we fix the int so GC doesn't move it
			{
				intBytes = new byte[4]; //4 byte array representing the ints
					
				Marshal.Copy((IntPtr)intPtr, intBytes, 0, 4);
			}
			
			//Write the bytes to the destination
			dest.Write(intBytes);
		}
		
		/// <summary>
        /// Perform the steps necessary to deserialize a int.
        /// </summary>
        /// <param name="source">The reader providing the input data.</param>
        /// <returns>A int value from the reader.</returns>
		public int Read(IWireMemberReaderStrategy source)
		{
			//Review the source for Trinitycore's int reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for int): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//4 bytes are treated as an int
			
			//Read 4 bytes for the int and just use converter
			//It's pretty fast: http://stackoverflow.com/questions/4326125/faster-way-to-convert-byte-array-to-int
			BitConverter.ToInt32(source.ReadBytes(4));
		}

		public intSerializerStrategy()
		{
			//this serializer needs no subserializers or services.
		}
	}
}
