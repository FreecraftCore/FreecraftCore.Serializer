using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class RawComplexTypeSerializationGenerator : BaseInvokationExpressionEmitter
	{
		public string SerializerTypeName { get; }

		public RawComplexTypeSerializationGenerator([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode, [NotNull] string serializerTypeName) 
			: base(actualType, member, mode)
		{
			if (string.IsNullOrEmpty(serializerTypeName)) throw new ArgumentException("Value cannot be null or empty.", nameof(serializerTypeName));
			SerializerTypeName = serializerTypeName;
		}

		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression(
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName(SerializerTypeName),
								IdentifierName("Instance")),
							IdentifierName(Mode.ToString())))
					.WithArgumentList(
						ArgumentList(
							SeparatedList<ArgumentSyntax>(Mode == SerializationMode.Write ? ComputeWriteMethodArgs() : ComputeReadMethodArgs())));
		}

		private SyntaxNodeOrToken[] ComputeReadMethodArgs()
		{
			return new SyntaxNodeOrToken[]{
				Argument(
					IdentifierName(CompilerConstants.BUFFER_NAME)),
				Token(
					TriviaList(),
					SyntaxKind.CommaToken,
					TriviaList(
						Space)),
				Argument(
						IdentifierName(CompilerConstants.OFFSET_NAME))
					.WithRefKindKeyword(
						Token(
							TriviaList(),
							SyntaxKind.RefKeyword,
							TriviaList(
								Space)))};
		}

		private SyntaxNodeOrToken[] ComputeWriteMethodArgs()
		{
			return new SyntaxNodeOrToken[]
				{
					Argument(
						IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{Member.Name}")),
					Token(
						TriviaList(),
						SyntaxKind.CommaToken,
						TriviaList(
							Space))
				}
				.Concat(ComputeReadMethodArgs())
				.ToArray();
		}
	}
}
