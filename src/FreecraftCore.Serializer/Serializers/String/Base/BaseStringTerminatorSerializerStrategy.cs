using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Base contract for serializer type that can write null terminators.
	/// </summary>
	/// <typeparam name="TChildType"></typeparam>
	public abstract class BaseStringTerminatorSerializerStrategy<TChildType> : BaseEncodableTypeSerializerStrategy<TChildType>, IStringTypeSerializerStrategy
		where TChildType : BaseStringTerminatorSerializerStrategy<TChildType>, new()
	{
		protected BaseStringTerminatorSerializerStrategy([NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override string Read(Span<byte> buffer, ref int offset)
		{
			//We don't really need to read the data, we just need to push ourselves forward
			offset += CharacterSize;
			return String.Empty;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(string value, Span<byte> buffer, ref int offset)
		{
			for (int i = 0; i < CharacterSize; i++)
				buffer[offset + i] = 0;

			offset += CharacterSize;
		}
	}
}
