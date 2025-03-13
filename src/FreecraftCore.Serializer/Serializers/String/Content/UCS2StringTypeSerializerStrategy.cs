using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// UCS2 encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UCS2StringTypeSerializerStrategy 
		: BaseStringTypeSerializerStrategy<UCS2StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		/// <inheritdoc />
		public int CharacterSize => SizeInfo.MaximumCharacterSize;

		public UCS2StringTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.UCS2)
		{

		}
	}
}
