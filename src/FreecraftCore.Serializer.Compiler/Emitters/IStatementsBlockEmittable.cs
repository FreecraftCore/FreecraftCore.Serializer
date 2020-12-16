using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for strategies that can emit statements
	/// for a method block.
	/// </summary>
	public interface IStatementsBlockEmittable
	{
		/// <summary>
		/// Creates a collection of method block statements.
		/// </summary>
		/// <returns>The method block statements.</returns>
		List<StatementSyntax> CreateStatements();
	}
}
