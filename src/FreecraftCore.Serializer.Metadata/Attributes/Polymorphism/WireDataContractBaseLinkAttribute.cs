using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker for Types that are linked to their base type
	/// via a key. Polymorphic types can be serialized directly without a link to
	/// their base information since all child's implement serialization for base-type fields.
	/// However, it's required for deserialization/read to know the linking.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)] //classes or structs can be WireDataContracts
	public class WireDataContractBaseLinkAttribute : Attribute
	{
		/// <summary>
		/// Unique index/key that links the child to the base type.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Links to the basetype with the provided index at runtime.
		/// </summary>
		/// <param name="index">Unique per Type index.</param>
		public WireDataContractBaseLinkAttribute(int index)
		{
			if (index < 0)
				throw new ArgumentException($"Provided wire child index is less than 0. Was: {index}.");

			Index = index;
		}
	}
}
