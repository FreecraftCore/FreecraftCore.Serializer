using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using Fasterflect;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SubComplexTypeWithFlagsSerializerDecorator<TBaseType> : ITypeSerializerStrategy<TBaseType>
	{
		private class ChildKeyPair
		{
			/// <summary>
			/// Flags to check for to verify type information.
			/// </summary>
			public int Flags { get; }

			/// <summary>
			/// Child type.
			/// </summary>
			public ITypeSerializerStrategy Serializer { get; }

			public ChildKeyPair(int flags, ITypeSerializerStrategy serializer)
			{
				Flags = flags;
				Serializer = serializer;
			}
		}

		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TBaseType); } }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serializer for multiple members)
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Provides read and write stragey for child keys.
		/// </summary>
		private IChildKeyStrategy keyStrategy { get; }

		//use array for performance
		private ChildKeyPair[] serializers { get; }

		private ITypeSerializerStrategy defaultSerializer;

		public SubComplexTypeWithFlagsSerializerDecorator(IGeneralSerializerProvider serializerProvider, IChildKeyStrategy childKeyStrategy)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(serializerProvider)} service was null.");

			if (childKeyStrategy == null)
				throw new ArgumentNullException(nameof(childKeyStrategy), $"Provided {nameof(IChildKeyStrategy)} used for key read and write is null.");

			keyStrategy = childKeyStrategy;
			serializerProviderService = serializerProvider;

			defaultSerializer = typeof(TBaseType).Attribute<DefaultNoFlagsAttribute>() != null
				? serializerProviderService.Get(typeof(TBaseType).Attribute<DefaultNoFlagsAttribute>().ChildType) : null;

			//We no longer reserve 0. Sometimes type information of a child is sent as a 0 in WoW protocol. We can opt for mostly metadata market style interfaces.

			List <ChildKeyPair> pairs = new List<ChildKeyPair>();

			foreach (WireDataContractBaseTypeByFlagsAttribute waf in typeof(TBaseType).Attributes<WireDataContractBaseTypeByFlagsAttribute>())
			{
				if (!typeof(TBaseType).IsAssignableFrom(waf.ChildType))
					throw new InvalidOperationException($"Failed to register Type: {typeof(TBaseType).GetType().FullName} because a provided ChildType: {waf.ChildType.FullName} was not actually a child.");

				//TODO: Maybe add a priority system for flags that may override others?
				pairs.Add(new ChildKeyPair(waf.Flag, serializerProviderService.Get(waf.ChildType)));
			}

			serializers = pairs.ToArray();
		}

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(TBaseType value, IWireMemberWriterStrategy dest)
		{
			//Defer key writing to the key writing strategy
			
			//TODO: Foreach may be better due to many access
			for (int i = 0; i < serializers.Length; i++)
				if(serializers[i].Serializer.SerializerType == value.GetType())
				{

					//Well, what do we do? Do we just write the flag?
					//It's the best we can do I think. Otherwise they should use NoConsume
					//to better define the flag written themselves
					keyStrategy.Write(serializers[i].Flags, dest);

					//Now write
					serializers[i].Serializer.Write(value, dest);
					return;
				}

			//Well, it's probably the default type then
			if (defaultSerializer != null)
				defaultSerializer.Write(value, dest);
			else
				throw new InvalidOperationException($"Couldn't locate serializer for {value.GetType().FullName} in the {nameof(SubComplexTypeWithFlagsSerializerDecorator<TBaseType>)} service.");
		}

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		public TBaseType Read(IWireMemberReaderStrategy source)
		{
			//TODO: Handle 0 flags
			//Ask the key strategy for what flags are present
			int flags = keyStrategy.Read(source); //defer to key reader (could be int, byte or something else)

			for (int i = 0; i < serializers.Length; i++)
			{
				if ((serializers[i].Flags & flags) != 0)
					return (TBaseType)serializers[i].Serializer.Read(source);
			}

			//If we didn't find a flags for it then try the default
			if (defaultSerializer != null)
				return (TBaseType)defaultSerializer.Read(source);
			else
				throw new InvalidOperationException($"{this.GetType()} attempted to deserialize to a child type with Flags: {flags} but no valid type matches and there is no default type.");
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
