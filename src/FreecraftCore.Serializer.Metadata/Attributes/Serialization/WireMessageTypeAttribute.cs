using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Optional attribute that indicates a type is a fully serialized conceptual wire message type.
	/// This is required if a type does not implement <see cref="IWireMessage{TMessageType}"/>.
	/// This indicates to the source generator it should implement <see cref="IWireMessage{TMessageType}"/>
	/// on the type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class WireMessageTypeAttribute : Attribute
	{

	}
}
