using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract/Base serializer strategy for string encodable serializers.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseEncodableTypeSerializerStrategy<TChildType> : StatelessTypeSerializerStrategy<TChildType, string>, IBaseEncodableTypeSerializerStrategy
		where TChildType : StatelessTypeSerializerStrategy<TChildType, string>, new()
	{
		/// <inheritdoc />
		public Encoding EncodingStrategy { get; }

		/// <inheritdoc />
		public int CharacterSize { get; }

		protected BaseEncodableTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
		{
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			EncodingStrategy = encodingStrategy;
			CharacterSize = new EncoderCharacterSizeStrategy().Compute(encodingStrategy);
		}
	}
}
