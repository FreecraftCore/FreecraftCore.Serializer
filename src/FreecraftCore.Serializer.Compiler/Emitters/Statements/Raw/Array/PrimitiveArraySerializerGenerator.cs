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
	public sealed class PrimitiveArraySerializerGenerator
	{
		public string MemberName { get; }

		public SendSizeAttribute.SizeType SizeType { get; }

		//It's possible array.Length is not the SIZE target.
		public string SizeAccessMemberName { get; }

		public bool ShouldWriteSize { get; }

		public PrimitiveArraySerializerGenerator([NotNull] string memberName, SendSizeAttribute.SizeType sizeType, [NotNull] string sizeAccessMemberName, bool shouldWriteSize)
		{
			if (string.IsNullOrEmpty(memberName)) throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
			if (!Enum.IsDefined(typeof(SendSizeAttribute.SizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(SendSizeAttribute.SizeType));
			MemberName = memberName;
			SizeType = sizeType;
			SizeAccessMemberName = sizeAccessMemberName ?? throw new ArgumentNullException(nameof(sizeAccessMemberName));
			ShouldWriteSize = shouldWriteSize;
		}

		public StatementSyntax Create()
		{
			//LengthPrefixedPrimitiveArraySerializerHelper.Write(value.Field, destination, ref int offset, (UInt32)Size.Access.Member);
			return ExpressionStatement
			(
				InvocationExpression
					(
						MemberAccessExpression
						(
							SyntaxKind.SimpleMemberAccessExpression,
							IdentifierName(nameof(PrimitiveArraySerializerHelper)),
							IdentifierName(nameof(PrimitiveArraySerializerHelper.Write))
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
										CastExpression
										(
											IdentifierName(SizeType.ToString()),
											IdentifierName(SizeAccessMemberName)
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
											ShouldWriteSize ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
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
