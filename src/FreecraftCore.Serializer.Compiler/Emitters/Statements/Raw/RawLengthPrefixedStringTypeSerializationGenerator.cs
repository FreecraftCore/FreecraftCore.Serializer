using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class RawLengthPrefixedStringTypeSerializationGenerator
	{
		public EncodingType Encoding { get; }

		public string MemberName { get; }

		public SendSizeAttribute.SizeType SizeType { get; }

		public RawLengthPrefixedStringTypeSerializationGenerator(EncodingType encoding, [NotNull] string memberName, SendSizeAttribute.SizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));
			if (string.IsNullOrEmpty(memberName)) throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
			if (!Enum.IsDefined(typeof(SendSizeAttribute.SizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(SendSizeAttribute.SizeType));

			Encoding = encoding;
			MemberName = memberName;
			SizeType = sizeType;
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
										Identifier("LengthPrefixedStringTypeSerializerStrategy")
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
										(
											SeparatedList<TypeSyntax>
											(
												new SyntaxNodeOrToken[]
												{
													IdentifierName($"{Encoding}StringTypeSerializerStrategy"),
													Token
													(
														TriviaList(),
														SyntaxKind.CommaToken,
														TriviaList
														(
															Space
														)
													),
													IdentifierName($"{Encoding}StringTerminatorTypeSerializerStrategy"),
													Token
													(
														TriviaList(),
														SyntaxKind.CommaToken,
														TriviaList
														(
															Space
														)
													),
													IdentifierName(SizeType.ToString())
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
