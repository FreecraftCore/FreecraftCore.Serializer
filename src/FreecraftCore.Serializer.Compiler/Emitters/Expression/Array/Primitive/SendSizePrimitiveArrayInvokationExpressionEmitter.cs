using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class SendSizePrimitiveArrayInvokationExpressionEmitter
		: BaseArraySerializationInvokationExpressionEmitter<SendSizePrimitiveArrayTypeSerializerStrategy>
	{
		public Type ElementType { get; }

		public PrimitiveSizeType SizeType { get; }

		public SendSizePrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType, PrimitiveSizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
			SizeType = sizeType;

			if (!ElementType.IsPrimitive)
				throw new InvalidOperationException($"Type: {elementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.Name);
			yield return IdentifierName(SizeType.ToString());
		}
	}
}
