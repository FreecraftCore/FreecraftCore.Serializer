using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal class GenericNameTypeCollector : CSharpSyntaxWalker
	{
		public List<IdentifierNameSyntax> Types { get; } = new List<IdentifierNameSyntax>();

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			if (node.ToFullString().ToLower().Contains("serializer"))
				Types.Add(node);
		}
	}

	internal abstract class BaseComplexArrayInvokationExpressionEmitter<TSerializerType>
		: BaseArraySerializationInvokationExpressionEmitter<TSerializerType> 
		where TSerializerType : BaseArraySerializerNonGenericMarker
	{
		protected BaseComplexArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arrayType, ISymbol member, SerializationMode mode)
			: base(arrayType, member, mode)
		{

		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			yield return YieldSerializerTypeName();
			yield return IdentifierName(ElementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
		}

		private SyntaxNodeOrToken YieldSerializerTypeName()
		{
			//We need special case handling for string arrays.
			if (ElementType.IsTypeExact<string>())
			{
				StringTypeSerializationStatementsBlockEmitter stringSerializerEmitter = new StringTypeSerializationStatementsBlockEmitter(ActualType, Member, Mode);
				InvocationExpressionSyntax expressionSyntax = stringSerializerEmitter.Create();

				GenericNameTypeCollector genericNameCollector = new GenericNameTypeCollector();
				genericNameCollector.Visit(expressionSyntax);

				//Now we analyze the expression to determine the string type information
				return genericNameCollector.Types.First();
			}
			else
				return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName(Member));
		}
	}
}
