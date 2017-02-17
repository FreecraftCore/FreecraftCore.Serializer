using System;
using System.Linq;
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
			//TODO: SizeOf is obsolete here we need to replace it soon
			sharedByteBuffer = new byte[Marshal.SizeOf(typeof(TType))];
		}

		public override byte[] GetBytes(TType obj)
		{
			//This is a SUPER hack and relies on the fact that we KNOW the sharedByteBuffer will contain
			//the byte representation of the object
			DefaultStreamWriterStrategy capture = new DefaultStreamWriterStrategy();

			//Must lock to protect shared buffer
			lock (syncObj)
			{
				Write(obj, capture);
				return capture.GetBytes().ToArray(); //make copy otherwise it may be shared buffer
			}
		}

		public override TType FromBytes(byte[] bytes)
		{
			return Read(new DefaultStreamReaderStrategy(bytes));
		}
	}
}
