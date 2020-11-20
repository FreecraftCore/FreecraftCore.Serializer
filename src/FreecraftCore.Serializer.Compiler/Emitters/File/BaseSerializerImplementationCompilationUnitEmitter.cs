using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="ICompilationUnitEmittable"/> emitter for the implementation of the serializer for the specified type.
	/// </summary>
	/// <typeparam name="TSerializableType">The type to serialize.</typeparam>
	public abstract class BaseSerializerImplementationCompilationUnitEmitter<TSerializableType> : ICompilationUnitEmittable
	{
		public string SerializerTypeName { get; } = new GeneratedSerializerNameStringBuilder<TSerializableType>().ToString();

		public string SerializableTypeName { get; } = new SerializableTypeNameStringBuilder<TSerializableType>().ToString();

		public string UnitName => SerializerTypeName;

		public CompilationUnitSyntax CreateUnit()
		{
			return CompilationUnit()
				.WithUsings
				(
					List<UsingDirectiveSyntax>
					(
						new UsingDirectiveSyntax[]
						{
							UsingDirective
								(
									IdentifierName("System")
								)
								.WithUsingKeyword
								(
									Token
									(
										TriviaList(),
										SyntaxKind.UsingKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithSemicolonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.SemicolonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								),
							UsingDirective
								(
									QualifiedName
									(
										QualifiedName
										(
											IdentifierName("System"),
											IdentifierName("Collections")
										),
										IdentifierName("Generic")
									)
								)
								.WithUsingKeyword
								(
									Token
									(
										TriviaList(),
										SyntaxKind.UsingKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithSemicolonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.SemicolonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								),
							UsingDirective
								(
									QualifiedName
									(
										QualifiedName
										(
											IdentifierName("System"),
											IdentifierName("Runtime")
										),
										IdentifierName("CompilerServices")
									)
								)
								.WithUsingKeyword
								(
									Token
									(
										TriviaList(),
										SyntaxKind.UsingKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithSemicolonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.SemicolonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								),
							UsingDirective
								(
									QualifiedName
									(
										IdentifierName("System"),
										IdentifierName("Text")
									)
								)
								.WithUsingKeyword
								(
									Token
									(
										TriviaList(),
										SyntaxKind.UsingKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithSemicolonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.SemicolonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								),
							UsingDirective
								(
									QualifiedName
									(
										IdentifierName("FreecraftCore"),
										IdentifierName("Serializer")
									)
								)
								.WithUsingKeyword
								(
									Token
									(
										TriviaList(),
										SyntaxKind.UsingKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithSemicolonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.SemicolonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								)
						}
					)
				)
				.WithMembers
				(
					List<MemberDeclarationSyntax>
					(
						CreateMembers()
					)
				);
		}

		private MemberDeclarationSyntax[] CreateMembers()
		{
			//Here we're checking if the self serializable functionality
			//must be implemented for the type, which requires additional code generation
			if (TypeExtensions.IsWireMessageType<TSerializableType>())
			{
				return new MemberDeclarationSyntax[]
				{
					new WireMessageImplementationMemberDeclarationEmitter<TSerializableType>(SerializerTypeName).Create(),
					CreateSerializerImplementationNamespaceMember(),
				};
			}
			else
			{
				return new MemberDeclarationSyntax[]
				{
					CreateSerializerImplementationNamespaceMember(),
				};
			}
		}

		protected abstract MemberDeclarationSyntax CreateSerializerImplementationNamespaceMember();
	}
}
