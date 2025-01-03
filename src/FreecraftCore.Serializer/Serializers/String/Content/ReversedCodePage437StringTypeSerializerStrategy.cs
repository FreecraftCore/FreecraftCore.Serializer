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
			//This is the default that WoW uses
			//however now that the serializer is being used for other projects
			//we need to have the option to use different ones.
		}
	}
}
