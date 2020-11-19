using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
	public sealed class PrimitiveTypeSerializationStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public PrimitiveTypeSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode)
			: base(actualType, member, mode)
		{
			if (!actualType.IsPrimitive)
				throw new InvalidOperationException($"Type: {actualType} is not a primitive type.");
		}

		public override InvocationExpressionSyntax Create()
		{
			string typeName = new TypeNameStringBuilder(ActualType).ToString();

			//GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(value, buffer, ref offset);
			return new RawPrimitiveTypeSerializationGenerator(ActualType, Member, Mode, typeName)
				.Create();
		}
	}
}
