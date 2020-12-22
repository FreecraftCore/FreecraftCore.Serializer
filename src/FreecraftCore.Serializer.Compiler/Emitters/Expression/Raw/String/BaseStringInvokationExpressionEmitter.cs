using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace FreecraftCore.Serializer
{
	public abstract class BaseStringInvokationExpressionEmitter : BaseInvokationExpressionEmitter
	{
		protected EncodingType Encoding { get; }

		protected bool IsReversedString { get; }

		protected BaseStringInvokationExpressionEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode, EncodingType encoding) 
			: base(actualType, member, mode)
		{
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));
			Encoding = encoding;

			IsReversedString = member.HasAttributeExact<ReverseDataAttribute>();
		}

		protected virtual string CalculateBaseSerializerTypeName()
		{
			if (IsReversedString)
				return $"Reversed{Encoding}StringTypeSerializerStrategy";
			else
				return $"{Encoding}StringTypeSerializerStrategy";
		}

		protected virtual string CalculateBaseSerializerTerminatorTypeName()
		{
			return $"{Encoding}StringTerminatorTypeSerializerStrategy";
		}
	}
}
