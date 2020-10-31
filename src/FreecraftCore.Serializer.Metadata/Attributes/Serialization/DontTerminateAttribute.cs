using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata that indicates that a string, or maybe a collection, shouldn't terminate.
	/// (Ex. String size 4 "Hell" should be written as byte[] { "H".ToByte() ... } with no '\0' character.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DontTerminateAttribute : Attribute
	{
		public DontTerminateAttribute()
		{
			//Do not need to do anything.
		}
	}
}
