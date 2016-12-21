using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	public static class IGeneralSerializerProviderExtensions
	{
		public static ITypeSerializerStrategy<TRequestedType> Get<TRequestedType>(this IGeneralSerializerProvider provider)
		{
			return provider.Get(typeof(TRequestedType)) as ITypeSerializerStrategy<TRequestedType>;
		}

		public static bool HasSerializerFor<TTypeToCheck>(this IGeneralSerializerProvider provider)
		{
			return provider.HasSerializerFor(typeof(TTypeToCheck));
		}
	}
}
