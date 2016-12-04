using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable members to indicate they should be packed.
	/// Consult (Blizzard's JAM for information on the concept).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] //members can be fields or props
	public class PackedAttribute : Attribute
	{
		//The marking of this attribute indicates it should be packed

		public PackedAttribute()
		{

		}
	}
}
