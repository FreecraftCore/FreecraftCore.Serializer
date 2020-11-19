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
	public sealed class ReadAssignmentStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		private ExpressionSyntax Expression { get; }

		public ReadAssignmentStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode, [NotNull] ExpressionSyntax expression) 
			: base(actualType, member, mode)
		{
			Expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();
			statements.Add(ExpressionStatement
			(
				AssignmentExpression
				(
					SyntaxKind.SimpleAssignmentExpression,
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME),
						IdentifierName(Member.Name)
					),
					Expression
				)
			));

			return statements;
		}
	}
}
