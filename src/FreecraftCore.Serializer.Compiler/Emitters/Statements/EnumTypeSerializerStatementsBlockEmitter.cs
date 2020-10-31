using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class EnumTypeSerializerStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public EnumTypeSerializerStatementsBlockEmitter([NotNull] Type primitiveType, [NotNull] MemberInfo member) : base(primitiveType, member)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			EnumStringAttribute enumStringAttri = Member.GetCustomAttribute<EnumStringAttribute>();

			//TODO: Support custom primitivie specifier.
			//Primitive serialization YAY
			if (enumStringAttri == null)
			{
				var generator = new RawEnumPrimitiveSerializationGenerator(Member.Name, PrimitiveType.GetEnumUnderlyingType(), PrimitiveType);
				statements.Add(generator.Create());
			}

			return statements;
		}
	}
}
