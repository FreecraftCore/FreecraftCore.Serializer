using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public abstract class BaseEncodableTypeSerializerStrategy<TChildType> : StatelessTypeSerializer<TChildType, string>
		where TChildType : StatelessTypeSerializer<TChildType, string>, new()
	{
		/// <summary>
		/// The encoding strategy to use for the serialization.
		/// </summary>
		protected Encoding EncodingStrategy { get; }

		/// <summary>
		/// Size of the individual char encoding.
		/// </summary>
		protected int CharacterSize { get; }

		protected BaseEncodableTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
		{
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			EncodingStrategy = encodingStrategy;
			CharacterSize = new EncoderCharacterSizeStrategy().Compute(encodingStrategy);
		}
	}
}
