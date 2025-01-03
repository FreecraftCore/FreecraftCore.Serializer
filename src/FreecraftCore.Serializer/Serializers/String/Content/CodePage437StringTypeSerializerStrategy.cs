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
			// If you REALLY need old CodePage437
		}
	}
}
