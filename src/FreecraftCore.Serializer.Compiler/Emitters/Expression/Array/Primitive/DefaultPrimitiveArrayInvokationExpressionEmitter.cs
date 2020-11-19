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
	internal sealed class DefaultPrimitiveArrayInvokationExpressionEmitter 
		: BaseArraySerializationInvokationExpressionEmitter<PrimitiveArrayTypeSerializerStrategy>
	{
		public DefaultPrimitiveArrayInvokationExpressionEmitter([NotNull] Type elementType, MemberInfo member, SerializationMode mode)
			: base(elementType, member, mode)
		{
			if (!ElementType.IsPrimitive)
				throw new InvalidOperationException($"Type: {elementType.Name} must be primitive.");
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return IdentifierName(ElementType.Name);
		}
	}
}
