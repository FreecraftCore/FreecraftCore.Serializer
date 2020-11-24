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
	public sealed class RawDefaultStringTypeSerializerGenerator : BaseInvokationExpressionEmitter
	{
		public EncodingType Encoding { get; }

		public bool ShouldTerminate { get; }

		public RawDefaultStringTypeSerializerGenerator([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode, 
			EncodingType encoding, bool shouldTerminate) 
			: base(actualType, member, mode)
		{
			if (!Enum.IsDefined(typeof(EncodingType), encoding)) throw new InvalidEnumArgumentException(nameof(encoding), (int) encoding, typeof(EncodingType));
			Encoding = encoding;
			ShouldTerminate = shouldTerminate;
		}

		public override InvocationExpressionSyntax Create()
		{
			//Not suppose to do this, but we will because I am lazy right now
			InvocationExpressionSyntax expressionSyntax = !ShouldTerminate
				? new SerializerMethodInvokationEmitter(Mode, $"{Encoding}StringTypeSerializerStrategy").Create()
				: new SerializerMethodInvokationEmitter(Mode, $"TerminatedStringTypeSerializerStrategy<{Encoding}StringTypeSerializerStrategy, {Encoding}StringTerminatorTypeSerializerStrategy>").Create();

			return expressionSyntax
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
