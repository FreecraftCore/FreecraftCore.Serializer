using System;
using System.Collections.Generic;
using System.Linq;
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
	public sealed class OptionalFieldStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public OptionalFieldStatementsBlockEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member) 
			: base(actualType, member, SerializationMode.None)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			AttributeData attribute = Member.GetAttributeExact<OptionalAttribute>();

			IfStatementSyntax statement = IfStatement
				(
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME),

						//Important to remove quotes because nameof does this
						IdentifierName(attribute.ConstructorArguments.First().ToCSharpString().Replace("\"", ""))
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
