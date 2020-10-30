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
	/// <summary>
	/// Strategy for emitting primitive serialization code.
	/// </summary>
	public sealed class PrimitiveTypeSerializationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public PrimitiveTypeSerializationStatementsBlockEmitter([NotNull] Type primitiveType, [NotNull] MemberInfo member)
			: base(primitiveType, member)
		{
			if (!primitiveType.IsPrimitive)
				throw new InvalidOperationException($"Type: {primitiveType} is not a primitive type.");
		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();
			string typeName = new TypeNameStringBuilder(PrimitiveType).ToString();

			//GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(value, destination, ref offset);
			statements.Add(new RawPrimitiveTypeSerializationGenerator(Member.Name, typeName).Create());

			return statements;
		}
	}
}
