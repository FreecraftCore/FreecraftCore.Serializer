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
	//TODO: Support generics
	public sealed class OverridenSerializationGenerator
	{
		public string MemberName { get; }

		public Type SerializerType { get; }

		public OverridenSerializationGenerator([NotNull] string memberName, [NotNull] Type serializerType)
		{
			if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(memberName));
			MemberName = memberName;
			SerializerType = serializerType ?? throw new ArgumentNullException(nameof(serializerType));
		}

		public StatementSyntax Create()
		{
			return ExpressionStatement(
				InvocationExpression(
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName(SerializerType.Name),
								IdentifierName("Instance")),
							IdentifierName("Write")))
					.WithArgumentList(
						ArgumentList(
							SeparatedList<ArgumentSyntax>(
								new SyntaxNodeOrToken[]{
									Argument(
										IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{MemberName}")),
									Token(
										TriviaList(),
										SyntaxKind.CommaToken,
										TriviaList(
											Space)),
									Argument(
										IdentifierName(CompilerConstants.OUTPUT_BUFFER_NAME)),
									Token(
										TriviaList(),
										SyntaxKind.CommaToken,
										TriviaList(
											Space)),
									Argument(
											IdentifierName(CompilerConstants.OUTPUT_OFFSET_NAME))
										.WithRefKindKeyword(
											Token(
												TriviaList(),
												SyntaxKind.RefKeyword,
												TriviaList(
													Space)))}))));
		}
	}
}
