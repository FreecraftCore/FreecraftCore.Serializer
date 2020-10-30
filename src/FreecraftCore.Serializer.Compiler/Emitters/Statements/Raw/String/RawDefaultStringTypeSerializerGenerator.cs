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
	public sealed class RawDefaultStringTypeSerializerGenerator
	{
		public string MemberName { get; }

		public EncodingType Encoding { get; }

		public bool ShouldTerminate { get; }

		public RawDefaultStringTypeSerializerGenerator([NotNull] string memberName, EncodingType encoding, bool shouldTerminate)
		{
			if (string.IsNullOrEmpty(memberName)) throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));

			MemberName = memberName;
			Encoding = encoding;
			ShouldTerminate = shouldTerminate;
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
							IdentifierName(nameof(DefaultStringSerializerHelper)),
							IdentifierName(nameof(DefaultStringSerializerHelper.Write))
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
										MemberAccessExpression
										(
											SyntaxKind.SimpleMemberAccessExpression,
											IdentifierName(nameof(EncodingType)),
											IdentifierName(Encoding.ToString())
										)
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
										LiteralExpression
										(
											ShouldTerminate ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
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
