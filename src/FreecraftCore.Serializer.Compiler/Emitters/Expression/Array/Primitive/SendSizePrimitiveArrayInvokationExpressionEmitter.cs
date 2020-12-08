using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
		public PrimitiveSizeType SizeType { get; }

		public SendSizePrimitiveArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arraySymbol, ISymbol member, PrimitiveSizeType sizeType, SerializationMode mode)
			: base(arraySymbol, member, mode)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			SizeType = sizeType;

			if (!arraySymbol.ElementType.IsPrimitive())
				throw new InvalidOperationException($"Type: {arraySymbol.ElementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
			yield return IdentifierName(SizeType.ToString());
		}
	}
}
