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
			//TODO: How can we make this NOT have a semicolon?
			return new List<StatementSyntax>
			{
				EmptyStatement()
					.WithTrailingTrivia(CarriageReturnLineFeed)
			};
		}
	}
}
