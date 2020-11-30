using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker that indicates a member sholdn't be written.
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class DontWriteAttribute : Attribute
	{

	}
}
