using System;
using System.Reflection.Emit;
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
		private delegate IntPtr MemoryAddressHack(ref TType t);
		
		/// <summary>
		/// Represents the required size 
		/// </summary>
		protected virtual int ByteArrayRepresentationSize { get; } = sizeof(TType); //platform/language differences may make this not work but it's overidable.
		
		/// <summary>
		/// Internally managed per-generic Func delegate that converts the bytes to TType.
		/// </summary>
		private static Func<byte[], int, TType> byteConversionReadingDelegate { get; }
		
		//This magic delegate let's us avoid having an abstract method just for grabbing the address
		//of the TType instance.
		/// <summary>
		/// Internally managed delegate that produces memory address for any TType.
		/// </summary>
		private static MemoryAddressHack memoryAddressGrabber { get; }
		
		static UnsafeByteConverterTypeSerializer()
		{
			string bitconvertMethodName = $"To{typeof(TType).Name)}"; 
			byteConversionReadingDelegate = Delegate.CreateDelegate(typeof(Func<TType, byte[], int>),
				typeof(BitConverter).GetMethod(bitconvertMethodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(byte[]), typeof(int) }), true) as Func<byte[], int, TType>;
			
			memoryAddressGrabber = CreateMemoryAddressGrabber();
		}
		
		private static MemoryAddressHack CreateMemoryAddressGrabber()
		{
			//Indicates how to make a Type a Ref Type https://limbioliong.wordpress.com/2011/07/22/passing-a-reference-parameter-to-type-memberinvoke/
			//This shows some stuff about emitting IL for & and IntPtr stuff: http://stackoverflow.com/questions/28728003/il-emit-struct-serializer
			
			DynamicMethod dynamicMethod = new DynamicMethod($"GrabAddressFor{typeof(TType).Name}", typeof(IntPtr), new Type[] { typeof(TType).MakeByRefType() });
			
			string parameterName = "ttypeValue";
			dm.DefineParameter(1, ParameterAttributes.None, parameterName);
			
			//Generate the generator
			ILGenerator generator = dynamicMethod.GetILGenerator();
			LocalBuilder localBuilderOfIntPtr = generator.DeclareLocal(typeof(IntPtr)); //declare a local IntPtr
			
			generator.Emit(OpCodes.Ldloca_S, p); //pushes the IntPtr on the stack
			generator.Emit(OpCodes.Ldarga_S, (byte)0); //push the 0th object on to the stack. This is the TType ref parameter.
			
			generator.Emit(OpCodes.Conv_U); //Convert to unsigned native int, pushing native int on stack. It does this conversion on the top of the stack. In this case TType
			
			ConstructorInfo intPtrCtor = typeof(IntPtr).GetConstructor(new[] {typeof(void*)}); //create a ctor info for the IntPtr type
			
			//See here for explaination: https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.call%28v=vs.110%29.aspx
			//Basically, takes the values on the stack and calls the method with them
			generator.Emit(OpCodes.Call, intPtrCtor);
			
			//Return the value on the stack (the IntPtr that was just constructed)
			generator.Emit(OpCodes.Ret);
			
			return (MemoryAddressHack)dynamicMethod.CreateDelegate(typeof(MemoryAddressHack));
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
