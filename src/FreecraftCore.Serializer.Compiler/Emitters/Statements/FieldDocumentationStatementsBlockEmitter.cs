using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class FieldDocumentationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public int? OptionalFieldNumber { get; init; }

		public FieldDocumentationStatementsBlockEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member)
			: base(actualType, member, SerializationMode.None)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			int fieldId = OptionalFieldNumber ?? int.Parse(Member.GetAttributeExact<WireMemberAttribute>().ConstructorArguments.First().ToCSharpString());

			//Gather all field MetaData to document it.

			//This is a HACK (including the slashes) but I am too dumb to figure out another way lol.
			statements.Add(EmptyStatement()
				.WithLeadingTrivia(Comment($"//Type: {Member.ContainingType.Name} Field: {fieldId} Name: {Member.Name} Type: {CalculateTypeName(ActualType)}")));

			return statements;
		}

		private static string CalculateTypeName(ITypeSymbol symbol)
		{
			if (String.IsNullOrWhiteSpace(symbol.Name))
			{
				if (symbol is IArrayTypeSymbol arrayTypeSymbol)
					return $"{CalculateTypeName(arrayTypeSymbol.ElementType)}[]";
				else
					throw new InvalidOperationException($"Cannot create comment TypeName for Type: {symbol.ToDisplayString()}");
			}
			else
				return symbol.Name;
		}
	}
}
