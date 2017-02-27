using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
#pragma warning disable 618

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
		protected byte[] SharedByteBuffer { get; }

		protected static int ByteRepresentationSize { get; } = Marshal.SizeOf(typeof(TType));

		protected SharedBufferTypeSerializer()
		{
			//Cant use sizeof
			//TODO: SizeOf is obsolete here we need to replace it soon
			SharedByteBuffer = new byte[ByteRepresentationSize];
		}

		//We now write and read in the abstract because we allow implementers
		//to only provide the serialization aspect.
		/// <inheritdoc />
		public override TType Read([NotNull] IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return DeserializeFromBuffer(source.ReadBytes(ByteRepresentationSize));
		}

		/// <inheritdoc />
		public override void Write(TType value, [NotNull] IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			lock (syncObj)
			{
				bool isPopulated = PopulateSharedBufferWith(value);

				if(!isPopulated)
					throw new InvalidOperationException($"Failed to populate {nameof(SharedByteBuffer)} in {nameof(SharedBufferTypeSerializer<TType>)} for Type: {typeof(TType)} with Value: {value}.");

				//At this point the SharedBuffer contains the serialized representation of the value
				//DO NOT RELEASE LOCK UNTIL WRITTEN
				dest.Write(SharedByteBuffer);
			}
		}

		/// <inheritdoc />
		public override Task WriteAsync(TType value, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Figure out a way to make this safely async
			//We cannot lock to protect the shared buffer in an async context
			Write(value, dest);

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public override async Task<TType> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return DeserializeFromBuffer(await source.ReadBytesAsync(ByteRepresentationSize));
		}

		/// <summary>
		/// Implementers should populate the <see cref="SharedByteBuffer"/> with the serialized
		/// format of the <typeparamref name="TType"/> <see cref="value"/>.
		/// </summary>
		/// <param name="value">The object to populate the buffer with.</param>
		/// <returns>Indicates if the buffer has been populated.</returns>
		protected abstract unsafe bool PopulateSharedBufferWith(TType value);

		/// <summary>
		/// Implementers should take the <see cref="bytes"/>
		/// a <typeparamref name="TType"/> object from the byte representation.
		/// </summary>
		/// <returns>A non-null instance of <typeparamref name="TType"/> deserialized from the provided buffer.</returns>
		[NotNull]
		protected abstract unsafe TType DeserializeFromBuffer(byte[] bytes);

		public override byte[] GetBytes(TType obj)
		{
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
