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
			serializerStrategyFactoryService = new DefaultSerializerStrategyFactory(SerializerDecoratorHandlerFactory.Create(serializerStorageService, lookupKeyFactoryService, this), serializerStorageService, this, lookupKeyFactoryService);

			FreecraftCoreSerializerKnownTypesPrimitivesMetadata.Assembly.GetTypes()
				.Where(t => t.GetTypeInfo().HasAttribute<KnownTypeSerializerAttribute>())
				.Select(t => Activator.CreateInstance(t) as ITypeSerializerStrategy)
				.ToList()
				.ForEach(s => serializerStorageService.RegisterType(s.SerializerType, s));
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

			//All types calling RegisterType are contextless complex types. Register the result as a contextless type serializer
			serializerStorageService.RegisterType(typeof(TTypeToRegister), serializer);

			//Return the serializer; callers shouldn't need it though
			return serializer != null;
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

		private ITypeSerializerStrategy GetLeastDerivedSerializer<TType>()
		{
			//If it's a primitive or an enum it doesn't have a derived serializer
			if(typeof(TType).GetTypeInfo().IsPrimitive || typeof(TType).GetTypeInfo().IsEnum)
				return serializerStorageService.Get<TType>();

			Type t = typeof(TType);

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

			//If the serializer is contextless we can register it with the general provider
			if (strategy.ContextRequirement == SerializationContextRequirement.Contextless)
				serializerStorageService.RegisterType(context.TargetType, strategy);
			else
			{
				//TODO: Clean this up
				if (context.HasContextualKey())
				{
					//Register the serializer with the context key that was built into the serialization context.
					serializerStorageService.RegisterType(context.BuiltContextKey.Value.ContextFlags, context.BuiltContextKey.Value.ContextSpecificKey, context.TargetType, strategy);
				}
				else
					throw new InvalidOperationException($"Serializer was created but Type: {context.TargetType} came with no contextual key in the end for a contextful serialization context.");
			}

			return strategy;
		}

		/// <inheritdoc />
		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data, IWireStreamWriterStrategy writer)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToSerialize>())
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			GetLeastDerivedSerializer<TTypeToSerialize>().Write(data, writer);

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

			TTypeToDeserializeTo deserializeObject = (TTypeToDeserializeTo)GetLeastDerivedSerializer<TTypeToDeserializeTo>().Read(source);

			return deserializeObject;
		}

		/// <inheritdoc />
		public bool Link<TChildType, TBaseType>() 
			where TChildType : TBaseType
		{
			WireDataContractBaseTypeRuntimeLinkAttribute linkAttribute =
				typeof(TChildType).GetTypeInfo().GetCustomAttribute<WireDataContractBaseTypeRuntimeLinkAttribute>(false);

			//Have to make sure link is valid
			//Without link attribute we can't know how to register it.
			if(linkAttribute == null)
				throw new InvalidOperationException($"Tried to link Child: {typeof(TChildType).FullName} with Base: {typeof(TBaseType).FullName} but no {nameof(WireDataContractBaseTypeRuntimeLinkAttribute)} attribute is marked on child.");

			//Ensure both are registered
			RegisterType<TBaseType>();
			RegisterType<TChildType>();

			ITypeSerializerStrategy<TBaseType> baseTypeSerializerStrategy = serializerStorageService.Get<TBaseType>();

			//This is bad design but we just try to see if it's a polymorphic runtime linker
			//If not we can't register the type.
			IRuntimePolymorphicRegisterable<TBaseType> strategy = baseTypeSerializerStrategy as IRuntimePolymorphicRegisterable<TBaseType>;
			if (strategy != null)
			{
				IRuntimePolymorphicRegisterable<TBaseType> linker = strategy;
				strategy.TryLink<TChildType>(linkAttribute.Index);
				return true;
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
			if (!serializerStorageService.HasSerializerFor<TTypeToSerialize>())
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			await GetLeastDerivedSerializer<TTypeToSerialize>().WriteAsync(data, writer);

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

			return (TTypeToDeserializeTo)await GetLeastDerivedSerializer<TTypeToDeserializeTo>().ReadAsync(source);
		}
	}
}
