using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// UTF32 null terminator encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UTF32StringTerminatorTypeSerializerStrategy : BaseStringTerminatorSerializerStrategy<UTF32StringTerminatorTypeSerializerStrategy>
	{
		public UTF32StringTerminatorTypeSerializerStrategy()
			: base(Encoding.UTF32)
		{

		}
	}
}
