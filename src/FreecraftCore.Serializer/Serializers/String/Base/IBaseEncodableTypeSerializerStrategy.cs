using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public interface IBaseEncodableTypeSerializerStrategy : ITypeSerializerStrategy<string>
	{
		/// <summary>
		/// The encoding strategy to use for the serialization.
		/// </summary>
		Encoding EncodingStrategy { get; }

		/// <summary>
		/// Size of the individual char encoding.
		/// </summary>
		int CharacterSize { get; }
	}
}
