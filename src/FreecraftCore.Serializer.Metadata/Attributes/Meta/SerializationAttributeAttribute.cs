using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="Attribute"/> for marking a target <see cref="Attribute"/>
	/// class <see cref="Type"/> as a serialization controlling attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SerializationAttributeAttribute : Attribute
	{
		public SerializationAttributeAttribute()
		{
			
		}
	}
}
