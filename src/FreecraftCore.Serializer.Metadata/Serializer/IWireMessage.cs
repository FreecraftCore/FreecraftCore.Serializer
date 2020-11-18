using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for top-level serializable types.
	/// Think of this as "messages" a complete Type that encloses around fields (object graph)
	/// that can be read/written.
	///
	/// This contract is not required for a Type to be serializable. Only <see cref="WireDataContractAttribute"/>
	/// is required for that. However, for the Serializer service to know how to read/write the Type it is required
	/// that this type be implemented.
	/// </summary>
	/// <typeparam name="TMessageType">Wire message type.</typeparam>
	public interface IWireMessage<TMessageType> : ISelfSerializable<TMessageType>
		where TMessageType : IWireMessage<TMessageType>
	{

	}
}
