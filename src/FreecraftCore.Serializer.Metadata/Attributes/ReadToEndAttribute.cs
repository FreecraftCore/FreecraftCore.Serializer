using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata that indicates that a collection should ready all the way to the end.
	/// (Ex. Read all remaining bytes in the stream and initialzie it to this field/prop)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ReadToEndAttribute : Attribute
	{
		//Nothing needed
		/// <summary>
		/// Metadata that indicates that a collection should ready all the way to the end.
		/// (Ex. Read all remaining bytes in the stream and initialzie it to this field/prop)
		/// </summary>
		public ReadToEndAttribute()
		{
			
		}
	}
}
