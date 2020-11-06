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
	public sealed class FixedSizePrimitiveArrayInvokationExpressionEmitter : IInvokationExpressionEmittable
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

		public InvocationExpressionSyntax Create()
		{
			return InvocationExpression
			(
				MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						MemberAccessExpression
							(
								SyntaxKind.SimpleMemberAccessExpression,
								GenericName
									(
										Identifier(nameof(FixedSizePrimitiveArrayTypeSerializerStrategy))
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
											(
												SeparatedList<TypeSyntax>
												(
													new SyntaxNodeOrToken[]
													{
														IdentifierName(ElementType.Name),
														Token(SyntaxKind.CommaToken),
														IdentifierName(new StaticlyTypedNumericNameBuilder<Int32>(KnownSize).BuildName())
													}
												)
											)
											.WithLessThanToken
											(
												Token(SyntaxKind.LessThanToken)
											)
											.WithGreaterThanToken
											(
												Token(SyntaxKind.GreaterThanToken)
											)
									),
								IdentifierName(nameof(PrimitiveArrayTypeSerializerStrategy<int>.Instance))
							)
							.WithOperatorToken
							(
								Token(SyntaxKind.DotToken)
							),
						IdentifierName(nameof(PrimitiveArrayTypeSerializerStrategy<int>.Instance.Write))
					)
					.WithOperatorToken
					(
						Token(SyntaxKind.DotToken)
					)
			);
		}
	}
}
