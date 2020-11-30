using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="ICompilationUnitEmittable"/> emitter for the implementation of the serializer for the specified type.
	/// </summary>
	public abstract class BaseSerializerImplementationCompilationUnitEmitter : ICompilationUnitEmittable
	{
		public string SerializerTypeName { get; }

		public string SerializableTypeName { get; }

		public string UnitName => SerializerTypeName;

		protected INamedTypeSymbol TypeSymbol { get; }

		protected BaseSerializerImplementationCompilationUnitEmitter([NotNull] INamedTypeSymbol typeSymbol)
		{
			TypeSymbol = typeSymbol ?? throw new ArgumentNullException(nameof(typeSymbol));
			SerializableTypeName = new SerializableTypeNameStringBuilder(TypeSymbol).ToString();
			SerializerTypeName = new GeneratedSerializerNameStringBuilder(TypeSymbol).ToString();
		}

		public CompilationUnitSyntax CreateUnit()
		{
			return CompilationUnit()
				.WithUsings
				(
					List<UsingDirectiveSyntax>
					(
						CreateUsingStatements()
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

		private IEnumerable<UsingDirectiveSyntax> CreateUsingStatements()
		{
			foreach (var u in CreateDefaultUsings())
				yield return u;

			if (TypeSymbol.ContainingNamespace != null)
				yield return UsingDirective(IdentifierName(TypeSymbol.ContainingNamespace.Name));
		}

		private static IEnumerable<UsingDirectiveSyntax> CreateDefaultUsings()
		{
			return new UsingDirectiveSyntax[]
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
			};
		}

		private MemberDeclarationSyntax[] CreateMembers()
		{
			//Here we're checking if the self serializable functionality
			//must be implemented for the type, which requires additional code generation
			if (TypeSymbol.IsWireMessageType())
			{
				return new MemberDeclarationSyntax[]
				{
					new WireMessageImplementationMemberDeclarationEmitter(SerializerTypeName, TypeSymbol).Create(),
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
