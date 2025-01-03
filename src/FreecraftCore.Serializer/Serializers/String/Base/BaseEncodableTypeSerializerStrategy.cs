using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer.Internal;
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
		public EncodingCharacterSizeData SizeInfo { get; }

		protected BaseEncodableTypeSerializerStrategy([NotNull] Encoding encodingStrategy)
		{
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			var sizeStrategy = new EncoderCharacterSizeStrategy();
			EncodingStrategy = encodingStrategy;
			SizeInfo = new EncodingCharacterSizeData(sizeStrategy.ComputeMinimum(encodingStrategy), sizeStrategy.ComputeMaximum(encodingStrategy), sizeStrategy.ComputeTerminator(encodingStrategy));
		}
	}
}
