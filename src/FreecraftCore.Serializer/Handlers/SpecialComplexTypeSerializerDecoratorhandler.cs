using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SpecialComplexTypeSerializerDecoratorhandler : DecoratorHandler
	{
		/// <inheritdoc />
		public SpecialComplexTypeSerializerDecoratorhandler([NotNull] IContextualSerializerProvider serializerProvider) 
			: base(serializerProvider)
		{
		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			//Right now we can handle only DateTimes as special complex types.
			return context.TargetType == typeof(DateTime) || context.TargetType == typeof(BitArray);
		}

		/// <inheritdoc />
		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//We don't need any special types registered
			return Enumerable.Empty<ISerializableTypeContext>();
		}

		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context.TargetType == typeof(DateTime))
				return new PackedDateTimeSerializerStrategyDecorator(serializerProviderService.Get<int>()) as ITypeSerializerStrategy<TType>;

			if (context.TargetType == typeof(BitArray))
				return new BitArraySerializerStrategyDecorator() as ITypeSerializerStrategy<TType>;

			throw new InvalidOperationException($"Special decorator cannot handle {typeof(TType).FullName}.");
		}
	}
}
