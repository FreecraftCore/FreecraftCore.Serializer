using FreecraftCore.Serializer;
using System;
using System.Collections;
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
					//TODO: Refactor into factory
					case SendSizeAttribute.SizeType.Byte:
						collectionSizeStrategy = new GenericCollectionSizeStrategy<byte>(serializerProviderService.Get<byte>());
						break;
					case SendSizeAttribute.SizeType.Int32:
						collectionSizeStrategy = new GenericCollectionSizeStrategy<int>(serializerProviderService.Get<int>());
						break;
					case SendSizeAttribute.SizeType.UShort:
						collectionSizeStrategy = new GenericCollectionSizeStrategy<ushort>(serializerProviderService.Get<ushort>());
						break;

					default:
						throw new InvalidOperationException($"Encountered unsupported {nameof(SendSizeAttribute.SizeType)} Value: {context.BuiltContextKey.Value.ContextSpecificKey.Key}");
				}
			}

			//TODO: Should we really have a default?
			//if they marked it with nothing then use a the byte
			if (collectionSizeStrategy == null)
				collectionSizeStrategy = new GenericCollectionSizeStrategy<byte>(serializerProviderService.Get<byte>());

			if (context.TargetType == null)
				throw new InvalidOperationException($"Provided target type null.");

			if (context.TargetType.GetElementType() == null)
				throw new InvalidOperationException($"Element type null.");

			ITypeSerializerStrategy<TType> strat = null;

			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Reverse) && context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.FixedSize))
			{
				//Check if it's a byte array. If it's not then we need to throw
				if(typeof(TType) == typeof(byte[]))
					strat = new FixedSizeReversedByteArraySerializerDecorator(collectionSizeStrategy) as ITypeSerializerStrategy<TType>;
				else
				{
					throw new InvalidOperationException($"Cannot have a {typeof(TType).FullName} serialized with Reverse and Fixed attributes.");
				}
			}
			else
			{
				//If we know about the size then we should create a knownsize array decorator
				strat = Activator.CreateInstance(typeof(ArraySerializerDecorator<>).MakeGenericType(context.TargetType.GetElementType()),
					serializerProviderService, collectionSizeStrategy, context.ContextRequirement) as ITypeSerializerStrategy<TType>;
			}

			//TODO: This is an expirmental high preformance array serializer. It could have buffer overflows or other faults. It's not safe
			/*if (typeof(TType) == typeof(int[]))
			{
				return new Int32ArraySerializerDecorator(serializerProviderService, collectionSizeStrategy, context.ContextRequirement) as ITypeSerializerStrategy<TType>;
			}*/


			if (strat == null)
				throw new InvalidOperationException($"Failed to construct an {nameof(ArraySerializerDecorator<TType>)} for the Type: {typeof(TType).FullName} in final creation step.");

			//Now check if it should be decorated with compression
			//TODO: Support multiple sizetypes for compression
			//TODO: Add support for compression as not the final member if WoW ever needs it
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Compressed))
			{
				strat = new CompressionTypeSerializerStrategyDecorator<TType>(strat, serializerProviderService.Get<uint>());
			}

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
