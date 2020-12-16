using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data marker to indicate an Enum should be sent as a string.
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EnumStringAttribute : Attribute
	{
		public EnumStringAttribute()
		{
			//Don't need to do anything
		}
	}
}
