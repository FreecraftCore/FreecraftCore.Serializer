using Fasterflect;
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
	/// Decorator handler and factory for <see cref="Array"/> serializers.
	/// </summary>
	[DecoratorHandler]
	public class ArraySerializerDecoratorHandler : DecoratorHandler
	{
		public ArraySerializerDecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider)
			: base(serializerProvider)
		{

		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context), $"Provided argument {nameof(context)} was null.");

			return context.TargetType.IsArray;
		}

		//TODO: Refactor; mess; too long
		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if(!context.BuiltContextKey.HasValue)
				throw new InvalidOperationException($"Provided {nameof(ISerializableTypeContext)} did not contain a valid {nameof(context.BuiltContextKey)} for Context: {context.ToString()}.");

			ICollectionSizeStrategy collectionSizeStrategy = null;

			//TODO: Handle contextless requests. The future may require a single array serializer for all unknown sizes.
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.FixedSize))
			{
				int knownSize = context.BuiltContextKey.Value.ContextSpecificKey.Key;

				collectionSizeStrategy = new FixedSizeCollectionSizeStrategy(knownSize);
			}
			else if(context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.SendSize))
			{
				switch ((SendSizeAttribute.SizeType)context.BuiltContextKey.Value.ContextSpecificKey.Key)
				{
					case SendSizeAttribute.SizeType.Byte:
						collectionSizeStrategy = new ByteSizeCollectionSizeStrategy();
						break;
					case SendSizeAttribute.SizeType.Int32:
						collectionSizeStrategy = new Int32SizeCollectionSizeStrategy(this.serializerProviderService.Get<int>());
						break;
					case SendSizeAttribute.SizeType.UShort:
						collectionSizeStrategy = new UInt16SizeCollectionSizeStrategy(this.serializerProviderService.Get<ushort>());
						break;

					default:
						throw new InvalidOperationException($"Encountered unsupported {nameof(SendSizeAttribute.SizeType)} Value: {context.BuiltContextKey.Value.ContextSpecificKey.Key}");
				}
			}

			//TODO: Should we really have a default?
			//if they marked it with nothing then use a the byte
			if (collectionSizeStrategy == null)
				collectionSizeStrategy = new ByteSizeCollectionSizeStrategy();

			if (context.TargetType == null)
				throw new InvalidOperationException($"Provided target type null.");

			if (context.TargetType.GetElementType() == null)
				throw new InvalidOperationException($"Element type null.");

			//TODO: This is an expirmental high preformance array serializer. It could have buffer overflows or other faults. It's not safe
			/*if (typeof(TType) == typeof(int[]))
			{
				return new Int32ArraySerializerDecorator(serializerProviderService, collectionSizeStrategy, context.ContextRequirement) as ITypeSerializerStrategy<TType>;
			}*/

			//If we know about the size then we should create a knownsize array decorator
			ITypeSerializerStrategy<TType> strat = typeof(ArraySerializerDecorator<>).MakeGenericType(context.TargetType.GetElementType())
				.CreateInstance(serializerProviderService, collectionSizeStrategy, context.ContextRequirement) as ITypeSerializerStrategy<TType>;

			if(strat == null)
				throw new InvalidOperationException($"Failed to construct an {nameof(ArraySerializerDecorator<TType>)} for the Type: {typeof(TType).FullName} in final creation step.");

			return strat;
		}

		/// <inheritdoc />
		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			//error handling and checking is done in base

			//TODO: Handling for GetElementType == null
			//array inner-type never have context
			return new ISerializableTypeContext[] { new TypeBasedSerializationContext(context.TargetType.GetElementType()) };
		}
	}
}
