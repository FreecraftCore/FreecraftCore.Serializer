using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.API;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Reflect.Extent;

namespace FreecraftCore.Serializer
{
	public class SerializerService : ISerializerService, ISerializerStrategyFactory
	{
		/// <summary>
		/// Indicates if the serializer is compiled.
		/// </summary>
		public bool isCompiled { get; private set; }

		/// <summary>
		/// Dictionary of known <see cref="Type"/>s and their corresponding <see cref="ITypeSerializerStrategy"/>s.
		/// </summary>
		[NotNull]
		private SerializerStrategyProvider serializerStorageService { get; }

		/// <summary>
		/// Service responsible for handling decoratable semi-complex types.
		/// </summary>
		[NotNull]
		private DefaultSerializerStrategyFactory serializerStrategyFactoryService { get; }

		//TODO: Abstract this away into a caching service
		//This was profiled and it was a major perf problem. We need to cache the least derived serializer.
		private Dictionary<Type, Type> LeastDerivedTypeCache { get; }

		private object SyncObj { get; } = new object();

		public SerializerService()
		{
			//We don't inject anything because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			serializerStorageService = new SerializerStrategyProvider();
			LeastDerivedTypeCache = new Dictionary<Type, Type>();

			ContextLookupKeyFactoryService lookupKeyFactoryService = new ContextLookupKeyFactoryService();
			//Create the decoration service
			serializerStrategyFactoryService = new DefaultSerializerStrategyFactory(SerializerDecoratorHandlerFactory.Create(serializerStorageService, lookupKeyFactoryService, this), serializerStorageService, this, lookupKeyFactoryService, serializerStorageService);

			typeof(SerializerService).GetTypeInfo().Assembly.GetTypes()
				.Where(t => t.GetTypeInfo().HasAttribute<KnownTypeSerializerAttribute>())
				.Select(t => Activator.CreateInstance(t) as ITypeSerializerStrategy)
				.ToList()
				.ForEach(s => serializerStorageService.RegisterType(s.SerializerType, s));

			RegisterPrimitiveGenericSerializer<short>();
			RegisterPrimitiveGenericSerializer<ushort>();
			RegisterPrimitiveGenericSerializer<int>();
			RegisterPrimitiveGenericSerializer<uint>();
			RegisterPrimitiveGenericSerializer<long>();
			RegisterPrimitiveGenericSerializer<ulong>();
			RegisterPrimitiveGenericSerializer<float>();
			RegisterPrimitiveGenericSerializer<double>();
			RegisterPrimitiveGenericSerializer<byte>();
			RegisterPrimitiveGenericSerializer<sbyte>();
		}

		private void RegisterPrimitiveGenericSerializer<TType>()
			where TType : struct
		{
			serializerStorageService.RegisterType(typeof(TType), new GenericTypePrimitiveSharedBufferSerializerStrategy<TType>());
		}

		public void Compile()
		{
			lock (SyncObj)
			{
				//Build the least derived cache
				foreach (Type t in serializerStorageService)
				{
					Type leastDerived = GetLeastDerivedType(t, typeof(object));

					//This was profiled as a perf problem. 40% of the time spent deserializing was spent looking up
					//the least derived type
					LeastDerivedTypeCache[t] = leastDerived;
				}	
			}

			isCompiled = true;
		}

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if a serializer is registered for the provided <see cref="Type"/>.</returns>
		public bool isTypeRegistered(Type type)
		{
			return serializerStorageService.HasSerializerFor(type);
		}

		/// <inheritdoc />
		public bool RegisterType<TTypeToRegister>()
		{
			//Ingoring all but wiretypes makes this a lot easier.
			if (typeof(TTypeToRegister).GetTypeInfo().GetCustomAttribute<WireDataContractAttribute>(true) == null)
				throw new InvalidOperationException($"Do not register any type that isn't marked with {nameof(WireDataContractAttribute)}. Only register WireDataContracts too; contained types will be registered automatically.");

			//At this point this is a class marked with [WireDataContract] so we should assume and treat it as a complex type
			ITypeSerializerStrategy<TTypeToRegister> serializer = serializerStrategyFactoryService.Create<TTypeToRegister>(new TypeBasedSerializationContext(typeof(TTypeToRegister))) as ITypeSerializerStrategy<TTypeToRegister>;

			bool result = true;

			//Check if it requires runtime linking
			if(typeof(TTypeToRegister).GetTypeInfo().HasAttribute<WireDataContractBaseLinkAttribute>())
			{
				WireDataContractBaseLinkAttribute linkAttribute = typeof(TTypeToRegister).GetTypeInfo().GetCustomAttribute<WireDataContractBaseLinkAttribute>();

				//Only link if they provided a basetype.
				//Users may call RegisterType before linking so don't throw
				if(linkAttribute.BaseType != null)
				{
					result = result && Link(linkAttribute, linkAttribute.BaseType, typeof(TTypeToRegister));
				}
			}

			//Return the serializer; callers shouldn't need it though
			return serializer != null && result;
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>([NotNull] byte[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			using (DefaultStreamReaderStrategy reader = new DefaultStreamReaderStrategy(data))
			{
				return Deserialize<TTypeToDeserializeTo>(reader);
			}
		}

		public byte[] Serialize<TTypeToSerialize>([NotNull] TTypeToSerialize data)
		{
			//Pass it to overload for custom writer
			using (DefaultStreamWriterStrategy writer = new DefaultStreamWriterStrategy())
			{
				return Serialize(data, writer);
			}
		}

		private ITypeSerializerStrategy GetLeastDerivedSerializer(Type typeToSerialize)
		{
			//If it's a primitive or an enum it doesn't have a derived serializer
			if(typeToSerialize.GetTypeInfo().IsPrimitive || typeToSerialize.GetTypeInfo().IsEnum || typeToSerialize.GetTypeInfo().IsInterface)
				return serializerStorageService.Get(typeToSerialize);

			Type t = typeToSerialize;

			//We need to switch between object and ValueType to support struct serialization
			//If it's in the cache we don't need to compute the least derived type
			t = LeastDerivedTypeCache.ContainsKey(t) ? LeastDerivedTypeCache[t] : GetLeastDerivedType(t, typeof(object));

			return serializerStorageService.Get(t);
		}

		private Type GetLeastDerivedType(Type t, Type defaultDerivedType)
		{
			//If t isn't null it has at least one base type, we need to move up the object graph if so.
			if (t != null && t != defaultDerivedType)
			{
				//Find the root type
				while (t.GetTypeInfo().BaseType != null && t.GetTypeInfo().BaseType != defaultDerivedType)
				{
					t = t.GetTypeInfo().BaseType;
				}
			}

			return t;
		}

		//Called as the fallback factory.
		/// <inheritdoc />
		public ITypeSerializerStrategy<TType> Create<TType>([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//This service acts a the default/fallback factory for serializer creation. If any factory encounters a type
			//inside of a type that it doesn't know about or requires handling outside of its scope it'll broadcast that
			//and we will implement more complex handling here

			ITypeSerializerStrategy<TType> strategy = serializerStrategyFactoryService.Create<TType>(context);

			//The serializer could be null; we should verify it
			if (strategy == null)
				throw new InvalidOperationException($"Failed to create serializer for Type: {context.TargetType} with Context: {context.ToString()}.");

			return strategy;
		}

		/// <inheritdoc />
		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data, IWireStreamWriterStrategy writer)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor(data.GetType()))
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if(!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			GetLeastDerivedSerializer(typeof(TTypeToSerialize).GetTypeInfo().IsInterface ? typeof(TTypeToSerialize) : data.GetType()).Write(data, writer);

			return writer.GetBytes();
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToDeserializeTo>())
				throw new InvalidOperationException($"Serializer cannot deserialize to Type: {typeof(TTypeToDeserializeTo).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot deserialize before compiling the serializer.");

			//TODO: Cache isInterface
			TTypeToDeserializeTo deserializeObject = (TTypeToDeserializeTo)GetLeastDerivedSerializer(typeof(TTypeToDeserializeTo)).Read(source);

			return deserializeObject;
		}

		/// <inheritdoc />
		public bool Link<TChildType, TBaseType>() 
			where TChildType : TBaseType
		{
			WireDataContractBaseLinkAttribute linkAttribute =
				typeof(TChildType).GetTypeInfo().GetCustomAttribute<WireDataContractBaseLinkAttribute>(false);

			//Have to make sure link is valid
			//Without link attribute we can't know how to register it.
			if(linkAttribute == null)
				throw new InvalidOperationException($"Tried to link Child: {typeof(TChildType).FullName} with Base: {typeof(TBaseType).FullName} but no {nameof(WireDataContractBaseLinkAttribute)} attribute is marked on child.");

			RegisterType<TChildType>();

			return Link(linkAttribute, typeof(TBaseType), typeof(TChildType));
		}

		private bool Link(WireDataContractBaseLinkAttribute linkAttribute, Type baseType, Type childType)
		{
			if(serializerStorageService.HasSerializerFor(baseType))
			{
				try
				{
					(serializerStorageService.Get(baseType) as IRuntimePolymorphicRegisterable)
						.TryLink(childType, linkAttribute.Index);

					return true;
				}
				catch(Exception e)
				{
					throw new InvalidOperationException($"Unable to link type {baseType.Name} and {childType.Name}.", e);
				}
			}

			return false;
		}

		/// <inheritdoc />
		public async Task<byte[]> SerializeAsync<TTypeToSerialize>(TTypeToSerialize data)
		{
			using (DefaultStreamWriterStrategyAsync asyncWriter = new DefaultStreamWriterStrategyAsync())
			{
				return await SerializeAsync(data, asyncWriter);
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> SerializeAsync<TTypeToSerialize>(TTypeToSerialize data, IWireStreamWriterStrategyAsync writer)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor(data.GetType()))
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			//TODO: Cache IsInterface
			await GetLeastDerivedSerializer(typeof(TTypeToSerialize).GetTypeInfo().IsInterface ? typeof(TTypeToSerialize) : data.GetType()).WriteAsync(data, writer);

			return await writer.GetBytesAsync();
		}

		/// <inheritdoc />
		public async Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>(byte[] data)
		{
			using (DefaultStreamReaderStrategyAsync asyncReader = new DefaultStreamReaderStrategyAsync(data))
			{
				return await DeserializeAsync<TTypeToDeserializeTo>(asyncReader);
			}
		}

		/// <inheritdoc />
		public async Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToDeserializeTo>())
				throw new InvalidOperationException($"Serializer cannot deserialize to Type: {typeof(TTypeToDeserializeTo).FullName} because it's not registered.");
#endif
			if (!isCompiled)
				throw new InvalidOperationException($"You cannot deserialize before compiling the serializer.");

			return (TTypeToDeserializeTo)await GetLeastDerivedSerializer(typeof(TTypeToDeserializeTo)).ReadAsync(source);
		}
	}
}
