using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class SerializerMethodInvokationEmitter : IInvokationExpressionEmittable
	{
		private SerializationMode Mode { get; }

		public string SerializerTypeName { get; }

		public SerializerMethodInvokationEmitter(SerializationMode mode, [NotNull] string serializerTypeName)
		{
			if (!Enum.IsDefined(typeof(SerializationMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(SerializationMode));
			if (string.IsNullOrWhiteSpace(serializerTypeName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(serializerTypeName));
			Mode = mode;
			SerializerTypeName = serializerTypeName;
		}

		public InvocationExpressionSyntax Create()
		{
			return InvocationExpression
			(
				MemberAccessExpression
				(
					SyntaxKind.SimpleMemberAccessExpression,
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						IdentifierName(SerializerTypeName),
						IdentifierName("Instance")

					),
					IdentifierName(Mode.ToString())
				)
			);
		}

		public InvocationExpressionSyntax Create(bool withDefaultArgs)
		{
			if (!withDefaultArgs)
				return Create();

			return Create()
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
						IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME)
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
