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
	internal sealed class SendSizeComplexArrayInvokationExpressionEmitter
		: BaseArraySerializationInvokationExpressionEmitter<SendSizeComplexArrayTypeSerializerStrategy>
	{
		public PrimitiveSizeType SizeType { get; }

		public SendSizeComplexArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arraySymbol, ISymbol member, PrimitiveSizeType sizeType, SerializationMode mode)
			: base(arraySymbol, member, mode)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			SizeType = sizeType;
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName(Member));
			yield return IdentifierName(ElementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
			yield return IdentifierName(SizeType.ToString());
		}
	}
}
