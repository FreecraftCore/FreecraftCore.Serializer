using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Contract for types that require unsafe byte[] reading and writing.
	/// </summary>
	public abstract class UnsafeByteConverterTypeSerializer<TType> : ITypeSerializerStrategy<TType>
		where TType : struct
	{
		/// <summary>
		/// Represents the required size 
		/// </summary>
		protected virtual int ByteArrayRepresentationSize { get; } = sizeof(TType); //platform/language differences may make this not work but it's overidable.
		
		/// <summary>
		/// Internally managed per-generic Func delegate that converts the bytes to TType.
		/// </summary>
		private static Func<byte[], int, TType> byteConversionReadingDelegate { get; }
		
		static UnsafeByteConverterTypeSerializer()
		{
			string bitconvertMethodName = $"To{typeof(TType).Name)}"; 
			byteConversionReadingDelegate = Delegate.CreateDelegate(typeof(Func<TType, byte[], int>),
				typeof(BitConverter).GetMethod(bitconvertMethodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(byte[]), typeof(int) }), true) as Func<byte[], int, TType>;
		}
		
		/// <summary>
        /// Perform the steps necessary to serialize the int.
        /// </summary>
        /// <param name="value">The int to be serialized.</param>
        /// <param name="dest">The writer entity that is accumulating the output data.</param>
		public unsafe void Write(TType value, IWireMemberWriterStrategy dest)
		{
			//Review the source for Trinitycore's writing for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for Type): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They cast the Types to uint8* which is basically a byte array. They serialize it that way
			//We're going to do something similar with unsafe C# code
			
			byte[] ttypeBytes = null;
			
			//Get the memory address of the value
			IntPtr intPtr = GetPointer(ref value); //using ref to prevent improper address value
				
			ttypeBytes = new byte[this.ByteArrayRepresentationSize]; //ByteArrayRepresentationSize sized byte array representing the TType
				
			Marshal.Copy(intPtr, ttypeBytes, 0, this.ByteArrayRepresentationSize);
			
			//Write the bytes to the destination
			dest.Write(ttypeBytes);
		}
		
		//Sadly we have to do this. C# won't let us do it... Maybe we can generate the IL to get around it?
		protected abstract unsafe IntPtr GetPointer(ref TType value);
		
		/// <summary>
        /// Perform the steps necessary to deserialize a int.
        /// </summary>
        /// <param name="source">The reader providing the input data.</param>
        /// <returns>A int value from the reader.</returns>
		public TType Read(IWireMemberReaderStrategy source)
		{
			//Review the source for Trinitycore's int reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for int): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//We use the sizeof to get the right size for the cast
			
			return byteConversionReadingDelegate((ByteArrayRepresentationSize > 1) ? (source.ReadBytes(ByteArrayRepresentationSize) : source.ReadByte()), ByteArrayRepresentationSize);
			
			//Read 4 bytes for the int and just use converter
			//It's pretty fast: http://stackoverflow.com/questions/4326125/faster-way-to-convert-byte-array-to-int
			BitConverter.ToInt32(source.ReadBytes(4));
		}

		public UnsafeByteConverterTypeSerializer()
		{
			
		}
	}
}
