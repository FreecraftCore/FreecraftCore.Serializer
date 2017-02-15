using FreecraftCore.Serializer.KnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.API
{

	public static class SerializerDecoratorHandlerFactory
	{
		public static IEnumerable<DecoratorHandler> Create([NotNull] IContextualSerializerProvider serializerProvider,
			[NotNull] IContextualSerializerLookupKeyFactory lookupKeyFactory,
			[NotNull] ISerializerStrategyFactory fallbackFactory)
		{
			if (serializerProvider == null) throw new ArgumentNullException(nameof(serializerProvider));
			if (lookupKeyFactory == null) throw new ArgumentNullException(nameof(lookupKeyFactory));
			if (lookupKeyFactory == null) throw new ArgumentNullException(nameof(lookupKeyFactory));

			//Order matters. The last handler act as the essentially a default handler except it does still have some requirements but not much
			return new List<DecoratorHandler>
			{
				new SpecialComplexTypeSerializerDecoratorhandler(serializerProvider), //handles special inaccessible types like DateTime
				new PrimitiveTypeSerializerDecoratorHandler(serializerProvider),
				new ArraySerializerDecoratorHandler(serializerProvider),
				new EnumSerializerDecoratorHandler(serializerProvider, fallbackFactory),
				new StringSerializerDecoratorHandler(serializerProvider),
				new SubComplexTypeSerializerDecoratorHandler(serializerProvider, new MemberSerializationMediatorFactory(serializerProvider, lookupKeyFactory)),
				new ComplexTypeSerializerDecoratorHandler(serializerProvider, new MemberSerializationMediatorFactory(serializerProvider, lookupKeyFactory)) //it's important that this is the final/last handler
			};
		}
	}
}
