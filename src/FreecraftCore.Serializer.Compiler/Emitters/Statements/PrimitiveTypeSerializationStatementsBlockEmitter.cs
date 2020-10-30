using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Strategy for emitting primitive serialization code.
	/// </summary>
	public sealed class PrimitiveTypeSerializationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public PrimitiveTypeSerializationStatementsBlockEmitter([NotNull] Type primitiveType, [NotNull] MemberInfo member)
			: base(primitiveType, member)
		{
			if (!primitiveType.IsPrimitive)
				throw new InvalidOperationException($"Type: {primitiveType} is not a primitive type.");
		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();
			string typeName = new TypeNameStringBuilder(PrimitiveType).ToString();

			//GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(value, destination, ref offset);
			statements.Add(CreateGenericPrimitiveSerializationStatement(Member.Name, typeName));
			statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());

			return statements;
		}

		private StatementSyntax CreateGenericPrimitiveSerializationStatement([NotNull] string memberName, [NotNull] string primitiveTypeName)
		{
			if (memberName == null) throw new ArgumentNullException(nameof(memberName));
			if (primitiveTypeName == null) throw new ArgumentNullException(nameof(primitiveTypeName));

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
										Identifier("GenericTypePrimitiveSerializerStrategy")
									)
									.WithTypeArgumentList
									(
										TypeArgumentList
										(
											SingletonSeparatedList<TypeSyntax>
											(
												//The generic type input!
												IdentifierName(primitiveTypeName)
											)
										)
									),
								IdentifierName("Instance")
							),
							IdentifierName("Write")
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
										IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{memberName}")
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
