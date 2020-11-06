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
	internal sealed class FixedSizePrimitiveArrayInvokationExpressionEmitter
		: BaseArraySerializationInvokationExpressionEmitter<FixedSizePrimitiveArrayTypeSerializerStrategy>
	{
		public Type ElementType { get; }

		public int KnownSize { get; }

		public FixedSizePrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType, int knownSize)
		{
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
			KnownSize = knownSize;

			if (!ElementType.IsPrimitive)
				throw new InvalidOperationException($"Type: {elementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.Name);
			yield return IdentifierName(new StaticlyTypedNumericNameBuilder<Int32>(KnownSize).BuildName());
		}
	}
}
