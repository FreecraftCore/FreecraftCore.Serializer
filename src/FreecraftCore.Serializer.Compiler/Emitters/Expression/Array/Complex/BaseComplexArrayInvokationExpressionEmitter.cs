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

				TypeNameTypeCollector genericNameCollector = new TypeNameTypeCollector();
				genericNameCollector.Visit(expressionSyntax);

				//Now we analyze the expression to determine the string type information
				return genericNameCollector.Types.First();
			}
			else if (ElementType.IsEnumType())
			{
				//TODO: Enum string arrays aren't supported. This will break if they send EnumString
				//Send element type instead of array type, it's SO much easier that way! But kinda hacky
				EnumTypeSerializerStatementsBlockEmitter emitter = new EnumTypeSerializerStatementsBlockEmitter(ActualType.ElementType, Member, Mode);
				InvocationExpressionSyntax invokeSyntax = emitter.Create();

				TypeNameTypeCollector genericNameCollector = new TypeNameTypeCollector();
				genericNameCollector.Visit(invokeSyntax);

				//Now we analyze the expression to determine the Enum serializer type.
				return genericNameCollector.Types.First();
			}
			else
				return IdentifierName(GeneratedSerializerNameStringBuilder.Create(ElementType).BuildName(Member));
		}
	}
}
