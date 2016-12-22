using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;
using FreecraftCore.Serializer.API;

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
		private SerializerStrategyProvider serializerStorageService { get; }

		/// <summary>
		/// Service responsible for handling decoratable semi-complex types.
		/// </summary>
		DefaultSerializerStrategyFactory serializerStrategyFactoryService { get; }

		public SerializerService()
		{
			//We don't inject anything because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			serializerStorageService = new SerializerStrategyProvider();

			//Create the decoration service
			serializerStrategyFactoryService = new DefaultSerializerStrategyFactory(SerializerDecoratorHandlerFactory.Create(serializerStorageService, new ContextLookupKeyFactoryService(), this), serializerStorageService, this);

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

		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data)
		{
			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToDeserializeTo>())
				throw new InvalidOperationException($"Serializer cannot deserialize to Type: {typeof(TTypeToDeserializeTo).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot deserialize before compiling the serializer.");

			return serializerStorageService.Get<TTypeToDeserializeTo>().Read(new DefaultWireMemberReaderStrategy(data));
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

		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
		{
			//Conditional compile this because it's not really very efficient anymore to lookup if a type is serialized.
#if DEBUG || DEBUGBUILD
			if (!serializerStorageService.HasSerializerFor<TTypeToSerialize>())
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");
#endif

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			using (DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy())
			{
				serializerStorageService.Get<TTypeToSerialize>().Write(data, writer);

				return writer.GetBytes();
			}
		}

		//Called as the fallback factory.
		public ITypeSerializerStrategy<TType> Create<TType>(ISerializableTypeContext context)
		{
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
