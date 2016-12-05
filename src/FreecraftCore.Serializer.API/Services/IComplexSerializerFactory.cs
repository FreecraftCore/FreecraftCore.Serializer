using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for a factory that produces complex serializers
	/// </summary>
	public interface IComplexSerializerFactory : ITypeDiscoveryPublisher, ISerializerStrategyFactory
	{

	}
}
