using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Reflection;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator for array types.
	/// </summary>
	/// <typeparam name="TObjectType">The type of the object the array contains.</typeparam>
	public class ArraySerializerDecorator<TObjectType> : ITypeSerializerStrategy<TObjectType[]>
	{
		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(TObjectType[]);

		public SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// The decorated underlying element serializer.
		/// </summary>
		[NotNull]
		protected ITypeSerializerStrategy<TObjectType> decoratedSerializer { get; }

		/// <summary>
		/// The strategy that determines sizing.
		/// </summary>
		[NotNull]
		protected ICollectionSizeStrategy sizeStrategyService { get; }

		public ArraySerializerDecorator([NotNull] IGeneralSerializerProvider serializerProvider, [NotNull] ICollectionSizeStrategy sizeStrategy, SerializationContextRequirement contextReq)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} to needed to decorate was null.");

			if (sizeStrategy == null)
				throw new ArgumentNullException(nameof(sizeStrategy), $"Provided {nameof(ICollectionSizeStrategy)} to needed to decorate was null.");

			if (!Enum.IsDefined(typeof(SerializationContextRequirement), contextReq))
				throw new InvalidEnumArgumentException(nameof(contextReq), (int) contextReq, typeof(SerializationContextRequirement));

			ContextRequirement = contextReq;

			try
			{
				decoratedSerializer = serializerProvider.Get<TObjectType>();
			}
			catch (InvalidOperationException e)
			{
				throw new InvalidOperationException($"Failed to produce a serializer for Type: {typeof(TObjectType).FullName} in required {nameof(ArraySerializerDecorator<TObjectType>)}", e);
			}
			
			sizeStrategyService = sizeStrategy;
		}

		/// <inheritdoc />
		public virtual TObjectType[] Read(IWireMemberReaderStrategy source)
		{
			TObjectType[] objectArray = new TObjectType[sizeStrategyService.Size(source)];

			//TODO: Error handling
			//read as many objects as in the message (this is safe for clients. Not so safe if the client sent a message of this type)
			for (int i = 0; i < objectArray.Length; i++)
				objectArray[i] = decoratedSerializer.Read(source);

			return objectArray;
		}

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			Write((TObjectType[])value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(source);
		}

		/// <inheritdoc />
		public virtual void Write(TObjectType[] value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			int size = sizeStrategyService.Size<TObjectType[], TObjectType>(value, dest);

			if(size != value.Length)
				throw new InvalidOperationException($"Invalid size. Provided {nameof(TObjectType)}[] had a size mismatch with expected size: {size} and was: {value.Length}.");

			unchecked
			{
				for (int i = 0; i < size; i++)
					decoratedSerializer.Write(value[i], dest);
			}
		}

		public TObjectType[] Read(ref TObjectType[] obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}
	}
}
