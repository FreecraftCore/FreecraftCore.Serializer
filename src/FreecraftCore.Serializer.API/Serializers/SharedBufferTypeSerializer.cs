using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Description of SharedBufferTypeSerializer.
	/// </summary>
	public abstract class SharedBufferTypeSerializer<TType> : SimpleTypeSerializerStrategy<TType>
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
		protected byte[] sharedByteBuffer;

		protected SharedBufferTypeSerializer()
		{
			//Cant use sizeof
			sharedByteBuffer = new byte[Marshal.SizeOf(typeof(TType))];
		}
	}
}
