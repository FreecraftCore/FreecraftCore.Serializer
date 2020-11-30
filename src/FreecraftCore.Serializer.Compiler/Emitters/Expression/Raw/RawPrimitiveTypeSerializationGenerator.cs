using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	//TODO: Create context and interface thingy.
	public sealed class RawPrimitiveTypeSerializationGenerator : BaseInvokationExpressionEmitter
	{
		public string PrimitiveTypeName { get; }

		public RawPrimitiveTypeSerializationGenerator([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode, [NotNull] string primitiveTypeName) 
			: base(actualType, member, mode)
		{
			if (string.IsNullOrEmpty(primitiveTypeName)) throw new ArgumentException("Value cannot be null or empty.", nameof(primitiveTypeName));
			PrimitiveTypeName = primitiveTypeName;
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
							ComputePrimitiveTypeSerializerTypeExpression(),
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

		private NameSyntax ComputePrimitiveTypeSerializerTypeExpression()
		{
			//We use a simplistic specializer serializer for byte types.
			if (ActualType.SpecialType == SpecialType.System_Byte)
				return IdentifierName(nameof(BytePrimitiveSerializerStrategy));

			//This picks between the normal and big endian version
			SyntaxToken nameToken = Member.HasAttributeExact<ReverseDataAttribute>()
				? Identifier("GenericTypePrimitiveSerializerStrategy")
				: Identifier("BigEndianGenericTypePrimitiveSerializerStrategy");

			return GenericName
				(
					nameToken
				)
				.WithTypeArgumentList
				(
					TypeArgumentList
					(
						SingletonSeparatedList<TypeSyntax>
						(
							//The generic type input!
							IdentifierName(PrimitiveTypeName)
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
