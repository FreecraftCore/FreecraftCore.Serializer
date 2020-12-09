using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class DefaultComplexArrayInvokationExpressionEmitter 
		: BaseComplexArrayInvokationExpressionEmitter<ComplexArrayTypeSerializerStrategy>
	{
		public DefaultComplexArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arrayType, ISymbol member, SerializationMode mode)
			: base(arrayType, member, mode)
		{

		}
	}
}
