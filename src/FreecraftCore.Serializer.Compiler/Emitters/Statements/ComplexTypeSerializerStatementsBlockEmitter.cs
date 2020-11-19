using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class ComplexTypeSerializerStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public ComplexTypeSerializerStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			string serializerTypeName = GeneratedSerializerNameStringBuilder
				.Create(ActualType)
				.BuildName();

			RawComplexTypeSerializationGenerator generator = new RawComplexTypeSerializationGenerator(ActualType, Member, Mode, serializerTypeName);
			return generator.Create();
		}
	}
}
