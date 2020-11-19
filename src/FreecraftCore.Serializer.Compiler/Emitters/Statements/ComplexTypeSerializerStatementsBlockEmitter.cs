using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class ComplexTypeSerializerStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public ComplexTypeSerializerStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			string serializerTypeName = GeneratedSerializerNameStringBuilder
				.Create(ActualType)
				.BuildName();

			RawComplexTypeSerializationGenerator generator = new RawComplexTypeSerializationGenerator(Member.Name, serializerTypeName, Mode);
			statements.Add(generator.Create());

			return statements;
		}
	}
}
