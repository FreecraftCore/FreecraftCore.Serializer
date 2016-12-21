using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// The decorated underlying element serializer.
		/// </summary>
		protected ITypeSerializerStrategy<TObjectType> decoratedSerializer { get; }

		/// <summary>
		/// The strategy that determines sizing.
		/// </summary>
		protected ICollectionSizeStrategy sizeStrategyService { get; }

		public ArraySerializerDecorator(IGeneralSerializerProvider serializerProvider, ICollectionSizeStrategy sizeStrategy, SerializationContextRequirement contextReq)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} to needed to decorate was null.");

			if (sizeStrategy == null)
				throw new ArgumentNullException(nameof(sizeStrategy), $"Provided {nameof(ICollectionSizeStrategy)} to needed to decorate was null.");

			ContextRequirement = contextReq;
			decoratedSerializer = serializerProvider.Get<TObjectType>();
			sizeStrategyService = sizeStrategy;
		}

		public virtual TObjectType[] Read(IWireMemberReaderStrategy source)
		{
			TObjectType[] objectArray = new TObjectType[sizeStrategyService.Size(source)];

			//TODO: Error handling
			//read as many objects as in the message (this is safe for clients. Not so safe if the client sent a message of this type)
			for (int i = 0; i < objectArray.Length; i++)
				objectArray[i] = decoratedSerializer.Read(source);

			return objectArray;
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TObjectType[])value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public virtual void Write(TObjectType[] value, IWireMemberWriterStrategy dest)
		{
			int size = sizeStrategyService.Size<TObjectType[], TObjectType>(value, dest);

			if(size != value.Length)
				throw new InvalidOperationException($"Invalid size. Provided {nameof(TObjectType)}[] had a size mismatch with expected size: {size} and was: {value.Length}.");

			unchecked
			{
				for (int i = 0; i < size; i++)
					decoratedSerializer.Write(value[i], dest);
			}
		}
	}
}
