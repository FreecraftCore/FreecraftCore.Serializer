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
	public sealed class SendSizePrimitiveArrayInvokationExpressionEmitter : IInvokationExpressionEmittable
	{
		public Type ElementType { get; }

		public PrimitiveSizeType SizeType { get; }

		public SendSizePrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType, PrimitiveSizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
			SizeType = sizeType;

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
										Identifier(nameof(SendSizePrimitiveArrayTypeSerializerStrategy))
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
														IdentifierName(SizeType.ToString())
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
