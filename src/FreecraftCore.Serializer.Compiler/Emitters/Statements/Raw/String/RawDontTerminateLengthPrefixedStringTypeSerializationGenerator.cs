using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class RawDontTerminateLengthPrefixedStringTypeSerializationGenerator
	{
		public EncodingType Encoding { get; }

		public string MemberName { get; }

		public PrimitiveSizeType SizeType { get; }

		public SerializationMode Mode { get; }

		public RawDontTerminateLengthPrefixedStringTypeSerializationGenerator(EncodingType encoding, [NotNull] string memberName, PrimitiveSizeType sizeType, SerializationMode mode)
		{
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));
			if (string.IsNullOrEmpty(memberName)) throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));

			Encoding = encoding;
			MemberName = memberName;
			SizeType = sizeType;
			Mode = mode;
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
										Identifier("DontTerminateLengthPrefixedStringTypeSerializerStrategy")
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
													IdentifierName(SizeType.ToString())
												}
											)
										)
									),
								IdentifierName("Instance")
							),
							IdentifierName(Mode.ToString())
						)
					)
					.WithArgumentList
					(
						ArgumentList
						(
							SeparatedList<ArgumentSyntax>
							(
								Mode == SerializationMode.Write ? ComputeWriteMethodArgs() : ComputeReadMethodArgs()
							)
						)
					)
			);
		}

		private SyntaxNodeOrToken[] ComputeReadMethodArgs()
		{
			return new SyntaxNodeOrToken[]
			{
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
			};
		}

		private SyntaxNodeOrToken[] ComputeWriteMethodArgs()
		{
			return new SyntaxNodeOrToken[]
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
					)
				}
				.Concat(ComputeReadMethodArgs())
				.ToArray();
		}
	}
}
