using System;

namespace FreecraftCore.Payload.Serializer
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
		/// Sharable byte buffer for 0 allocation serialization.
		/// </summary>
		protected byte[] sharedByteBuffer = new byte[sizeof(TType)];
		
		public abstract void Write(TType value, IWireMemberWriterStrategy dest);
	
		public abstract TType Read(IWireMemberReaderStrategy source);
	}
}
