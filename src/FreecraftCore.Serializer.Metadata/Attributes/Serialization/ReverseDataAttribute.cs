using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data marker to indicate that a member should be sent in reverse.
	/// (Ex. Instead of "x86" sent "68x" and the data is reversed back on the other side)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ReverseDataAttribute : Attribute
	{

		public ReverseDataAttribute()
		{
			//Don't need to do anything.
		}
	}
}
