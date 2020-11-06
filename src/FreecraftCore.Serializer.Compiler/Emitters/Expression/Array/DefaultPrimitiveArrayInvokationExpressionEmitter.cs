﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class DefaultPrimitiveArrayInvokationExpressionEmitter 
		: BaseArraySerializationInvokationExpressionEmitter<PrimitiveArrayTypeSerializerStrategy>
	{
		public Type ElementType { get; }

		public DefaultPrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType)
		{
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));

			if (!ElementType.IsPrimitive)
				throw new InvalidOperationException($"Type: {elementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.Name);
		}
	}
}
