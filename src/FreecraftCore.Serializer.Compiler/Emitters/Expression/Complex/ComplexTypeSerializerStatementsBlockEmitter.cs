using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class ComplexTypeSerializerStatementsBlockEmitter : BaseInvokationExpressionEmitter<INamedTypeSymbol>
	{
		public ComplexTypeSerializerStatementsBlockEmitter([NotNull] INamedTypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode) 
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
