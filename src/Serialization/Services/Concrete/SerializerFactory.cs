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
		IReadOnlyDictionary<Type, ITypeSerializerStrategy> knownSerializableTypeSerializers { get; }

		public SerializerFactory(IReadOnlyDictionary<Type, ITypeSerializerStrategy> knownTypes)
		{
			if (knownTypes == null)
				throw new ArgumentNullException(nameof(knownTypes), $"Provided collection of known types {nameof(knownTypes)} must be non-null.");


			knownSerializableTypeSerializers = knownTypes;
		}

		public ITypeSerializerStrategy Create(Type forType)
		{
			return knownSerializableTypeSerializers[forType];
		}
	}
}
