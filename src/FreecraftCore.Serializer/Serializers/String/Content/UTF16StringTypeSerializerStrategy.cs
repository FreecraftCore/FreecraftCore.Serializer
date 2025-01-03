using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Unicode encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UTF16StringTypeSerializerStrategy : BaseStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy>
	{
		public UTF16StringTypeSerializerStrategy()
			: base(Encoding.Unicode)
		{

		}
	}
}
