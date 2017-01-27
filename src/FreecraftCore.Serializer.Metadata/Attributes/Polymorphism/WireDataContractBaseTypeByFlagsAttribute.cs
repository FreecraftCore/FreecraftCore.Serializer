using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// that have basetypes that deserialize to. This particular version checks for flags in a value.
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class WireDataContractBaseTypeByFlagsAttribute : Attribute
	{
		/// <summary>
		/// The child type. Should be a child of the targeted class.
		/// </summary>
		[NotNull]
		public Type ChildType { get; }

		/// <summary>
		/// Flag to check for.
		/// If the flag is present then the type maps to ChildType.
		/// </summary>
		public int Flag { get; }

		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireDataContract.
		/// </summary>
		public WireDataContractBaseTypeByFlagsAttribute(int flagToCheckFor, [NotNull] Type childType)
		{
			if (childType == null)
				throw new ArgumentNullException(nameof(childType), $"Provided {nameof(childType)} was null.");

			ChildType = childType;
			Flag = flagToCheckFor;
		}
	}
}