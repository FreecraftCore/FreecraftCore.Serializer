using FreecraftCore.Serializer.KnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer.API
{

	public static class SerializerDecoratorHandlerFactory
	{
		public static IEnumerable<DecoratorHandler> Create(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory lookupKeyFactory, ISerializerStrategyFactory fallbackFactory)
		{
			//Order matters. The last handler act as the essentially a default handler except it does still have some requirements but not much
			return new List<DecoratorHandler>
			{
				new ArraySerializerDecoratorHandler(serializerProvider),
				new EnumSerializerDecoratorHandler(serializerProvider, fallbackFactory),
				new StringSerializerDecoratorHandler(serializerProvider),
				new SubComplexTypeSerializerDecoratorHandler(serializerProvider),
				new ComplexTypeSerializerDecoratorHandler(serializerProvider, lookupKeyFactory) //it's important that this is the final/last handler
			};
		}
	}
}
