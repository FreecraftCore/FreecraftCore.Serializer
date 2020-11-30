using System;
using System.ComponentModel;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// Marking a class with this attribute will prepare it for the wire. (Refer to the concept of Protobuf-net or
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, Inherited = true, AllowMultiple = false)] //classes or structs can be WireDataContracts. Inteffaces now can be too.
	public sealed class WireDataContractAttribute : Attribute
	{
		/// <summary>
		/// Indicates the size of the key.
		/// (Ex. Send only a byte to indicate type or maybe define it using a full Int32)
		/// </summary>
		public PrimitiveSizeType? OptionalSubTypeKeySize { get; }

		/// <summary>
		/// Indicates if SubType size is used.
		/// </summary>
		public bool UsesSubTypeSize => OptionalSubTypeKeySize.HasValue;

		//If this CTOR is ever changed you MUST change: IsDefiningPolymorphicDefinition
		public WireDataContractAttribute(PrimitiveSizeType optionalSubtypeKeySize)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), optionalSubtypeKeySize)) throw new InvalidEnumArgumentException(nameof(optionalSubtypeKeySize), (int) optionalSubtypeKeySize, typeof(PrimitiveSizeType));
			OptionalSubTypeKeySize = optionalSubtypeKeySize;
		}

		public WireDataContractAttribute()
		{
			OptionalSubTypeKeySize = null;
		}

		/// <summary>
		/// Internal compiler API method that indicates if an instance
		/// of the attribute is likely yo define a polymorphic definition.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		internal static bool IsDefiningPolymorphicDefinition(int parameters)
		{
			return parameters != 0;
		}
	}
}