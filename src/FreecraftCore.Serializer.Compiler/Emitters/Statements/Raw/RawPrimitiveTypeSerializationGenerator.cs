using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	//TODO: Create context and interface thingy.
	public sealed class RawPrimitiveTypeSerializationGenerator
	{
		public string MemberName { get; }

		public string PrimitiveTypeName { get; }

		public RawPrimitiveTypeSerializationGenerator([NotNull] string memberName, [NotNull] string primitiveTypeName)
		{
			MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
			PrimitiveTypeName = primitiveTypeName ?? throw new ArgumentNullException(nameof(primitiveTypeName));
		}

		public StatementSyntax Create()
		{
			return ExpressionStatement
			(
				InvocationExpression
					(
						MemberAccessExpression
						(
							SyntaxKind.SimpleMemberAccessExpression,
							MemberAccessExpression
							(
								SyntaxKind.SimpleMemberAccessExpression,
								GenericName
									(
										Identifier("GenericTypePrimitiveSerializerStrategy")
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
										(
											SingletonSeparatedList<TypeSyntax>
											(
												//The generic type input!
												IdentifierName(PrimitiveTypeName)
											)
										)
									),
								IdentifierName("Instance")
							),
							IdentifierName("Write")
						)
					)
					.WithArgumentList
					(
						ArgumentList
						(
							SeparatedList<ArgumentSyntax>
							(
								new SyntaxNodeOrToken[]
								{
									Argument
									(
										//This is the critical part that accesses the member and passed it for serialization.
										IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{MemberName}")
									),
									Token
									(
										TriviaList(),
										SyntaxKind.CommaToken,
										TriviaList
										(
											Space
										)
									),
									Argument
									(
										IdentifierName(CompilerConstants.OUTPUT_BUFFER_NAME)
									),
									Token
									(
										TriviaList(),
										SyntaxKind.CommaToken,
										TriviaList
										(
											Space
										)
									),
									Argument
										(
											IdentifierName(CompilerConstants.OUTPUT_OFFSET_NAME)
										)
										.WithRefKindKeyword
										(
											Token
											(
												TriviaList(),
												SyntaxKind.RefKeyword,
												TriviaList
												(
													Space
												)
											)
										)
								}
							)
						)
					)
			);
		}
	}
}
