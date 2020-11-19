using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class OptionalFieldStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public OptionalFieldStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member) 
			: base(actualType, member, SerializationMode.None)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			OptionalAttribute attribute = Member.GetCustomAttribute<OptionalAttribute>();

			IfStatementSyntax statement = IfStatement
				(
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME),
						IdentifierName(attribute.MemberName)
					),
					ExpressionStatement
						(
							IdentifierName
							(
								MissingToken(SyntaxKind.IdentifierToken)
							)
						)
						.WithSemicolonToken
						(
							MissingToken(SyntaxKind.SemicolonToken)
						)
				)
				.WithIfKeyword
				(
					Token
					(
						TriviaList(),
						SyntaxKind.IfKeyword,
						TriviaList
						(
							Space
						)
					)
				);

			return new List<StatementSyntax>(){ statement };
		}
	}
}
