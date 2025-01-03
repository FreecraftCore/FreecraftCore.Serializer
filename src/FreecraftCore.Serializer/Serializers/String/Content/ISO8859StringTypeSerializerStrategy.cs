using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// ISO8859 encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ISO8859StringTypeSerializerStrategy
		: BaseStringTypeSerializerStrategy<ISO8859StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		public ISO8859StringTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.ISO8859)
		{
			// This is useful for when you want to support the extended character set to include characters with accents.
			// Ex. is chat messages which should be able to contain those.
		}
	}
}
