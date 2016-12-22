using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using Fasterflect;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SubComplexTypeSerializerDecorator<TBaseType> : ITypeSerializerStrategy<TBaseType>
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TBaseType); } }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		private ITypeSerializerStrategy<TBaseType> baseSerializerStrategy { get; }

		/// <summary>
		/// The lookup table that maps ints to the Type.
		/// </summary>
		private IDictionary<int, Type> keyToTypeLookup { get; }

		/// <summary>
		/// The lookup table that maps types to an int.
		/// </summary>
		private IDictionary<Type, int> typeToKeyLookup { get; }

		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serializer for multiple members)
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Provides read and write stragey for child keys.
		/// </summary>
		private IChildKeyStrategy keyStrategy { get; }

		public SubComplexTypeSerializerDecorator(IGeneralSerializerProvider serializerProvider, IChildKeyStrategy childKeyStrategy)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(serializerProvider)} service was null.");

			if (childKeyStrategy == null)
				throw new ArgumentNullException(nameof(childKeyStrategy), $"Provided {nameof(IChildKeyStrategy)} used for key read and write is null.");

			keyStrategy = childKeyStrategy;
			serializerProviderService = serializerProvider;
			typeToKeyLookup = new Dictionary<Type, int>();
			keyToTypeLookup = new Dictionary<int, Type>();

			//We no longer reserve 0. Sometimes type information of a child is sent as a 0 in WoW protocol. We can opt for mostly metadata market style interfaces.

			foreach(WireDataContractBaseTypeAttribute wa in typeof(TBaseType).Attributes<WireDataContractBaseTypeAttribute>())
			{
				try
				{
					keyToTypeLookup.Add(wa.Index, wa.ChildType);
					typeToKeyLookup.Add(wa.ChildType, wa.Index);
				}
				catch(ArgumentException e)
				{
					throw new InvalidOperationException($"Failed to register child Type: {wa.ChildType} for BaseType: {typeof(TBaseType).FullName} due to likely duplicate key index for {wa.Index}. Index must be unique per Type.", e);
				}
			}
		}

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(TBaseType value, IWireMemberWriterStrategy dest)
		{
			if (value == null)
				throw new Exception();

			if (!typeToKeyLookup.ContainsKey(value.GetType()))
				throw new InvalidOperationException($"Cannot serialize Type: {value.GetType()} in {this.GetType().FullName}.");

			//Defer key writing to the key writing strategy
			keyStrategy.Write(typeToKeyLookup[value.GetType()], dest);

			ITypeSerializerStrategy serializer = serializerProviderService.Get(value.GetType());

			if (serializer == null)
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().FullName} in the {nameof(IGeneralSerializerProvider)} service.");

			serializer.Write(value, dest);
		}

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		public TBaseType Read(IWireMemberReaderStrategy source)
		{
			//Incoming should be a byte that indicates the child type to use
			//Read it to lookup in the map to determine which type we should create
			int childIndexRequested = keyStrategy.Read(source); //defer to key reader (could be int, byte or something else)

			//Check if we have that index
			if (!keyToTypeLookup.ContainsKey(childIndexRequested))
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the index didn't exist in the lookup table. Check the Type: {typeof(TBaseType).FullName} attributes for duplicate index.");

			Type childTypeRequest = keyToTypeLookup[childIndexRequested];

			if(childTypeRequest == null)
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Index: {childIndexRequested} but the lookup table provided a null type. This may indicate a failure in registeration of child types.");

			return (TBaseType)serializerProviderService.Get(childTypeRequest).Read(source);
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TBaseType)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
