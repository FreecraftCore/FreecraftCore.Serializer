using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	//TODO: Support generics
	public sealed class OverridenSerializationGenerator : BaseInvokationExpressionEmitter
	{
		public ITypeSymbol SerializerType { get; }

		public OverridenSerializationGenerator([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode, [NotNull] ITypeSymbol serializerType) 
			: base(actualType, member, mode)
		{
			SerializerType = serializerType ?? throw new ArgumentNullException(nameof(serializerType));
		}

		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression(
						MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								IdentifierName(CalculateSerializerTypeName()),
								IdentifierName("Instance")),
							IdentifierName(Mode.ToString())))
					.WithArgumentList(
						ArgumentList(
							SeparatedList<ArgumentSyntax>(Mode == SerializationMode.Read ? ComputeReadMethodArgs() : ComputeWriteMethodArgs())));
		}

		private string CalculateSerializerTypeName()
		{
			//Ther is a case where the serializer type may not be in the same namespace as the current serializer type
			//thereforce we must becareful!
			if (SerializerType.ContainingNamespace != null && ActualType.ContainingNamespace != null)
				if (ActualType.ContainingNamespace.FullNamespaceString() == SerializerType.ContainingNamespace.FullNamespaceString())
					return SerializerType.Name;

			if ((SerializerType.ContainingNamespace == null && ActualType.ContainingNamespace == null))
				return SerializerType.Name;

			//Full name is needed if the namespaces are different.
			return SerializerType.ToFullName();
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
