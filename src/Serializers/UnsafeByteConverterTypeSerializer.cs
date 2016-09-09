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
		/// <summary>
		/// Internally managed per-generic Func delegate that converts the bytes to TType.
		/// </summary>
		private static Func<byte[], int, TType> byteConversionReadingDelegate { get; }
		
		//This magic delegate let's us avoid having an abstract method just for grabbing the address
		//of the TType instance.
		/// <summary>
		/// Internally managed delegate that produces memory address for any TType.
		/// </summary>
		private static Action<TType, IWireMemberWriterStrategy> writerAction { get; }
		
		private int byteArrayRepresentationSize { get; } = sizeof(TType);
		
		static UnsafeByteConverterTypeSerializer()
		{
			string bitconvertMethodName = $"To{typeof(TType).Name)}"; 
			byteConversionReadingDelegate = Delegate.CreateDelegate(typeof(Func<TType, byte[], int>),
				typeof(BitConverter).GetMethod(bitconvertMethodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(byte[]), typeof(int) }), true) as Func<byte[], int, TType>;
			
			if(byteConversionReadingDelegate == null)
				throw new InvalidOperationException($"Failed to find {nameof(typeof(BitConverter).Name} method for Type: {typeof(TType).Name}.");
			
			writerAction = CreateDelegate();
		}
	
		public static Action<TType, IWireMemberWriterStrategy> CreateDelegate()
		{
			//This code adapted from: http://stackoverflow.com/questions/28728003/il-emit-struct-serializer
			var dm = new DynamicMethod("Write" + typeof (T).Name,
			typeof (void),
			new[] {typeof (T), typeof(IWireMemberWriterStrategy)},
			Assembly.GetExecutingAssembly().ManifestModule);
			const string parameterName = "value";
			dm.DefineParameter(1, ParameterAttributes.None, parameterName);
			var generator = dm.GetILGenerator();
			var intPtrLocal = generator.DeclareLocal(typeof (IntPtr)); //create the IntPtr to store the ref
			generator.DeclareLocal(typeof (byte[])); //create the byte[] result
			generator.DeclareLocal(typeof (byte[])); //create the lhs byte array
			generator.Emit(OpCodes.Ldloca_S, intPtrLocal); //load the IntPtr
			generator.Emit(OpCodes.Ldarga_S, (byte)0); //Load the first arg T
			generator.Emit(OpCodes.Conv_U); //get address from arg
			
			var intPtrCtor = typeof (IntPtr).GetConstructor(new[] {typeof(void*)});
			
			generator.Emit(OpCodes.Call, intPtrCtor); //call the ctor for IntrPtr with the &t
			
			generator.Emit(OpCodes.Ldc_I4_S, (short)Marshal.SizeOf(typeof(TType))); //get the size of T (should work most cases)
			generator.Emit(OpCodes.Newarr, typeof (byte));
			generator.Emit(OpCodes.Stloc_1);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Conv_I4);
			
			var marshalCopy = typeof (Marshal).GetMethod("Copy", new[] {typeof (IntPtr), typeof (byte[]), typeof (int), typeof (int)});
			generator.EmitCall(OpCodes.Call, marshalCopy, null);
			generator.Emit(OpCodes.Ldarg_1); //load writer
			generator.Emit(OpCodes.Ldloc_1);
			generator.EmitCall(OpCodes.Callvirt, typeof(IWireMemberWriterStrategy).GetMethod("Write"), null); //calls the write method
			generator.Emit(OpCodes.Ret);
			
			return (Action<TType, IWireMemberWriterStrategy>)dm.CreateDelegate(typeof(Action<TType, IWireMemberWriterStrategy>));
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
			
			//writes the value with the writer
			//It's a magic IL method that grabs the address of value and does 
			writerAction(value, dest):
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
			
			return byteConversionReadingDelegate((byteArrayRepresentationSize > 1) ? (source.ReadBytes(byteArrayRepresentationSize) : source.ReadByte()), byteArrayRepresentationSize);
		}

		public UnsafeByteConverterTypeSerializer()
		{
			
		}
	}
}
