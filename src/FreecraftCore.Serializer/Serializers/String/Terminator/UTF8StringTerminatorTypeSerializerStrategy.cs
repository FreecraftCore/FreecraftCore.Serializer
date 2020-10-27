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
	public sealed class UTF8StringTerminatorTypeSerializerStrategy : BaseStringTerminatorSerializerStrategy<UTF8StringTerminatorTypeSerializerStrategy>
	{
		public UTF8StringTerminatorTypeSerializerStrategy()
			: base(Encoding.UTF8)
		{

		}
	}
}
