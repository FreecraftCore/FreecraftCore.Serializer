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
	public sealed class FieldDocumentationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public FieldDocumentationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member)
			: base(actualType, member)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			int fieldId = Member.GetCustomAttribute<WireMemberAttribute>().MemberOrder;

			//Gather all field MetaData to document it.

			//This is a HACK (including the slashes) but I am too dumb to figure out another way lol.
			statements.Add(EmptyStatement()
				.WithLeadingTrivia(Comment($"//Type: {Member.DeclaringType.Name} Field: {fieldId} Name: {Member.Name} Type: {ActualType.Name}")));

			return statements;
		}
	}
}
