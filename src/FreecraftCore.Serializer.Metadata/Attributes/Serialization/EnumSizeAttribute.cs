using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EnumSizeAttribute : Attribute
	{

	}
}
