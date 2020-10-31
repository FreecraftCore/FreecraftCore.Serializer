using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data marker to indicate an Enum should be sent as a string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EnumStringAttribute : Attribute
	{
		public EnumStringAttribute()
		{
			//Don't need to do anything
		}
	}
}
