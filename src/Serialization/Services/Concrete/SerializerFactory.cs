using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Factory service that produces serializers.
	/// </summary>
	public class SerializerFactory : ISerializerFactory
	{
		/// <summary>
		/// Known serializers that the factory can produce.
		/// </summary>
		IEnumerable<ITypeSerializerStrategy> knownSerializableTypeSerializers { get; }

		/// <summary>
		/// Service that decorators services extending their compatbility with "nearly" known types.
		/// </summary>
		ISerializerDecoratorService serializerDecoratorService { get; }

		public SerializerFactory(IEnumerable<ITypeSerializerStrategy> knownTypes, ISerializerDecoratorService decoratorService)
		{
			if (knownTypes == null)
				throw new ArgumentNullException(nameof(knownTypes), $"Provided collection of known types {nameof(knownTypes)} must be non-null.");

			if (decoratorService == null)
				throw new ArgumentNullException(nameof(decoratorService), $"Provided service {nameof(ISerializerDecoratorService)} must not be null.");

			knownSerializableTypeSerializers = knownTypes;
			serializerDecoratorService = decoratorService;
		}

		public ITypeSerializerStrategy Create(Type forType)
		{
			//If the type requires an advanced serializer (a decorated one) we should ask the decorator service.
			if(serializerDecoratorService.RequiresDecorating(forType))
			{
				//If it's null there are big issues
				return serializerDecoratorService.TryProduceDecoratedSerializer(forType, new SerializerDecorationContext(forType.Attributes<Attribute>(), knownSerializableTypeSerializers));
			}
			else
			{
				//Currently we don't handle base/interface types. The provided type MUST be directly defined in a serializer.
				return knownSerializableTypeSerializers.First(x => x.SerializerType == forType);
			}
		}
	}
}
