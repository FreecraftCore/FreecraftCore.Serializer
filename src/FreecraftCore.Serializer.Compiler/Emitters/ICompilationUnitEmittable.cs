using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore
{
	public interface ICompilationUnitEmittable
	{
		string UnitName { get; }

		CompilationUnitSyntax CreateUnit();
	}
}
