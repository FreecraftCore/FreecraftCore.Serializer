using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public static class IGeneralSerializerProviderExtensions
	{
		public static ITypeSerializerStrategy<TRequestedType> Get<TRequestedType>(this IGeneralSerializerProvider factory)
		{
			return factory.Get(typeof(TRequestedType)) as ITypeSerializerStrategy<TRequestedType>;
		}
	}
}
