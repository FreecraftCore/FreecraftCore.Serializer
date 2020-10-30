using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// UTF32 encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class UTF32StringTypeSerializerStrategy : BaseStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		public UTF32StringTypeSerializerStrategy()
			: base(Encoding.UTF32)
		{

		}
	}
}
