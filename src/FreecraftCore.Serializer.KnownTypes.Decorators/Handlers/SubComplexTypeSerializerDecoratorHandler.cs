using Fasterflect;
using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator handler and factory for complex types that have children.
	/// </summary>
	[DecoratorHandler]
	public class SubComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		public SubComplexTypeSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider)
			: base(serializerProvider)
		{

		}
		/// <summary>
		/// Indicates if the <see cref="ISerializerDecoraterHandler"/> is able to handle the specified <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The member context.</param>
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context), $"Provided argument {nameof(context)} was null.");

			//Check if the type has wirebase type attributes.
			//If it does then the type is complex and can have subtypes coming across the wire
			return context.TargetType.GetCustomAttributes<WireDataContractBaseTypeAttribute>(false).Count() != 0 || context.TargetType.GetCustomAttribute<WireDataContractBaseTypeByFlagsAttribute>(false) != null;
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//error handling in base

			IChildKeyStrategy keyStrategy = null;

			//Check the WireDataContract attribute for keysize information
			WireDataContractAttribute contractAttribute = typeof(TType).GetCustomAttribute<WireDataContractAttribute>(true);

			//Build the strategy for child size and value read/write
			switch (contractAttribute.OptionalChildTypeKeySize)
			{
				case WireDataContractAttribute.KeyType.Byte:
					keyStrategy = new ByteChildKeyStrategy(contractAttribute.ShouldConsumeTypeInformation);
					break;
				case WireDataContractAttribute.KeyType.Int32:
					keyStrategy = new Int32ChildKeyStrategy(this.serializerProviderService.Get<int>(), contractAttribute.ShouldConsumeTypeInformation);
					break;
				case WireDataContractAttribute.KeyType.None:
				default:
					throw new InvalidOperationException($"Encountered Type: {typeof(TType).FullName} that requires child type mapping but has provided {nameof(WireDataContractAttribute.KeyType)} Value: {contractAttribute.OptionalChildTypeKeySize} which is invalid.");
			}

			//TODO: flags implementation

			//Won't be null at this point. Should be a valid strategy. We also don't need to deal with context since there is only EVER 1 serializer of this type per type.
			return new SubComplexTypeSerializerDecorator<TType>(serializerProviderService, keyStrategy);
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//We need to include the potential default child type now
			IEnumerable<ISerializableTypeContext> contexts = context.TargetType.Attribute<DefaultNoFlagsAttribute>() != null ?
				new ISerializableTypeContext[] { new TypeBasedSerializationContext(context.TargetType.Attribute<DefaultNoFlagsAttribute>().ChildType) } : Enumerable.Empty<ISerializableTypeContext>();


			//Grab the children from the metadata; return type contexts so the types can be handled (no context is required because the children are their own registerable type
#if !NET35
			return contexts.Concat(GetAssociatedChildren(context.TargetType).Select(t => new TypeBasedSerializationContext(t)));
#else
			return contexts.Concat(GetAssociatedChildren(context.TargetType).Select(t => new TypeBasedSerializationContext(t) as ISerializableTypeContext));
#endif
		}

		private IEnumerable<Type> GetAssociatedChildren(Type type)
		{
			return type.Attributes<WireDataContractBaseTypeAttribute>().Select(x => x.ChildType);
		}
	}
}
