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
	public class WireDataContractBaseLinkAttribute : PolymorphicTypeLinkingAttribute
	{
		public WireDataContractBaseLinkAttribute(int index) 
			: base(index)
		{

		}
	}
}
