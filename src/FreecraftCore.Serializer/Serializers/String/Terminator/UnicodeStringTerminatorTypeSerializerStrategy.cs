using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Unicode null terminator encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UTF16StringTerminatorTypeSerializerStrategy : BaseStringTerminatorSerializerStrategy<UTF16StringTerminatorTypeSerializerStrategy>
	{
		public UTF16StringTerminatorTypeSerializerStrategy()
			: base(Encoding.Unicode)
		{

		}
	}
}
