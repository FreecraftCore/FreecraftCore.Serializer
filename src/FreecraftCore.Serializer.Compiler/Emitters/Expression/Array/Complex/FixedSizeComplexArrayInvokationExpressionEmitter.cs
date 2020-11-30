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
	internal sealed class FixedSizeComplexArrayInvokationExpressionEmitter
		: BaseArraySerializationInvokationExpressionEmitter<FixedSizeComplexArrayTypeSerializerStrategy>
	{
		public int KnownSize { get; }

		public FixedSizeComplexArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arraySymbol, ISymbol member, int knownSize, SerializationMode mode)
			: base(arraySymbol, member, mode)
		{
			KnownSize = knownSize;
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName());
			yield return IdentifierName(ElementType.Name);
			yield return IdentifierName(new StaticlyTypedNumericNameBuilder<Int32>(KnownSize).BuildName());
		}
	}
}
