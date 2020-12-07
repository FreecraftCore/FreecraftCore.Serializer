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
	public sealed class RawKnownSizeStringTypeSerializerGenerator : BaseStringInvokationExpressionEmitter
	{
		public int FixedSizeValue { get; }

		public bool ShouldTerminate { get; }

		public RawKnownSizeStringTypeSerializerGenerator([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode, 
			EncodingType encoding, int fixedSizeValue, bool shouldTerminate) 
			: base(actualType, member, mode, encoding)
		{
			if (fixedSizeValue < 0) throw new ArgumentOutOfRangeException(nameof(fixedSizeValue));
			FixedSizeValue = fixedSizeValue;
			ShouldTerminate = shouldTerminate;
		}

		public override InvocationExpressionSyntax Create()
		{
			return CreateSerializerInvocation()
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

		private InvocationExpressionSyntax CreateSerializerInvocation()
		{
			string serializerTypeName = !ShouldTerminate
				? $"FixedSizeStringTypeSerializerStrategy<{CalculateBaseSerializerTypeName()}, {new StaticlyTypedNumericNameBuilder<int>(FixedSizeValue).BuildName()}>"
				: $"FixedSizeStringTypeSerializerStrategy<{CalculateBaseSerializerTypeName()}, {new StaticlyTypedNumericNameBuilder<int>(FixedSizeValue).BuildName()}, {CalculateBaseSerializerTerminatorTypeName()}>";

			return new SerializerMethodInvokationEmitter(Mode, serializerTypeName)
				.Create();
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
