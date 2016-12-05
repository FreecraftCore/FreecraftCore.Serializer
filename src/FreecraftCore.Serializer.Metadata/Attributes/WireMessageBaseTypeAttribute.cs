using System;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// that have basetypes that deserialize to.
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)] //classes or structs can be WireMessages
	public class WireMessageBaseTypeAttribute : Attribute
	{
		/// <summary>
		/// The child type. Should be a child of the targeted class.
		/// </summary>
		public Type ChildType { get; }

		/// <summary>
		/// Unique index/key for the interpter to know what the base type is in the stream.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireMessage.
		/// </summary>
		public WireMessageBaseTypeAttribute(int uniqueIndex, Type childType)
		{
			if (uniqueIndex < 1)
				throw new ArgumentException($"Provided wire child index is less than 1. Was: {uniqueIndex}.");

			if (childType == null)
				throw new ArgumentNullException(nameof(childType), $"Provided {childType} was null.");

			ChildType = childType;

			Index = uniqueIndex;
		}
	}
}