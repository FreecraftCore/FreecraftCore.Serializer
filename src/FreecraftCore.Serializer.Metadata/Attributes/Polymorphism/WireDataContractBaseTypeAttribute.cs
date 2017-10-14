using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// that have basetypes that deserialize to.
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)] //classes or structs can be WireDataContracts
	public class WireDataContractBaseTypeAttribute : Attribute
	{
		/// <summary>
		/// The child type. Should be a child of the targeted class.
		/// </summary>
		[NotNull]
		public Type ChildType { get; }

		/// <summary>
		/// Unique index/key for the interpter to know what the base type is in the stream.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireDataContract.
		/// </summary>
		public WireDataContractBaseTypeAttribute(int uniqueIndex, [NotNull] Type childType)
		{
			if (uniqueIndex < 0)
				throw new ArgumentException($"Provided wire child index is less than 0. Was: {uniqueIndex}.");

			//TODO: How can we prevent them from registering types with reserved keys?
			if(uniqueIndex == Int32.MaxValue)
				throw new ArgumentException($"Provided wire child index was reserved value Int32.MaxValue. This is reserved internally. Was: {uniqueIndex}.");

			if (childType == null)
				throw new ArgumentNullException(nameof(childType), $"Provided {nameof(childType)} was null.");

			ChildType = childType;
			Index = uniqueIndex;
		}
	}
}