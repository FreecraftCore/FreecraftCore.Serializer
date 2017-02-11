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
		//TODO: Refactor
		private class ByteRepresentationCapture : IWireStreamWriterStrategy
		{
			public byte[] ByteRepresentation { get; private set; }

			public void Write(byte[] data)
			{
				if (data == null) throw new ArgumentNullException(nameof(data));

				ByteRepresentation = ByteRepresentation?.Concat(data).ToArray() ?? data;
			}

			public void Write(byte[] data, int offset, int count)
			{
				if (data == null) throw new ArgumentNullException(nameof(data));

				//TODO: Check if this is valid
				ByteRepresentation = ByteRepresentation?.Concat(data.Skip(offset).Take(count)).ToArray() ?? data.Skip(offset).Take(count).ToArray();
			}

			public void Write(byte data)
			{
				//Do nothing
				//This should be implemented but for now we know this never gets called
				//This is bad design not to have it implemented but for the sake of time
				//I skip it
			}

			public byte[] GetBytes()
			{
				//TODO; Should we return null?
				return ByteRepresentation;
			}

			public void Dispose()
			{

			}
		}

		//TODO: Refactor
		private class ByteChunkContainer : IWireStreamReaderStrategy
		{
			private int currentIndex { get; set; } = 0;

			[NotNull]
			private byte[] ByteRepresentation { get; }

			public ByteChunkContainer([NotNull] byte[] byteRepresentation)
			{
				if (byteRepresentation == null) throw new ArgumentNullException(nameof(byteRepresentation));

				ByteRepresentation = byteRepresentation;
			}

			public byte ReadByte()
			{
				return ByteRepresentation[currentIndex++];
			}

			public byte PeekByte()
			{
				return ByteRepresentation[currentIndex];
			}

			public byte[] ReadAllBytes()
			{
				byte[] bytes = ByteRepresentation.Skip(currentIndex).ToArray();
				currentIndex = ByteRepresentation.Length;

				return bytes;
			}

			public byte[] ReadBytes(int count)
			{
				byte[] bytes = ByteRepresentation.Skip(currentIndex).Take(count).ToArray();
				currentIndex += count;

				return bytes;
			}

			public byte[] PeakBytes(int count)
			{
				return ByteRepresentation.Skip(currentIndex).Take(count).ToArray();
			}

			public void Dispose()
			{

			}
		}

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

		public override byte[] GetBytes(TType obj)
		{
			//This is a SUPER hack and relies on the fact that we KNOW the sharedByteBuffer will contain
			//the byte representation of the object
			ByteRepresentationCapture capture = new ByteRepresentationCapture();

			//Must lock to protect shared buffer
			lock (syncObj)
			{
				Write(obj, capture);
				return capture.ByteRepresentation.ToArray(); //make copy otherwise it may be shared buffer
			}
		}

		public override TType FromBytes(byte[] bytes)
		{
			return Read(new ByteChunkContainer(bytes));
		}
	}
}
