using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public class SerializerService : ISerializerService
	{
		/// <summary>
		/// Indicates if the serializer is compiled.
		/// </summary>
		public bool isCompiled { get; private set; }

		/// <summary>
		/// The serializer provider service.
		/// </summary>
		ISerializerProvider serializerProvider { get; }

		/// <summary>
		/// Dictionary of known <see cref="Type"/>s and their corresponding <see cref="ITypeSerializerStrategy"/>s.
		/// </summary>
		IDictionary<Type, ITypeSerializerStrategy> knownMappedSerializers { get; }

		/// <summary>
		/// Service responsible for handling complex types.
		/// </summary>
		IComplexTypeRegistry complexTypeRegisteryService { get; }

		/// <summary>
		/// Service responsible for handling decoratable semi-complex types.
		/// </summary>
		ISerializerDecoratorService decoratorRegistryService { get; }

		public SerializerService()
		{
			//We don't inject anything because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			knownMappedSerializers = new Dictionary<Type, ITypeSerializerStrategy>();
			serializerProvider = new DefaultSerializerProvider(new ReadOnlyDictionary<Type, ITypeSerializerStrategy>(knownMappedSerializers));

			complexTypeRegisteryService = new DefaultComplexTypeRegistry(serializerProvider);
			//TODO: Register all primitives
			//TODO: Gather all decorator handlers
		}

		public void Compile()
		{
			isCompiled = true;
		}

		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data) 
			where TTypeToDeserializeTo : new()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if a serializer is registered for the provided <see cref="Type"/>.</returns>
		public bool isTypeRegistered(Type type)
		{
			return serializerProvider.HasSerializerFor(type);
		}

		public ITypeSerializerStrategy<TTypeToRegister> RegisterType<TTypeToRegister>() 
			where TTypeToRegister : new()
		{
			//Ingoring all but wiretypes makes this a lot easier.
			if (typeof(TTypeToRegister).GetCustomAttribute<WireMessageAttribute>() == null)
				throw new InvalidOperationException($"Do not register any type that isn't marked with {nameof(WireMessageAttribute)}.");

			//At this point this is a class marked with [WireMessage] so we should assume and treat it as a complex type
			ITypeSerializerStrategy serializer = complexTypeRegisteryService.RegisterType<TTypeToRegister>();

			//Only register contextless serializers (call complex types should be contextless)
			if (serializer.ContextRequirement == SerializationContextRequirement.Contextless)
				knownMappedSerializers.Add(typeof(TTypeToRegister), serializer);
		}

		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data) 
			where TTypeToSerialize : new()
		{
			throw new NotImplementedException();
		}

		/*private IComplexTypeRegistry complexRegistry { get; }

		// roslyn automatically implemented properties, in particular for get-only properties: <{Name}>k__BackingField;
		//var backingFieldName = $"<{property.Name}>k__BackingField";

		

		public SerializerService()
		{
			
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
		}*/
	}
}
