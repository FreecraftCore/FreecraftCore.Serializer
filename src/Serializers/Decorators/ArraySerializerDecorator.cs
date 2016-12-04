using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Decorator for array types.
	/// </summary>
	/// <typeparam name="TObjectType">The type of the object the array contains.</typeparam>
	class ArraySerializerDecorator<TObjectType> : ITypeSerializerStrategy<TObjectType[]>
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TObjectType); } }

		private ITypeSerializerStrategy<TObjectType> decoratedSerializer { get; }

		/// <summary>
		/// Indicates if the aray serializer decorator should write the size of the array into the message.
		/// </summary>
		private bool shouldSerializeSize { get; }

		/// <summary>
		/// Indiciates the known size (0 is unknown)
		/// </summary>
		private int size { get; }

		/// <summary>
		/// Internally controlled by serializer for size handling.
		/// </summary>
		private static ByteSerializerStrategy sharedByteSerializer { get; } = new ByteSerializerStrategy();

		public ArraySerializerDecorator(ITypeSerializerStrategy<TObjectType> singleSerializer, int knownSize = 0)
		{
			if (singleSerializer == null)
				throw new ArgumentNullException(nameof(singleSerializer), $"Provided {nameof(ITypeSerializerStrategy)} to decorate was null.");

			size = knownSize;
			//If no size was provided then we should write it.
			shouldSerializeSize = knownSize == 0;

			decoratedSerializer = singleSerializer;
		}

		public TObjectType[] Read(IWireMemberReaderStrategy source)
		{
			TObjectType[] objectArray = null;
			int readSize = 0;

			//As far as I known TC/WoW writes collection sizes as a uint8 (byte) so that is what we'll do
			if (shouldSerializeSize)
				readSize = sharedByteSerializer.Read(source); //read first byte for collection size
			else
				readSize = size;

			objectArray = new TObjectType[readSize];


			//TODO: Error handling
			//read as many objects as in the message (this is safe for clients. Not so safe if the client sent a message of this type)
			for (int i = 0; i < readSize; i++)
				objectArray[i] = decoratedSerializer.Read(source);

			return objectArray;
		}

		public void Write(TObjectType[] value, IWireMemberWriterStrategy dest)
		{
			if (!shouldSerializeSize && size != value.Length)
				throw new InvalidOperationException($"Invalid size. Provided {nameof(TObjectType)}[] had a size mismatch with expected size: {size} and was: {value.Length}.");

			//If size is needed write the size first
			if (shouldSerializeSize)
				sharedByteSerializer.Write((byte)value.Length, dest);

			for (int i = 0; i < value.Length; i++)
				decoratedSerializer.Write(value[i], dest);
		}
	}
}
