using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore.Serializer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class RecordCreationExpressionEmitter : IStatementsBlockEmittable
	{
		public ITypeSymbol Symbol { get; }

		private string SerializableTypeName { get; }

		private List<StatementSyntax> DerivedStatements { get; }

		public List<StatementSyntax> BaseStatements { get; }

		private List<StatementSyntax> PreSerializationStatements { get; } = new List<StatementSyntax>();

		public RecordCreationExpressionEmitter(ITypeSymbol symbol, string serializableTypeName, List<StatementSyntax> derivedStatements, List<StatementSyntax> baseStatements)
		{
			Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
			SerializableTypeName = serializableTypeName;
			DerivedStatements = derivedStatements ?? throw new ArgumentNullException(nameof(derivedStatements));
			BaseStatements = baseStatements ?? throw new ArgumentNullException(nameof(baseStatements));
		}

		public List<StatementSyntax> CreateStatements()
		{
			var args = CreateConstructorArgs()
				.ToArray();

			var createExpression = ObjectCreationExpression
				(
					IdentifierName(SerializableTypeName)
				)
				.WithArgumentList
				(
					ArgumentList
					(
						SeparatedList<ArgumentSyntax>
						(
							args
						)
					)
				);

			if (BaseStatements.Any(bs => bs is ExpressionStatementSyntax))
				createExpression = createExpression
					.WithInitializer(CreateObjectBaseInitializerList());

			PreSerializationStatements.Add(EmitValueConstructionStatement(createExpression));

			return PreSerializationStatements;
		}

		private LocalDeclarationStatementSyntax EmitValueConstructionStatement(ObjectCreationExpressionSyntax createExpression)
		{
			return LocalDeclarationStatement
			(
				VariableDeclaration
					(
						IdentifierName(SerializableTypeName)
					)
					.WithVariables
					(
						SingletonSeparatedList<VariableDeclaratorSyntax>
						(
							VariableDeclarator
								(
									Identifier(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME)
								)
								.WithInitializer
								(
									EqualsValueClause
									(
										createExpression
									)
								)
						)
					)
			);
		}

		private InitializerExpressionSyntax CreateObjectBaseInitializerList()
		{
			return InitializerExpression
			(
				SyntaxKind.ObjectInitializerExpression,
				SeparatedList<ExpressionSyntax>
				(
					CreateInitializerListSyntaxNodeOrTokens()
				)
			);
		}

		private IEnumerable<SyntaxNodeOrToken> CreateInitializerListSyntaxNodeOrTokens()
		{
			foreach(AssignmentExpressionSyntax statement in BaseStatements
				.Where(s => s is ExpressionStatementSyntax)
				.Reverse()
				.Skip(1)
				.Reverse()
				.Cast<ExpressionStatementSyntax>()
				.Select(es => (AssignmentExpressionSyntax)es.Expression))
			{
				string fieldName = statement.Left.ToFullString().Split('.').Last();
				string tempLocalName = $"local_{fieldName}";

				CreateTempLocal(tempLocalName, statement);

				yield return AssignmentExpression
				(
					SyntaxKind.SimpleAssignmentExpression,
					IdentifierName(fieldName),
					IdentifierName(tempLocalName)
				);

				yield return Token(SyntaxKind.CommaToken);
			}

			var last = BaseStatements
				.Where(s => s is ExpressionStatementSyntax)
				.Cast<ExpressionStatementSyntax>()
				.Select(es => (AssignmentExpressionSyntax)es.Expression)
				.Last();

			foreach (var syntaxNodeOrToken in YieldLast(last)) 
				yield return syntaxNodeOrToken;
		}

		private IEnumerable<SyntaxNodeOrToken> YieldLast(AssignmentExpressionSyntax last)
		{
			string fieldName = last.Left.ToFullString().Split('.').Last();
			string tempLocalName = $"local_{fieldName}";

			CreateTempLocal(tempLocalName, last);

			yield return AssignmentExpression
			(
				SyntaxKind.SimpleAssignmentExpression,
				IdentifierName(last.Left.ToFullString().Split('.').Last()),
				IdentifierName(tempLocalName)
			);
		}

		private void CreateTempLocal(string tempLocalName, AssignmentExpressionSyntax statement)
		{
			PreSerializationStatements.Add(LocalDeclarationStatement
			(
				VariableDeclaration
					(
						IdentifierName
						(
							Identifier
							(
								TriviaList(),
								SyntaxKind.VarKeyword,
								"var",
								"var",
								TriviaList()
							)
						)
					)
					.WithVariables
					(
						SingletonSeparatedList<VariableDeclaratorSyntax>
						(
							VariableDeclarator
								(
									Identifier(tempLocalName)
								)
								.WithInitializer
								(
									EqualsValueClause
									(
										statement.Right
									)
								)
						)
					)
			));
		}

		private IEnumerable<SyntaxNodeOrToken> CreateConstructorArgs()
		{
			if (!DerivedStatements.Any(s => s is ExpressionStatementSyntax))
				yield break;

			foreach (var statement in DerivedStatements
				.Where(s => s is ExpressionStatementSyntax)
				.Reverse()
				.Skip(1)
				.Reverse()
				.Cast<ExpressionStatementSyntax>()
				.Select(es => (AssignmentExpressionSyntax)es.Expression))
			{
				yield return Argument(statement.Right);
				yield return Token(SyntaxKind.CommaToken);
			}

			var last = DerivedStatements
				.Where(s => s is ExpressionStatementSyntax)
				.Cast<ExpressionStatementSyntax>()
				.Select(es => (AssignmentExpressionSyntax)es.Expression)
				.Last();

			yield return Argument(last.Right);
		}
	}
}
