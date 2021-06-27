using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class FixedSizePrimitiveArrayInvokationExpressionEmitter
		: BaseArraySerializationInvokationExpressionEmitter<FixedSizePrimitiveArrayTypeSerializerStrategy>
	{
		public int KnownSize { get; }

		public FixedSizePrimitiveArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arraySymbol, ISymbol member, int knownSize, SerializationMode mode)
			: base(arraySymbol, member, mode)
		{
			KnownSize = knownSize;

			if (!arraySymbol.ElementType.IsPrimitive())
				throw new InvalidOperationException($"Type: {arraySymbol.ElementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
			yield return IdentifierName(new StaticlyTypedNumericNameBuilder<Int32>(KnownSize).BuildName());
		}
	}
}
