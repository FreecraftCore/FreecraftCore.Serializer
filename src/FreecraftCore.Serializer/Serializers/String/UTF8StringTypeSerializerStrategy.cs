using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// UTF8 encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UTF8StringTypeSerializerStrategy : BaseStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy>
	{
		public UTF8StringTypeSerializerStrategy()
			: base(Encoding.UTF8)
		{

		}
	}
}
