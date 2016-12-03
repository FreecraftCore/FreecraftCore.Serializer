using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Metadata marker for marking serializers that serialize inherently known types.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal class KnownTypeSerializerAttribute : Attribute
	{

	}
}
