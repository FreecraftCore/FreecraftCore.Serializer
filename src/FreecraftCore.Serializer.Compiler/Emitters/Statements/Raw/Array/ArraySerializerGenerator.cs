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
	public sealed class ArraySerializerGenerator
	{
		public string MemberName { get; }

		private IInvokationExpressionEmittable InvokationEmitter { get; }

		public ArraySerializerGenerator([NotNull] string memberName, 
			[NotNull] IInvokationExpressionEmittable invokationEmitter)
		{
			if (string.IsNullOrEmpty(memberName)) throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
			MemberName = memberName;
			InvokationEmitter = invokationEmitter ?? throw new ArgumentNullException(nameof(invokationEmitter));
		}

		public StatementSyntax Create()
		{
			return ExpressionStatement
			(
				InvokationEmitter.Create()
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
