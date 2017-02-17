using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	[DecoratorHandler]
	public class PrimitiveTypeSerializerDecoratorHandler : DecoratorHandler
	{
		public PrimitiveTypeSerializerDecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider) 
			: base(serializerProvider)
		{

		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			return context.TargetType.GetTypeInfo().IsPrimitive && context.HasContextualMemberMetadata();
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//The only relevant type is the primitive type itself
			return new ISerializableTypeContext[] {new TypeBasedSerializationContext(context.TargetType)};
		}

		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (!context.HasContextualKey())
				throw new InvalidOperationException($"No contextual key was built for Type: {context.TargetType} in {GetType().FullName}");

			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot create serializer for Type: {context.TargetType} and Context: {context?.BuiltContextKey?.ToString()}");

			//The only type of context available to decorate primitive types with
			//is ReverseData for now
			ITypeSerializerStrategy<TType> serializer = serializerProviderService.Get<TType>();

			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Reverse))
				serializer = new EndianReverseDecorator<TType>(serializer);

			return serializer;
		}
	}
}
