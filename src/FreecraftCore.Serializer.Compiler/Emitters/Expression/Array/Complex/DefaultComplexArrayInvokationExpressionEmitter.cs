using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class DefaultComplexArrayInvokationExpressionEmitter 
		: BaseArraySerializationInvokationExpressionEmitter<ComplexArrayTypeSerializerStrategy>
	{
		public Type ElementType { get; }

		public DefaultComplexArrayInvokationExpressionEmitter([NotNull] Type elementType)
		{
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName());
			yield return IdentifierName(ElementType.Name);
		}
	}
}
