using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore
{
	/// <summary>
	/// Contract for types that emit a complete and entire compilation unit.
	/// See: <see cref="CompilationUnitSyntax"/>.
	/// </summary>
	public interface ICompilationUnitEmittable
	{
		/// <summary>
		/// The name of the unit.
		/// </summary>
		string UnitName { get; }

		/// <summary>
		/// Creates a compilation unit based on the strategy.
		/// </summary>
		/// <returns>A complete compilation unit.</returns>
		CompilationUnitSyntax CreateUnit();

		/// <summary>
		/// Provides the <see cref="ITypeSymbol"/>s of generic types
		/// that were requested for serialization.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ITypeSymbol> GetRequestedGenericTypes();
	}
}
