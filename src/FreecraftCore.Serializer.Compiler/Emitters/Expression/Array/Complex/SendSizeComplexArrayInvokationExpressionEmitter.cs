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

		public SendSizeComplexArrayInvokationExpressionEmitter([NotNull] Type elementType, MemberInfo member, PrimitiveSizeType sizeType, SerializationMode mode)
			: base(elementType, member, mode)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			SizeType = sizeType;
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName());
			yield return IdentifierName(ElementType.Name);
			yield return IdentifierName(SizeType.ToString());
		}
	}
}
