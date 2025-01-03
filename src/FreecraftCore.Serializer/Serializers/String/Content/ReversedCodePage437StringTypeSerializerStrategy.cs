using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// CodePage437 encoding implementation of string serialization using data reversal.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ReversedCodePage437StringTypeSerializerStrategy 
		: BaseReversedStringTypeSerializerStrategy<ReversedCodePage437StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		public ReversedCodePage437StringTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.CodePage437)
		{
			// If you REALLY need old CodePage437
		}
	}
}
