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
	/// Decorator handler and factory for <see cref="Array"/> serializers.
	/// </summary>
	[DecoratorHandler]
	public class ArraySerializerDecoratorHandler : DecoratorHandler
	{
		public ArraySerializerDecoratorHandler(IContextualSerializerProvider serializerProvider)
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

			return context.TargetType.IsArray;
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//TODO: This is an expirmental high preformance array serializer. It could have buffer overflows or other faults. It's not safe
			/*if (typeof(TType) == typeof(int[]))
			{
				return new Int32ArraySerializerDecorator(serializerProviderService) as ITypeSerializerStrategy<TType>;
			}*/

			ICollectionSizeStrategy collectionSizeStrategy = null;

			//TODO: Handle contextless requests. The future may require a single array serializer for all unknown sizes.
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.FixedSize))
			{
				int knownSize = context.BuiltContextKey.Value.ContextSpecificKey.Key;

				collectionSizeStrategy = new FixedSizeCollectionSizeStrategy(knownSize);
			}
			else
				collectionSizeStrategy = new UnknownSizeCollectionSizeStrategy();

			//If we know about the size then we should create a knownsize array decorator
			return typeof(ArraySerializerDecorator<>).MakeGenericType(context.TargetType.GetElementType())
				.CreateInstance(serializerProviderService, collectionSizeStrategy, context.ContextRequirement) as ITypeSerializerStrategy<TType>;
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//array inner-type never have context
			return new ISerializableTypeContext[] { new TypeBasedSerializationContext(context.TargetType.GetElementType()) }; 
		}
	}
}
