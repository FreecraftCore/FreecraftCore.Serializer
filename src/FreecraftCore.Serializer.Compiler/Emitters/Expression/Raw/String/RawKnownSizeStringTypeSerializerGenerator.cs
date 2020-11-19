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
	public sealed class RawKnownSizeStringTypeSerializerGenerator : BaseInvokationExpressionEmitter
	{
		public EncodingType Encoding { get; }

		public int FixedSizeValue { get; }

		public bool ShouldTerminate { get; }

		public RawKnownSizeStringTypeSerializerGenerator([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode, 
			EncodingType encoding, int fixedSizeValue, bool shouldTerminate) 
			: base(actualType, member, mode)
		{
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));
			if (fixedSizeValue < 0) throw new ArgumentOutOfRangeException(nameof(fixedSizeValue));
			Encoding = encoding;
			FixedSizeValue = fixedSizeValue;
			ShouldTerminate = shouldTerminate;
		}

		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression
				(
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(nameof(KnownSizeStringSerializerHelper)),
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
				);
		}

		private SyntaxNodeOrToken[] ComputeReadMethodArgs()
		{
			return new SyntaxNodeOrToken[]
			{
				Argument
				(
					IdentifierName(CompilerConstants.BUFFER_NAME)
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
						IdentifierName(CompilerConstants.OFFSET_NAME)
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
					LiteralExpression
					(
						SyntaxKind.NumericLiteralExpression,
						Literal(FixedSizeValue)
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
						//Fancy pants way to indicate to the helper if we should terminate
						ShouldTerminate ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
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
						IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{Member.Name}")
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
