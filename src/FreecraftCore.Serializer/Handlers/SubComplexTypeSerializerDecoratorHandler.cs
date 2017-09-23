using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator handler and factory for complex types that have children.
	/// </summary>
	[DecoratorHandler]
	public class SubComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		[NotNull]
		public IMemberSerializationMediatorFactory mediatorFactoryService { get; }

		public SubComplexTypeSerializerDecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider,
			[NotNull] IMemberSerializationMediatorFactory mediatorFactory)
			: base(serializerProvider)
		{
			if (mediatorFactory == null) throw new ArgumentNullException(nameof(mediatorFactory));

			mediatorFactoryService = mediatorFactory;
		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context), $"Provided argument {nameof(context)} was null.");

			//Check if the type has wirebase type attributes.
			//If it does then the type is complex and can have subtypes coming across the wire
			//Include checking for runtime link information
			return (context.TargetType.GetTypeInfo().GetCustomAttributes<WireDataContractAttribute>(false).Any() && context.TargetType.GetTypeInfo().GetCustomAttribute<WireDataContractAttribute>(false).ExpectRuntimeLink) 
				|| context.TargetType.GetTypeInfo().GetCustomAttributes<WireDataContractBaseTypeAttribute>(false).Any() 
				|| context.TargetType.GetTypeInfo().GetCustomAttribute<WireDataContractBaseTypeByFlagsAttribute>(false) != null;
		}

		//TODO: Refactor
		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			IChildKeyStrategy keyStrategy = null;

			//TODO: Check if we can get this from context?
			//Check the WireDataContract attribute for keysize information
			WireDataContractAttribute contractAttribute = typeof(TType).GetTypeInfo().GetCustomAttribute<WireDataContractAttribute>(true);

			//Build the strategy for child size and value read/write
			switch (contractAttribute.OptionalChildTypeKeySize)
			{
				//TODO: Make it a factory
				case WireDataContractAttribute.KeyType.Byte:
					keyStrategy = new GenericChildKeyStrategy<byte>(contractAttribute.TypeHandling, serializerProviderService.Get<byte>());
					break;
				case WireDataContractAttribute.KeyType.Int32:
					keyStrategy = new GenericChildKeyStrategy<int>(contractAttribute.TypeHandling, serializerProviderService.Get<int>());
					break;
				case WireDataContractAttribute.KeyType.UShort:
					keyStrategy = new GenericChildKeyStrategy<ushort>(contractAttribute.TypeHandling, serializerProviderService.Get<ushort>());
					break;
				case WireDataContractAttribute.KeyType.None:
				default:
					throw new InvalidOperationException($"Encountered Type: {typeof(TType).FullName} that requires child type mapping but has provided {nameof(WireDataContractAttribute.KeyType)} Value: {contractAttribute.OptionalChildTypeKeySize} which is invalid.");
			}

			ITypeSerializerStrategy<TType> strat = null;

			//Depending on if we're flags or key return the right serializer decorator.
			if (typeof(TType).GetTypeInfo().GetCustomAttribute<WireDataContractBaseTypeByFlagsAttribute>() == null)
				//Won't be null at this point. Should be a valid strategy. We also don't need to deal with context since there is only EVER 1 serializer of this type per type.
				strat = new SubComplexTypeSerializerDecorator<TType>(new LambdabasedDeserializationPrototyeFactory<TType>(), new MemberSerializationMediatorCollection<TType>(mediatorFactoryService),  serializerProviderService, keyStrategy);
			else
				strat = new SubComplexTypeWithFlagsSerializerDecorator<TType>(new LambdabasedDeserializationPrototyeFactory<TType>(), new MemberSerializationMediatorCollection<TType>(mediatorFactoryService), serializerProviderService, keyStrategy);

			//Check for compression flags
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Compressed))
				strat = new CompressionTypeSerializerStrategyDecorator<TType>(strat, serializerProviderService.Get<uint>());

			return strat;
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We need to include the potential default child type now
			IEnumerable<ISerializableTypeContext> contexts = context.TargetType.GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>() != null ?
				new ISerializableTypeContext[] { new TypeBasedSerializationContext(context.TargetType.GetTypeInfo().GetCustomAttribute<DefaultChildAttribute>().ChildType) } : Enumerable.Empty<ISerializableTypeContext>();


			contexts.Concat(new TypeMemberParsedTypeContextCollection(context.TargetType));

			//Grab the children from the metadata; return type contexts so the types can be handled (no context is required because the children are their own registerable type
#if !NET35
			return contexts.Concat(GetAssociatedChildren(context.TargetType).Select(t => new TypeBasedSerializationContext(t)));
#else
			return contexts.Concat(GetAssociatedChildren(context.TargetType).Select(t => new TypeBasedSerializationContext(t) as ISerializableTypeContext));
#endif
		}

		[NotNull]
		private IEnumerable<Type> GetAssociatedChildren([NotNull] Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			IEnumerable<Type> baseTypesByKey = type.GetTypeInfo().GetCustomAttributes<WireDataContractBaseTypeAttribute>().Select(x => x.ChildType);

			IEnumerable<Type> baseTypesByFlags = type.GetTypeInfo().GetCustomAttributes<WireDataContractBaseTypeByFlagsAttribute>().Select(x => x.ChildType);

			return baseTypesByKey.Concat(baseTypesByFlags);
		}
	}
}
