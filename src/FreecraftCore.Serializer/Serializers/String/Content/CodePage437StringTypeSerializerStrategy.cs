using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// CodePage437 encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class CodePage437StringTypeSerializerStrategy 
		: BaseStringTypeSerializerStrategy<CodePage437StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		public CodePage437StringTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.CodePage437)
		{
			// This is useful for when you want to support the extended character set to include characters with accents.
			// Ex. is chat messages which should be able to contain those.
		}
	}
}
