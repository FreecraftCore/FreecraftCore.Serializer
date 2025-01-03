using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// ISO8859 null terminator encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ISO8859StringTerminatorTypeSerializerStrategy 
		: BaseStringTerminatorSerializerStrategy<ISO8859StringTerminatorTypeSerializerStrategy>
	{
		public ISO8859StringTerminatorTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.ISO8859)
		{

		}
	}
}
