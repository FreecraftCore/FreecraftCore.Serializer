using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class EnumParseInvocationExpressionEmitter : BaseInvokationExpressionEmitter
	{
		private InvocationExpressionSyntax Invokable { get; }

		public EnumParseInvocationExpressionEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, [NotNull] InvocationExpressionSyntax invokable) 
			: base(actualType, member, SerializationMode.None)
		{
			if (!actualType.IsEnumType())
				throw new InvalidOperationException($"Cannot use non-Enum Type: {actualType.Name} in {nameof(EnumParseInvocationExpressionEmitter)}");
			Invokable = invokable ?? throw new ArgumentNullException(nameof(invokable));
		}

		//TODO: In later versions of .NET we can one day replace internal enum extensions.
		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression
				(
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(nameof(InternalEnumExtensions)),
						GenericName
							(
								Identifier(nameof(InternalEnumExtensions.Parse))
							)
							.WithTypeArgumentList
							(
								TypeArgumentList
								(
									SingletonSeparatedList<TypeSyntax>
									(
										IdentifierName(ActualType.Name)
									)
								)
							)
					)
				)
				.WithArgumentList
				(
					ArgumentList
					(
						SeparatedList<ArgumentSyntax>
						(
							new SyntaxNodeOrToken[]
							{
								Argument(Invokable),
								Token
								(
									TriviaList(),
									SyntaxKind.CommaToken,
									TriviaList
									(
										Space
									)
								),
								Argument
								(
									LiteralExpression
									(
										SyntaxKind.TrueLiteralExpression
									)
								)
							}
						)
					)
				);
		}
	}
}
