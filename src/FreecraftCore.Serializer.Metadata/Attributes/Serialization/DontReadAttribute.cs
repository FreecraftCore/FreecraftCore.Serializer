using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker that indicates a member sholdn't be read.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class DontReadAttribute : Attribute
	{

	}
}
