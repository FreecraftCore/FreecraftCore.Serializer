using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// ISO8859 encoding implementation of string serialization using data reversal.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ReversedISO8859StringTypeSerializerStrategy
		: BaseReversedStringTypeSerializerStrategy<ReversedISO8859StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		public ReversedISO8859StringTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.ISO8859)
		{
			// If you REALLY need old CodePage437
		}
	}
}
