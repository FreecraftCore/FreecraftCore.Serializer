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
	/// <summary>
	/// <see cref="IStatementsBlockEmittable"/> strategy for emitting an empty line statement.
	/// </summary>
	public sealed class EmptyLineStatementBlockEmitter : IStatementsBlockEmittable
	{
		public List<StatementSyntax> CreateStatements()
		{
			return new List<StatementSyntax>
			{
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
			};
		}
	}
}
