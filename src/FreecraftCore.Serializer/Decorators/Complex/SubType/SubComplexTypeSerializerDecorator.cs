using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SubComplexTypeSerializerDecorator<TBaseType> : SubComplexTypeSerializer<TBaseType>, IRuntimePolymorphicRegisterable<TBaseType>
	{
		/// <summary>
		/// The lookup table that maps ints to the Type.
		/// </summary>
		[NotNull]
		private IDictionary<int, Type> keyToTypeLookup { get; }

		/// <summary>
		/// The lookup table that maps types to an int.
		/// </summary>
		[NotNull]
		private IDictionary<Type, int> typeToKeyLookup { get; }

		/// <summary>
		/// Provides read and write stragey for child keys.
		/// </summary>
		[NotNull]
		private IChildKeyStrategy keyStrategy { get; }

		//TODO: Reimplement default type handling cleanly
		[CanBeNull]
		public ITypeSerializerStrategy DefaultSerializer { get; }

		//This exists for cases where the type is non-abstract and we need to serialize as if we weren't a subtype.
		private ComplexTypeSerializerDecorator<TBaseType> InternallyManagedComplexSerializer { get; }

		public SubComplexTypeSerializerDecorator([NotNull] IDeserializationPrototypeFactory<TBaseType> prototypeGenerator,
			[NotNull] IEnumerable<IMemberSerializationMediator<TBaseType>> serializationDirections,
			[NotNull] IGeneralSerializerProvider serializerProvider, [NotNull] IChildKeyStrategy childKeyStrategy)
			: base(prototypeGenerator, serializationDirections, serializerProvider)
		{
			if (childKeyStrategy == null)
				throw new ArgumentNullException(nameof(childKeyStrategy),
					$"Provided {nameof(IChildKeyStrategy)} used for key read and write is null.");

			keyStrategy = childKeyStrategy;
			typeToKeyLookup = new Dictionary<Type, int>();
			keyToTypeLookup = new Dictionary<int, Type>();

			DefaultSerializer = typeof(TBaseType).GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>(false) != null
				? serializerProviderService.Get(typeof(TBaseType).GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>(false).ChildType)
				: null;

			//We no longer reserve 0. Sometimes type information of a child is sent as a 0 in WoW protocol. We can opt for mostly metadata marker style interfaces.
			//TODO: Add support for basetype serialization metadata marking.
			foreach (WireDataContractBaseTypeAttribute wa in typeof(TBaseType).GetTypeInfo().GetCustomAttributes<WireDataContractBaseTypeAttribute>(false))
			{
				if(wa.Index == keyStrategy.DefaultKey)
					throw new InvalidOperationException($"Encountered reserved BaseType Key: {wa.Index} on Type: {wa.ChildType.Name}. This key value is reserved for internal handling.");

				RegisterPair(wa.ChildType, wa.Index);
			}

			//If we're not abstract we may be asked to serialize ourselves
			if(!typeof(TBaseType).GetTypeInfo().IsAbstract)
				InternallyManagedComplexSerializer = new ComplexTypeSerializerDecorator<TBaseType>(serializationDirections, prototypeGenerator, serializerProvider);
		}

		/// <inheritdoc />
		public override void Write(TBaseType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			if (value == null)
				throw new InvalidOperationException($"Serializing a null {typeof(TBaseType).FullName} is not a supported serialization scenario. It is impossible to know which type to encode.");

			Type childType = value.GetType();

			//If the actual type is just this type then we should handle serialization as if we
			//were a regular complex type
			if(childType == typeof(TBaseType))
			{
				//We know the complex serializer won't be null because it HAS to be non-abstract for this to 
				//have been true
				keyStrategy.WriteDefault(dest);
				InternallyManagedComplexSerializer.Write(value, dest);
				return;
			}

			ITypeSerializerStrategy serializer = GetWriteSerializer(value, ref childType);

			if(serializer == null || childType == null || !typeToKeyLookup.ContainsKey(childType))
				throw new InvalidOperationException($"Unable to find serializer for Type: {typeof(TBaseType).Name} looking for Subtype: {childType.Name}");

			//TODO: Oh man, this is a disaster. How do we handle the default? How do we tell consumers to use the default?
			//Defer key writing to the key writing strategy
			keyStrategy.Write(typeToKeyLookup[childType], dest);
			serializer.Write(value, dest);
		}

		/// <inheritdoc />
		public override TBaseType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			int childIndexRequested = keyStrategy.Read(source); //defer to key reader (could be int, byte or something else)

			ITypeSerializerStrategy strategy = GetReadStrategy(childIndexRequested);

			//Once we know which child this particular object should be
			//we need to dispatch the read request to that child's serializer handler
			//and if it happens to map to another child, which should be rare, it'll dispatch until it reaches a ComplexType serializer which is where
			//the end of the inheritance graph tree should end up. The complextype serializer, which is the true type serializer, should handle deserialization
			//include going up the base heriachiry.
			return (TBaseType)strategy.Read(source);
		}

		private ITypeSerializerStrategy GetReadStrategy(int key)
		{
			Console.WriteLine($"Ley: {key} Type: {typeof(TBaseType)}");
			//If it's the reserved key self then we know we should
			//dispatch reading to the internally managed complex version of this Type.
			if(key == keyStrategy.DefaultKey)
			{
				return InternallyManagedComplexSerializer;
			}

			//Check if we have that index; if not use default
			if(!keyToTypeLookup.ContainsKey(key))
			{
				Console.WriteLine($"Not Found: {key} Type: {typeof(TBaseType).Name}");

				if(DefaultSerializer != null)
				{
					return DefaultSerializer;
				}
				else
					throw new InvalidOperationException($"{this.GetType()} attempted to deserialize a child of Type: {typeof(TBaseType).FullName} with Key: {key} but no valid type matches and there is no default type.");
			}

			Type childTypeRequest = keyToTypeLookup[key];

			if(childTypeRequest == null)
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {key} but the lookup table provided a null type. This may indicate a failure in registeration of child types.");

			//Once we know which child this particular object should be
			//we need to dispatch the read request to that child's serializer handler
			//and if it happens to map to another child, which should be rare, it'll dispatch until it reaches a ComplexType serializer which is where
			//the end of the inheritance graph tree should end up. The complextype serializer, which is the true type serializer, should handle deserialization
			//include going up the base heriachiry.
			return serializerProviderService.Get(childTypeRequest);
		}

		private ITypeSerializerStrategy GetWriteSerializer(TBaseType value, ref Type childType)
		{
			//TODO: Clean up default serializer implementation
			if(!typeToKeyLookup.ContainsKey(childType))
			{
				bool foundType = false;
				//We might be encountering a multiple level polymorphic/multiinheritance Type
				//We need to find a Type in the heirarchy that we do know
				while(childType != null && childType != typeof(object))
				{
					//Check if we know this type
					if(typeToKeyLookup.ContainsKey(childType))
					{
						foundType = true;
						break;
					}

					childType = childType.GetTypeInfo().BaseType;
				}

				if(!foundType)
					throw new InvalidOperationException($"{this.GetType()} attempted to serialize a child Type: {value.GetType()} but no valid type matches. Writing cannot use default types.");
			}

			try
			{
				return serializerProviderService.Get(childType);

			}
			catch(KeyNotFoundException e)
			{
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().Name} in the {nameof(IGeneralSerializerProvider)} service.", e);
			}
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TBaseType value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			Type childType = value.GetType();

			//If the actual type is just this type then we should handle serialization as if we
			//were a regular complex type
			if(childType == typeof(TBaseType))
			{
				//We know the complex serializer won't be null because it HAS to be non-abstract for this to 
				//have been true
				keyStrategy.WriteDefault(dest);
				InternallyManagedComplexSerializer.Write(value, dest);
				return;
			}

			ITypeSerializerStrategy serializer = GetWriteSerializer(value, ref childType);

			//TODO: Oh man, this is a disaster. How do we handle the default? How do we tell consumers to use the default?
			//Defer key writing to the key writing strategy
			await keyStrategy.WriteAsync(typeToKeyLookup[childType], dest);

			await serializer.WriteAsync(value, dest);
		}

		/// <inheritdoc />
		public override async Task<TBaseType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			int childIndexRequested = keyStrategy.Read(source); //defer to key reader (could be int, byte or something else)

			ITypeSerializerStrategy strategy = GetReadStrategy(childIndexRequested);


			//Once we know which child this particular object should be
			//we need to dispatch the read request to that child's serializer handler
			//and if it happens to map to another child, which should be rare, it'll dispatch until it reaches a ComplexType serializer which is where
			//the end of the inheritance graph tree should end up. The complextype serializer, which is the true type serializer, should handle deserialization
			//include going up the base heriachiry.
			return (TBaseType)await strategy.ReadAsync(source);
		}

		private void RegisterPair(Type child, int key)
		{
			keyToTypeLookup[key] = child;
			typeToKeyLookup[child] = key;
		}

		/// <inheritdoc />
		public bool TryLink<TChildType>(int key) 
			where TChildType : TBaseType
		{
			//Just try to register it
			return TryLink(typeof(TChildType), key);
		}

		/// <inheritdoc />
		public bool TryLink(Type childType, int key)
		{
			if(!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(childType))
				throw new InvalidOperationException($"Linked {typeof(TBaseType).Name} is not the base type of {childType.Name}.");

			//Just try to register it
			RegisterPair(childType, key);

			return true;
		}
	}
}
