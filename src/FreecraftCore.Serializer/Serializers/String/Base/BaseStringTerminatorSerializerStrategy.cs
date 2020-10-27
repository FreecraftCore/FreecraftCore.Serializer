using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Base contract for serializer type that can write null terminators.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseStringTerminatorSerializerStrategy<TChildType> : BaseEncodableTypeSerializerStrategy<TChildType>
		where TChildType : StatelessTypeSerializer<TChildType, string>, new()
	{
		protected BaseStringTerminatorSerializerStrategy([NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{

		}

		public override string Read(Span<byte> source, ref int offset)
		{
			//We don't really need to read the data, we just need to push ourselves forward
			offset += CharacterSize;
			return String.Empty;
		}

		public override void Write(string value, Span<byte> destination, ref int offset)
		{
			for (int i = 0; i < CharacterSize; i++)
				destination[offset + i] = 0;

			offset += CharacterSize;
		}
	}
}
