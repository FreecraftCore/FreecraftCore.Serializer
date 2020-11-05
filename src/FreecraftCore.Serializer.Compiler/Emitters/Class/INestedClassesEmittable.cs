using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public interface INestedClassesEmittable
	{
		IEnumerable<ClassDeclarationSyntax> CreateClasses();
	}
}
