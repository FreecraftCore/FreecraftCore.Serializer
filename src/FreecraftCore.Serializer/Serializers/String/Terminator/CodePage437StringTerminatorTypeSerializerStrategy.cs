using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// CodePage437 null terminator encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class CodePage437StringTerminatorTypeSerializerStrategy : BaseStringTerminatorSerializerStrategy<CodePage437StringTerminatorTypeSerializerStrategy>
	{
		public CodePage437StringTerminatorTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.CodePage437)
		{

		}
	}
}
