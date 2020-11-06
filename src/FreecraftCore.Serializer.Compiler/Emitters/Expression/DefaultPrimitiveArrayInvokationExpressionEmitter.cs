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
	public sealed class DefaultPrimitiveArrayInvokationExpressionEmitter : IInvokationExpressionEmittable
	{
		public Type ElementType { get; }

		public DefaultPrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType)
		{
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));

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
										Identifier(nameof(PrimitiveArrayTypeSerializerStrategy))
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
											(
												SeparatedList<TypeSyntax>
												(
													new SyntaxNodeOrToken[]
													{
														IdentifierName(ElementType.Name)
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
