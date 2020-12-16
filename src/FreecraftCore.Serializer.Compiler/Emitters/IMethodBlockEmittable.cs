using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for types that emit and entire Method Body Block.
	/// See: <see cref="BlockSyntax"/>.
	/// </summary>
	public interface IMethodBlockEmittable
	{
		/// <summary>
		/// Creates a method body block based on the strategy.
		/// </summary>
		/// <returns>A method body block.</returns>
		BlockSyntax CreateBlock();
	}
}
