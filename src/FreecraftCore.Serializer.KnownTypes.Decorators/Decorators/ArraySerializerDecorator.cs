using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator for array types.
	/// </summary>
	/// <typeparam name="TObjectType">The type of the object the array contains.</typeparam>
	public class ArraySerializerDecorator<TObjectType> : ITypeSerializerStrategy<TObjectType[]>
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TObjectType); } }

		//TODO: Make it so that reuse of serializer for this basetype is possible. Context requirement needs to be handled better.
		public virtual SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		private ITypeSerializerStrategy<TObjectType> decoratedSerializer { get; }

		public ArraySerializerDecorator(ISerializerProvider serializerProvider)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(ISerializerProvider)} to needed to decorate was null.");

			decoratedSerializer = serializerProvider.Get<TObjectType>();
		}

		public TObjectType[] Read(IWireMemberReaderStrategy source)
		{
			TObjectType[] objectArray = new TObjectType[GetCollectionSize(source)];

			//TODO: Error handling
			//read as many objects as in the message (this is safe for clients. Not so safe if the client sent a message of this type)
			for (int i = 0; i < objectArray.Length; i++)
				objectArray[i] = decoratedSerializer.Read(source);

			return objectArray;
		}

		protected virtual byte GetCollectionSize(IWireMemberReaderStrategy source)
		{
			//As far as I known TC/WoW writes collection sizes as a uint8 (byte) so that is what we'll do
			return source.ReadByte(); //read first byte for collection size
		}
		
		protected virtual byte SetCollectionSize(IWireMemberWriterStrategy dest, byte size)
		{
			//If size is needed write the size first
			dest.Write((byte)size);

			return size;
		}

		public void Write(TObjectType[] value, IWireMemberWriterStrategy dest)
		{
			byte size = SetCollectionSize(dest, (byte)value.Length);

			if(size != value.Length)
				throw new InvalidOperationException($"Invalid size. Provided {nameof(TObjectType)}[] had a size mismatch with expected size: {size} and was: {value.Length}.");

			for (int i = 0; i < size; i++)
				decoratedSerializer.Write(value[i], dest);
		}
	}
}
