using System;
using System.Runtime.InteropServices;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Description of SharedBufferTypeSerializer.
	/// </summary>
	public abstract class SharedBufferTypeSerializer<TType> : ITypeSerializerStrategy<TType>
		where TType : struct
	{
		//TODO: Do per-thread buffer to speed up multithreading scenarios with serialization.
		
		/// <summary>
		/// Syncronization object.
		/// </summary>
		protected object syncObj { get; } = new object();

		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get; } = typeof(TType);

		public abstract SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// Sharable byte buffer for 0 allocation serialization.
		/// </summary>
		protected byte[] sharedByteBuffer;

		public SharedBufferTypeSerializer()
		{
			//Cant use sizeof
			sharedByteBuffer = new byte[Marshal.SizeOf(typeof(TType))];
		}
		
		public abstract void Write(TType value, IWireMemberWriterStrategy dest);
	
		public abstract TType Read(IWireMemberReaderStrategy source);
	}
}
