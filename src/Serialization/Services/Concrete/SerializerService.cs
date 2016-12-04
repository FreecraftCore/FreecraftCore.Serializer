using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace FreecraftCore.Payload.Serializer
{
	public class SerializerService : ISerializerService
	{
		/// <summary>
		/// Efficient cache map for serializers based on Type.
		/// </summary>
		private IDictionary<Type, ITypeSerializerStrategy> serializerMap { get; set; }

		private IComplexTypeRegistry complexRegistry { get; }

		// roslyn automatically implemented properties, in particular for get-only properties: <{Name}>k__BackingField;
		//var backingFieldName = $"<{property.Name}>k__BackingField";

		bool isCompiled { get; set; }

		public SerializerService()
		{
			//We don't inject knowns because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			serializerMap = new Dictionary<Type, ITypeSerializerStrategy>();

			foreach(ITypeSerializerStrategy knownSerializer in GetType().Assembly.GetTypes()
				.Where(t => t.GetCustomAttribute<KnownTypeSerializerAttribute>() != null)
				.Select(t => Activator.CreateInstance(t) as ITypeSerializerStrategy))
			{
				serializerMap.Add(knownSerializer.SerializerType, knownSerializer);
			}

			complexRegistry = new DefaultComplexTypeRegistry(serializerMap);
		}

		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data)
			where TTypeToDeserializeTo : new()
		{
			if (!isCompiled)
				throw new InvalidOperationException($"Cannot use the service until it's compiled.");

			if (data == null)
				throw new ArgumentNullException(nameof(data), $"Provided bytes {nameof(data)} must not be null.");

			if (!isTypeRegistered(typeof(TTypeToDeserializeTo)))
				throw new InvalidOperationException($"Tried to deserialize Type: {typeof(TTypeToDeserializeTo).FullName} but the type is not known by the serializer service.");

			//TODO: Error handling
			using (var reader = new DefaultWireMemberReaderStrategy(data))
			{
				//TODO: Null checking
				return ((ITypeSerializerStrategy<TTypeToDeserializeTo>)serializerMap[typeof(TTypeToDeserializeTo)]).Read(reader);
			}
		}

		public bool isTypeRegistered(Type type)
		{
			//if not built we don't know any type
			if (serializerMap == null)
				return false;

			return serializerMap.ContainsKey(type);
		}

		public bool RegisterType<TTypeToRegister>()
			where TTypeToRegister : new()
		{
			if (isCompiled)
				throw new InvalidOperationException($"Cannot register new types after the service has been compiled");

			return complexRegistry.RegisterType<TTypeToRegister>();
		}

		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
			where TTypeToSerialize : new()
		{
			if (!isCompiled)
				throw new InvalidOperationException($"Cannot use the service until it's compiled.");

			if (!isTypeRegistered(typeof(TTypeToSerialize)))
				throw new InvalidOperationException($"Tried to serialize Type: {typeof(TTypeToSerialize).FullName} but the type is not known by the serializer service.");

			//TODO: Error handling
			using (var writer = new DefaultWireMemberWriterStrategy())
			{
				((ITypeSerializerStrategy<TTypeToSerialize>)serializerMap[typeof(TTypeToSerialize)]).Write(data, writer);

				return writer.GetBytes();
			}	
		}

		public void Compile()
		{
			isCompiled = true;
		}
	}
}
