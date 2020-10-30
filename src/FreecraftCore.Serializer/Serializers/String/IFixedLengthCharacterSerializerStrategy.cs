using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Generic constaint metadata marker interface that marks a serializer
	/// as having fixed-length character encoding/serialization behavior.
	/// </summary>
	public interface IFixedLengthCharacterSerializerStrategy
	{
		/// <summary>
		/// The fixed-length size of the character.
		/// </summary>
		int CharacterSize { get; }
	}
}
