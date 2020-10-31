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
	public sealed class RawEnumPrimitiveSerializationGenerator
	{
		public string MemberName { get; }

		public Type PrimitiveSerializationType { get; }

		public Type EnumType { get; }

		public RawEnumPrimitiveSerializationGenerator([NotNull] string memberName, [NotNull] Type primitiveSerializationType, [NotNull] Type enumType)
		{
			MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
			PrimitiveSerializationType = primitiveSerializationType ?? throw new ArgumentNullException(nameof(primitiveSerializationType));
			EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
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
										Identifier("GenericPrimitiveEnumTypeSerializerStrategy")
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
										(
											SeparatedList<TypeSyntax>
											(
												new SyntaxNodeOrToken[]
												{
													IdentifierName(EnumType.Name),
													Token
													(
														TriviaList(),
														SyntaxKind.CommaToken,
														TriviaList
														(
															Space
														)
													),
													IdentifierName(PrimitiveSerializationType.Name)
												}
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
