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
	public sealed class ReadAssignmentStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		private StatementSyntax Statement { get; }

		public ReadAssignmentStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode, [NotNull] StatementSyntax statement) 
			: base(actualType, member, mode)
		{
			Statement = statement ?? throw new ArgumentNullException(nameof(statement));
		}

		public override List<StatementSyntax> CreateStatements()
		{
			return null;
		}
	}
}
