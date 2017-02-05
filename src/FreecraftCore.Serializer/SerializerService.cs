using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;
using FreecraftCore.Serializer.API;
using JetBrains.Annotations;

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
		DefaultSerializerStrategyFactory serializerStrategyFactoryService { get; }

		public SerializerService()
		{
			//We don't inject anything because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			serializerStorageService = new SerializerStrategyProvider();

			ContextLookupKeyFactoryService lookupKeyFactoryService = new ContextLookupKeyFactoryService();
			//Create the decoration service
			serializerStrategyFactoryService = new DefaultSerializerStrategyFactory(SerializerDecoratorHandlerFactory.Create(serializerStorageService, lookupKeyFactoryService, this), serializerStorageService, this, lookupKeyFactoryService);

			FreecraftCoreSerializerKnownTypesPrimitivesMetadata.Assembly.GetTypes()
				.Where(t => t.HasAttribute<KnownTypeSerializerAttribute>())
				.Select(t => t.CreateInstance() as ITypeSerializerStrategy)
				.ToList()
				.ForEach(s => serializerStorageService.RegisterType(s.SerializerType, s));
		}

		public void Compile()
		{
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
		public ITypeSerializerStrategy<TTypeToRegister> RegisterType<TTypeToRegister>()
		{
			//Ingoring all but wiretypes makes this a lot easier.
			if (typeof(TTypeToRegister).GetCustomAttribute<WireDataContractAttribute>(true) == null)
				throw new InvalidOperationException($"Do not register any type that isn't marked with {nameof(WireDataContractAttribute)}. Only register WireDataContracts too; contained types will be registered automatically.");

			//At this point this is a class marked with [WireDataContract] so we should assume and treat it as a complex type
			ITypeSerializerStrategy<TTypeToRegister> serializer = serializerStrategyFactoryService.Create<TTypeToRegister>(new TypeBasedSerializationContext(typeof(TTypeToRegister))) as ITypeSerializerStrategy<TTypeToRegister>;

			//All types calling RegisterType are contextless complex types. Register the result as a contextless type serializer
			serializerStorageService.RegisterType(typeof(TTypeToRegister), serializer);

			//Return the serializer; callers shouldn't need it though
			return serializer;
		}

		/// <inheritdoc />
		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>([NotNull] byte[] data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToDeserializeTo>())
				throw new InvalidOperationException($"Serializer cannot deserialize to Type: {typeof(TTypeToDeserializeTo).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot deserialize before compiling the serializer.");

			TTypeToDeserializeTo deserializeObject = (TTypeToDeserializeTo)GetLeastDerivedSerializer<TTypeToDeserializeTo>().Read(new DefaultWireMemberReaderStrategy(data));

			//invoke after deserialization if it's available
			(deserializeObject as ISerializationEventListener)?.OnAfterDeserialization();

			return deserializeObject;
		}

		public byte[] Serialize<TTypeToSerialize>([NotNull] TTypeToSerialize data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToSerialize>())
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			//invoke after deserialization if it's available
			(data as ISerializationEventListener)?.OnBeforeSerialization();

			using (DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy())
			{
				GetLeastDerivedSerializer<TTypeToSerialize>().Write(data, writer);

				return writer.GetBytes();
			}
		}

		private ITypeSerializerStrategy GetLeastDerivedSerializer<TType>()
		{
			Type t = typeof(TType).BaseType;

			//If t isn't null it has at least one base type, we need to move up the object graph if so.
			if (t != null && t != typeof(object))
			{
				//Find the root type
				while (t.BaseType != null && t.BaseType != typeof(object))
				{
					t = t.BaseType;
				}

				return serializerStorageService.Get(t);
			}
			else
			{
				return serializerStorageService.Get<TType>();
			}
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
	}
}
