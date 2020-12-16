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
	public class WireDataContractBaseTypeAttribute : PolymorphicTypeLinkingAttribute
	{
		/// <summary>
		/// The child type. Should be a child of the targeted class.
		/// </summary>
		[NotNull]
		public Type ChildType { get; }

		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireDataContract.
		/// </summary>
		public WireDataContractBaseTypeAttribute(int uniqueIndex, [NotNull] Type childType)
			: base(uniqueIndex)
		{
			if(uniqueIndex < 0)
				throw new ArgumentException($"Provided wire child index is less than 0. Was: {uniqueIndex}.");

			if(childType == null)
				throw new ArgumentNullException(nameof(childType), $"Provided {nameof(childType)} was null.");

			ChildType = childType;
		}
	}
}