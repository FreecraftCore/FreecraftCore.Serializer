using System;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// Marking a class with this attribute will prepare it for the wire. (Refer to the concept of Protobuf-net or
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)] //classes or structs can be WireMessages
	public class WireMessageAttribute : Attribute
	{
		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireMessage.
		/// </summary>
		public WireMessageAttribute()
		{
			
		}
	}
}