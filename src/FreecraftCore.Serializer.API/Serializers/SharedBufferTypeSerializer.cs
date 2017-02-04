using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

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


		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(TType);

		/// <inheritdoc />
		public abstract SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// Sharable byte buffer for 0 allocation serialization.
		/// </summary>
		protected byte[] sharedByteBuffer;

		protected SharedBufferTypeSerializer()
		{
			//Cant use sizeof
			sharedByteBuffer = new byte[Marshal.SizeOf(typeof(TType))];
		}

		/// <inheritdoc />
		public abstract void Write(TType value, [NotNull] IWireMemberWriterStrategy dest);

		/// <inheritdoc />
		public abstract TType Read([NotNull] IWireMemberReaderStrategy source);

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, [NotNull] IWireMemberWriterStrategy dest)
		{
			Write((TType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read([NotNull] IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public TType Read(ref TType obj, IWireMemberReaderStrategy source)
		{
			//TODO: Support complex shared struct types with shared buffers. Not important though.
			obj = Read(source);

			return obj;
		}
	}
}
