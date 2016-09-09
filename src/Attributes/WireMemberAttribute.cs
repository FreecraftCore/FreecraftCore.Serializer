using System;

/// <summary>
/// Meta-data attribute that can be used to mark wire serializable members on <see cref="Type"/>s
/// Marking a member with this attribute will prepare it for the wire. (Refer to the concept of Protobuf-net or
/// Blizzard's JAM for information on the concept).
/// </summary>
public class WireMemberAttribute : Attribute
{
	/// <summary>
	/// Unique integer value (per Type) that indicates the order the member
	/// should be wire serialized in. (Ex. Member with Order 1 will be serialized before Members with Order 7)
	/// </summary>
	public int MemberOrder { get; private set; }

	/// <summary>
	/// Creates a new Meta-data attribute for a Type that contains
	/// WireType member fields/properties.
	/// </summary>
	/// <param name="memberOrder"></param>
	public WireMemberAttribute(int memberOrder)
	{
		if(memberOrder < 0)
			throw new ArgumentException($"Provider argument {nameof(memberOrder)} must be greater than or equal to 0.", nameof(memberOrder));
		
		MemberOrder = memberOrder;
	}
}