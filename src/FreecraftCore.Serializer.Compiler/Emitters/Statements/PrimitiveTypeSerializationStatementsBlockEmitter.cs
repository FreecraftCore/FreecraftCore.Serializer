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
		public PrimitiveTypeSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode)
			: base(actualType, member, mode)
		{
			if (!actualType.IsPrimitive)
				throw new InvalidOperationException($"Type: {actualType} is not a primitive type.");
		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();
			string typeName = new TypeNameStringBuilder(ActualType).ToString();

			//GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(value, destination, ref offset);
			statements.Add(new RawPrimitiveTypeSerializationGenerator(Member.Name, typeName, Mode).Create());

			return statements;
		}
	}
}
