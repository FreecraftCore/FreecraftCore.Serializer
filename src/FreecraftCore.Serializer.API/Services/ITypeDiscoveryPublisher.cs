using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{

	public delegate void FoundUnknownAssociatedType(ISerializableTypeContext context);

	/// <summary>
	/// Contract for types that can discover known types and broadcast
	/// their existence.
	/// </summary>
	public interface ITypeDiscoveryPublisher
	{
		/// <summary>
		/// Event that can be subscribed to for alerts on found associated types.
		/// </summary>
		event FoundUnknownAssociatedType OnFoundUnknownAssociatedType;
	}
}
