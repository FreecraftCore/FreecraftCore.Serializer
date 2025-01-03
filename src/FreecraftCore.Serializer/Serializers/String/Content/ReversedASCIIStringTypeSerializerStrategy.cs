using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// ASCII encoding implementation of string serialization using data reversal.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ReversedASCIIStringTypeSerializerStrategy : BaseReversedStringTypeSerializerStrategy<ReversedASCIIStringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		/// <inheritdoc />
		public int CharacterSize => SizeInfo.MaximumCharacterSize;

		public ReversedASCIIStringTypeSerializerStrategy()
			: base(Encoding.ASCII)
		{
			//This is the default that WoW uses
			//however now that the serializer is being used for other projects
			//we need to have the option to use different ones.
		}
	}
}
