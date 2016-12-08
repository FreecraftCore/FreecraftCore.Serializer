using FreecraftCore.Serializer.KnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.API
{

	public static class SerializerDecoratorHandlerFactory
	{
		public static IEnumerable<DecoratorHandler> Create(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory lookupKeyFactory)
		{
			//Order matters. The last handler act as the essentially a default handler except it does still have some requirements but not much
			return new List<DecoratorHandler>
			{
				new ArraySerializerDecoratorHandler(serializerProvider, lookupKeyFactory),
				new EnumSerializerDecoratorHandler(serializerProvider, lookupKeyFactory),
				new StringSerializerDecoratorHandler(serializerProvider, lookupKeyFactory),
				new SubComplexTypeSerializerDecoratorHandler(serializerProvider, lookupKeyFactory),
				new ComplexTypeSerializerDecoratorHandler(serializerProvider, lookupKeyFactory) //it's important that this is the final/last handler
			};
		}
	}
}
