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

			DefaultSerializer = typeof(TBaseType).GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>() != null
				? serializerProviderService.Get(typeof(TBaseType).GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>().ChildType)
				: null;

			//We no longer reserve 0. Sometimes type information of a child is sent as a 0 in WoW protocol. We can opt for mostly metadata marker style interfaces.
			//TODO: Add support for basetype serialization metadata marking.
			foreach (WireDataContractBaseTypeAttribute wa in typeof(TBaseType).GetTypeInfo().GetCustomAttributes<WireDataContractBaseTypeAttribute>())
			{
				RegisterPair(wa.ChildType, wa.Index);
			}
		}

		/// <inheritdoc />
		public override void Write(TBaseType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			if (value == null)
				throw new InvalidOperationException($"Serializes a null {typeof(TBaseType).FullName} is not a supported serialization scenario. It is impossible to know which type to encode.");


			//TODO: Clean up default serializer implementation
			if (!typeToKeyLookup.ContainsKey(value.GetType()))
			{
				throw new InvalidOperationException($"{this.GetType()} attempted to serialize a child Type: {value.GetType()} but no valid type matches. Writing cannot use default types.");
			}

			//TODO: Oh man, this is a disaster. How do we handle the default? How do we tell consumers to use the default?
			//Defer key writing to the key writing strategy
			keyStrategy.Write(typeToKeyLookup[value.GetType()], dest);

			ITypeSerializerStrategy serializer;

			try
			{
				serializer = serializerProviderService.Get(value.GetType());

			}
			catch (KeyNotFoundException e)
			{
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().FullName} in the {nameof(IGeneralSerializerProvider)} service.", e);
			}

			serializer.Write(value, dest);
		}

		/// <inheritdoc />
		public override TBaseType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			int childIndexRequested = keyStrategy.Read(source); //defer to key reader (could be int, byte or something else)

			//Check if we have that index; if not use default
			if (!keyToTypeLookup.ContainsKey(childIndexRequested))
			{
				if (DefaultSerializer != null)
				{
					return (TBaseType)DefaultSerializer.Read(source);
				}
				else
					throw new InvalidOperationException($"{this.GetType()} attempted to deserialize a child of Type: {typeof(TBaseType).FullName} with Key: {childIndexRequested} but no valid type matches and there is no default type.");
			}

			Type childTypeRequest = keyToTypeLookup[childIndexRequested];

			if(childTypeRequest == null)
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the lookup table provided a null type. This may indicate a failure in registeration of child types.");


			//Once we know which child this particular object should be
			//we need to dispatch the read request to that child's serializer handler
			//and if it happens to map to another child, which should be rare, it'll dispatch until it reaches a ComplexType serializer which is where
			//the end of the inheritance graph tree should end up. The complextype serializer, which is the true type serializer, should handle deserialization
			//include going up the base heriachiry.
			return (TBaseType)serializerProviderService.Get(childTypeRequest).Read(source);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TBaseType value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Clean up default serializer implementation
			if (!typeToKeyLookup.ContainsKey(value.GetType()))
			{
				throw new InvalidOperationException($"{this.GetType()} attempted to serialize a child Type: {value.GetType()} but no valid type matches. Writing cannot use default types.");
			}

			//TODO: Oh man, this is a disaster. How do we handle the default? How do we tell consumers to use the default?
			//Defer key writing to the key writing strategy
			await keyStrategy.WriteAsync(typeToKeyLookup[value.GetType()], dest);

			ITypeSerializerStrategy serializer;

			try
			{
				serializer = serializerProviderService.Get(value.GetType());

			}
			catch (KeyNotFoundException e)
			{
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().FullName} in the {nameof(IGeneralSerializerProvider)} service.", e);
			}

			await serializer.WriteAsync(value, dest);
		}

		/// <inheritdoc />
		public override async Task<TBaseType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			int childIndexRequested = await keyStrategy.ReadAsync(source);

			//Check if we have that index; if not use default
			if (!keyToTypeLookup.ContainsKey(childIndexRequested))
			{
				if (DefaultSerializer != null)
				{
					return (TBaseType)await DefaultSerializer.ReadAsync(source);
				}
				else
					throw new InvalidOperationException($"{this.GetType()} attempted to deserialize a child of Type: {typeof(TBaseType).FullName} with Key: {childIndexRequested} but no valid type matches and there is no default type.");
			}

			Type childTypeRequest = keyToTypeLookup[childIndexRequested];

			if (childTypeRequest == null)
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the lookup table provided a null type. This may indicate a failure in registeration of child types.");


			//Once we know which child this particular object should be
			//we need to dispatch the read request to that child's serializer handler
			//and if it happens to map to another child, which should be rare, it'll dispatch until it reaches a ComplexType serializer which is where
			//the end of the inheritance graph tree should end up. The complextype serializer, which is the true type serializer, should handle deserialization
			//include going up the base heriachiry.
			return (TBaseType)await serializerProviderService.Get(childTypeRequest).ReadAsync(source);
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
			RegisterPair(typeof(TChildType), key);

			return true;
		}
	}
}
