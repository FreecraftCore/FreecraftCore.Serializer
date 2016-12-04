using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public static class ISerializerProviderExtensions
	{
		public static ITypeSerializerStrategy<TRequestedType> Get<TRequestedType>(this ISerializerProvider factory)
		{
			return factory.Get(typeof(TRequestedType)) as ITypeSerializerStrategy<TRequestedType>;
		}
	}
}
