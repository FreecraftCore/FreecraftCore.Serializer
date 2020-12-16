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
	public static class InvocationExpressionSyntaxExtensions
	{
		/// <summary>
		/// Enclosed a provided <see cref="invocationExpression"/> to a statement.
		/// </summary>
		/// <param name="invocationExpression">The invocation</param>
		/// <returns>A statement.</returns>
		public static StatementSyntax ToStatement([NotNull] this InvocationExpressionSyntax invocationExpression)
		{
			if (invocationExpression == null) throw new ArgumentNullException(nameof(invocationExpression));

			return ExpressionStatement(invocationExpression);
		}
	}
}
