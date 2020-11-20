using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public interface IMemberDeclarationEmittable
	{
		/// <summary>
		/// Creates a method body block based on the strategy.
		/// </summary>
		/// <returns>A method body block.</returns>
		MemberDeclarationSyntax Create();
	}
}
