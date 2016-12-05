using Fasterflect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// The default complex serializer factory.
	/// </summary>
	public class DefaultComplexSerializerFactory : IComplexSerializerFactory
	{
		/// <summary>
		/// Serializer provider service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		/// <summary>
		/// Fallback factory for creating contextual based serializers for known types.
		/// </summary>
		private ISerializerStrategyFactory fallbackSerializerFactoryService { get; }

		/// <summary>
		/// Event that can be subscribed to for alerts on found associated types.
		/// </summary>
		public event FoundUnknownAssociatedType OnFoundUnknownAssociatedType;

		public DefaultComplexSerializerFactory(IGeneralSerializerProvider serializerProvider, ISerializerStrategyFactory fallbackSerializerFactory) //this is mutable
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} service was null.");

			//Fallback factory is for when we find a type that isn't registered and it requires context to build.
			if (fallbackSerializerFactory == null)
				throw new ArgumentNullException(nameof(fallbackSerializerFactory), $"Provided {nameof(ISerializerStrategyFactory)} service was null.");

			serializerProviderService = serializerProvider;
			fallbackSerializerFactoryService = fallbackSerializerFactory;
		}

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool isTypeRegistered(Type type)
		{
			return serializerProviderService.Get(type) != null;
		}

		/// <summary>
		/// Attempts to look at the the object graph of depth 1 from the provided <see cref="Type"/>.
		/// (Goes only 1 depth; leave the consumers of the broadcast to determine if recursive calls should be made to register entire graph)
		/// </summary>
		/// <param name="typeToParse">The <see cref="Type"/> that should be inspected.</param>
		/// <returns></returns>
		private Dictionary<int, ITypeSerializerStrategy> DiscoverContextualSerializers(Type typeToParse)
		{
			//Fine to be empty when returning. Shouldn't be null. We may find no members that require context for serialization
			Dictionary<int, ITypeSerializerStrategy> contextBasedSerializers = new Dictionary<int, ITypeSerializerStrategy>();

			//To register a type we must traverse the object graph and work our way up
			//This will cause hangs if there is a circular reference
			foreach (MemberInfo mi in typeToParse.MembersWith<WireMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility))
			{
				//If the provider knows about this type we should continue
				if (isTypeRegistered(mi.Type()))
					continue;

				//Fields/Props found on a type that are marked with WireMember attributes MAY have additional context. This context is important
				//in determining if a general serializer can handle this or if
				MemberInfoBasedSerializationContext context = new MemberInfoBasedSerializationContext(mi);

				//If the context doesn't have member specific context associated with it we can broadcast about it
				//And let a listener handle this situation
				if (!context.HasContextualMemberMetadata())
					OnFoundUnknownAssociatedType?.Invoke(context);
				else
				{
					//If there is contextual information associated with the member we need to fall back
					//Also, just because we're in the complex registry and we encountered a member that requires context it does NOT mean that the member is a complex type
					//We may have found something like MyEnum[] and so we need to fall back to the general handler for this scenario

					//Fallback to the fallback factory
					ITypeSerializerStrategy strategy = fallbackSerializerFactoryService.Create(context);

					if (strategy == null)
						throw new InvalidOperationException($"Could not handle creation of factory for Type: {context.TargetType} on Type: {typeToParse}.");

					//Add the contextual serializer to the map
					contextBasedSerializers.Add(mi.Attribute<WireMemberAttribute>().MemberOrder, strategy);
				}
			}

			return contextBasedSerializers;
		}

		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <see cref="ISerializableTypeContext"/>
		/// </summary>
		/// <param name="context">The member context for the <see cref="ITypeSerializerStrategy"/>.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		public ITypeSerializerStrategy Create(ISerializableTypeContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context), $"Provided argument {nameof(context)} is null.");

			if (context.ContextRequirement == SerializationContextRequirement.RequiresContext)
				throw new InvalidOperationException($"Complex serializer factory doesn't support member contexts yet. Failed to create serializer for Type: {context.TargetType.FullName}.");

			if (isTypeRegistered(context.TargetType))
				return serializerProviderService.Get(context.TargetType); //If it's known we just provide it

			//We need serializers that are context based
			IDictionary<int, ITypeSerializerStrategy> contextualSerializers = DiscoverContextualSerializers(context.TargetType);

			//Once we've check the object graph of depth 1 to generate contextual serializers or alert the serialization service about unknown types we
			//can now create the complex type serializer
			return ComplexTypeSerializerStrategy.Create(context.TargetType, new DefaultContextualSerializerProvider(serializerProviderService, new ReadOnlyDictionary<int, ITypeSerializerStrategy>(contextualSerializers)));
		}
	}
}
