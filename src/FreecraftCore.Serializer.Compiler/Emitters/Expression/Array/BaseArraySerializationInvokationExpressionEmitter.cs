using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal abstract class BaseArraySerializationInvokationExpressionEmitter<TSerializerTypeNameType> : IInvokationExpressionEmittable
		where TSerializerTypeNameType : BaseArraySerializerNonGenericMarker
	{
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
										Identifier(typeof(TSerializerTypeNameType).Name)
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
											(
												SeparatedList<TypeSyntax>
												(
													CalculateGenericParameterList()
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

		private IEnumerable<SyntaxNodeOrToken> CalculateGenericParameterList()
		{
			SyntaxNodeOrToken[] parameters = CalculateGenericTypeParameters()
				.ToArray();

			if (parameters.Length < 2)
			{
				yield return parameters[0];
				yield break;
			}

			//Yield first, rest should have comma tokens
			yield return parameters[0];

			foreach (SyntaxNodeOrToken entry in parameters.Skip(1)) //already did first
			{
				yield return Token(SyntaxKind.CommaToken);
				yield return entry;
			}
		}

		protected abstract IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters();
	}
}
