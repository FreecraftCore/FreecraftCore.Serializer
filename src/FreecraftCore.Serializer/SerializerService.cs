using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
		/// The serializer provider service.
		/// </summary>
		IGeneralSerializerProvider serializerProvider { get; }

		/// <summary>
		/// Dictionary of known <see cref="Type"/>s and their corresponding <see cref="ITypeSerializerStrategy"/>s.
		/// </summary>
		IDictionary<Type, ITypeSerializerStrategy> knownMappedSerializers { get; }

		/// <summary>
		/// Service responsible for handling complex types.
		/// </summary>
		IComplexSerializerFactory complexTypeFactoryService { get; }

		/// <summary>
		/// Service responsible for handling decoratable semi-complex types.
		/// </summary>
		IDecoratedSerializerFactory decoratorFactoryService { get; }

		public SerializerService()
		{
			//We don't inject anything because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service
			knownMappedSerializers = new Dictionary<Type, ITypeSerializerStrategy>();
			serializerProvider = new DefaultGeneralSerializerProvider(new ReadOnlyDictionary<Type, ITypeSerializerStrategy>(knownMappedSerializers));

			//TODO: Cleanup creation
			complexTypeFactoryService = new DefaultComplexSerializerFactory(serializerProvider, this);
			decoratorFactoryService = new DefaultSerializerDecoratorService(FreecraftCoreSerializerKnownTypesDecoratorMetadata.Assembly.GetTypes()
				.Where(t => t.HasAttribute<DecoratorHandlerAttribute>())
				.Select(t => t.CreateInstance(serializerProvider) as DectoratorHandler), serializerProvider);

			//Subscribe to unknown type broadcasts
			complexTypeFactoryService.OnFoundUnknownAssociatedType += ctx => ((ISerializerStrategyFactory)this).Create(ctx);
			decoratorFactoryService.OnFoundUnknownAssociatedType += ctx => ((ISerializerStrategyFactory)this).Create(ctx);

			//TODO: Cleanup. Error handling
			foreach (Type t in FreecraftCoreSerializerKnownTypesPrimitivesMetadata.Assembly.GetTypes().Where(t => t.HasAttribute<KnownTypeSerializerAttribute>()))
			{
				ITypeSerializerStrategy strategy = t.CreateInstance() as ITypeSerializerStrategy;
				knownMappedSerializers.Add(strategy.SerializerType, strategy);
			}
		}

		public void Compile()
		{
			isCompiled = true;
		}

		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data) 
			where TTypeToDeserializeTo : new()
		{
			if (!serializerProvider.HasSerializerFor<TTypeToDeserializeTo>())
				throw new InvalidOperationException($"Serializer cannot deserialize to Type: {typeof(TTypeToDeserializeTo).FullName} because it's not registered.");

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot deserialize before compiling the serializer.");

			return serializerProvider.Get<TTypeToDeserializeTo>().Read(new DefaultWireMemberReaderStrategy(data));
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
				throw new InvalidOperationException($"Do not register any type that isn't marked with {nameof(WireMessageAttribute)}. Only register WireMessages too; contained types will be registered automatically.");

			//At this point this is a class marked with [WireMessage] so we should assume and treat it as a complex type
			ITypeSerializerStrategy<TTypeToRegister> serializer = complexTypeFactoryService.Create(new TypeBasedSerializationContext(typeof(TTypeToRegister))) as ITypeSerializerStrategy<TTypeToRegister>;

			//Only register contextless serializers (all complex types should be contextless)
			if (serializer.ContextRequirement == SerializationContextRequirement.Contextless)
				knownMappedSerializers.Add(typeof(TTypeToRegister), serializer);

			//Return the serializer; callers shouldn't need it though
			return serializer;
		}

		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data) 
			where TTypeToSerialize : new()
		{
			if (!serializerProvider.HasSerializerFor<TTypeToSerialize>())
				throw new InvalidOperationException($"Serializer cannot serialize Type: {typeof(TTypeToSerialize).FullName} because it's not registered.");

			if (!isCompiled)
				throw new InvalidOperationException($"You cannot serialize before compiling the serializer.");

			using (DefaultWireMemberWriterStrategy writer = new DefaultWireMemberWriterStrategy())
			{
				serializerProvider.Get<TTypeToSerialize>().Write(data, new DefaultWireMemberWriterStrategy());

				return writer.GetBytes();
			}
		}

		//Called as the fallback factory.
		ITypeSerializerStrategy ISerializerStrategyFactory.Create(ISerializableTypeContext context)
		{
			//This service acts a the default/fallback factory for serializer creation. If any factory encounters a type
			//inside of a type that it doesn't know about or requires handling outside of its scope it'll broadcast that
			//and we will implement more complex handling here

			ITypeSerializerStrategy strategy = null;

			//First check if the type can be decorated
			if(decoratorFactoryService.RequiresDecorating(context))
			{
				strategy = decoratorFactoryService.Create(context);
			}
			else
			{
				//Then we've encountered a complex type
				strategy = complexTypeFactoryService.Create(context);
			}

			//The serializer could be null; we should verify it
			if (strategy == null)
				throw new InvalidOperationException($"Failed to create serializer for Type: {context.TargetType} with Context: {context.ToString()}.");

			//If the serializer is contextless we can register it with the general provider
			if (strategy.ContextRequirement == SerializationContextRequirement.Contextless)
				knownMappedSerializers.Add(context.TargetType, strategy);

			return strategy;
		}
	}
}
