using System;
using System.ComponentModel;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable classes/<see cref="Type"/>s
	/// Marking a class with this attribute will prepare it for the wire. (Refer to the concept of Protobuf-net or
	/// Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface)] //classes or structs can be WireDataContracts. Inteffaces now can be too.
	public class WireDataContractAttribute : Attribute
	{
		public enum KeyType
		{
			None = 0,
			Byte = 1,
			UShort = 2,
			Int32 = 3
		}

		/// <summary>
		/// Indicates the size of the key.
		/// (Ex. Send only a byte to indicate type or maybe define it using a full Int32)
		/// </summary>
		public KeyType OptionalChildTypeKeySize { get; }

		/// <summary>
		/// Indicates if the type information used for mapping base to child should
		/// be consumed when encountered.
		/// </summary>
		public InformationHandlingFlags TypeHandling { get; }

		/// <summary>
		/// Creates a new Meta-data attribute indicating the type is a WireDataContract.
		/// </summary>
		public WireDataContractAttribute(KeyType keyType = KeyType.None, InformationHandlingFlags typeHandling = InformationHandlingFlags.Default)
		{
			if (!Enum.IsDefined(typeof(KeyType), keyType))
				throw new InvalidEnumArgumentException(nameof(keyType), (int) keyType, typeof(KeyType));

			int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new InvalidEnumArgumentException(nameof(typeHandling), (int) typeHandling,
					typeof(InformationHandlingFlags));

			//The keytype provided is optional to be none.
			//If you want to wire up children to this for polymorphic serialization then
			//A provided keysize must be declared on the 1 WireDataContract instead of the N many WireBaseType attributes

			OptionalChildTypeKeySize = keyType;
			TypeHandling = typeHandling;
		}
	}
}