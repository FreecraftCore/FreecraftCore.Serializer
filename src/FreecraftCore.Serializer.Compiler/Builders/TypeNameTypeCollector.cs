using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	internal class TypeNameTypeCollector : CSharpSyntaxWalker
	{
		public List<NameSyntax> Types { get; } = new List<NameSyntax>();

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			if(node.ToFullString().ToLower().Contains("serializer"))
				Types.Add(node);
		}

		public override void VisitGenericName(GenericNameSyntax node)
		{
			if(node.ToFullString().ToLower().Contains("serializer"))
				Types.Add(node);
		}
	}
}
