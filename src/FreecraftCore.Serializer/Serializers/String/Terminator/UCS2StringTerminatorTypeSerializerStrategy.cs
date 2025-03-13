using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// UCS-2 null terminator encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UCS2StringTerminatorTypeSerializerStrategy 
		: BaseStringTerminatorSerializerStrategy<UCS2StringTerminatorTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		/// <inheritdoc />
		public int CharacterSize => SizeInfo.MaximumCharacterSize;

		public UCS2StringTerminatorTypeSerializerStrategy()
			: base(CustomCharacterEncodingHelpers.UCS2)
		{

		}
	}
}
