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
	public sealed class RawEnumPrimitiveSerializationGenerator : BaseInvokationExpressionEmitter
	{
		public Type PrimitiveSerializationType { get; }

		public RawEnumPrimitiveSerializationGenerator([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode,
			[NotNull] Type primitiveSerializationType) 
			: base(actualType, member, mode)
		{
			PrimitiveSerializationType = primitiveSerializationType ?? throw new ArgumentNullException(nameof(primitiveSerializationType));
		}

		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression
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
												IdentifierName(ActualType.Name),
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
