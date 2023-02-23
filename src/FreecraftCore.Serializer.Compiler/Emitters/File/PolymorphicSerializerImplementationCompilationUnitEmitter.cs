﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class PolymorphicSerializerImplementationCompilationUnitEmitter : BaseSerializerImplementationCompilationUnitEmitter
	{
		private PrimitiveSizeType PolymorphicKeySizeType { get; }

		private Compilation CompilationUnit { get; }

		private CancellationToken CancelToken { get; }

		public PolymorphicSerializerImplementationCompilationUnitEmitter([NotNull] INamedTypeSymbol typeSymbol, 
			[NotNull] Compilation compilationUnit, 
			CancellationToken cancelToken)
			: base(typeSymbol)
		{
			CompilationUnit = compilationUnit ?? throw new ArgumentNullException(nameof(compilationUnit));
			CancelToken = cancelToken;
			PolymorphicKeySizeType = InternalEnumExtensions.ParseFull<PrimitiveSizeType>(TypeSymbol.GetAttributeExact<WireDataContractAttribute>().ConstructorArguments.First().ToCSharpString(), true);
		}

		protected override MemberDeclarationSyntax CreateSerializerImplementationNamespaceMember()
		{
			return NamespaceDeclaration
				(
					QualifiedName
					(
						IdentifierName("FreecraftCore"),
						IdentifierName
						(
							Identifier
							(
								TriviaList(),
								"Serializer",
								TriviaList
								(
									CarriageReturnLineFeed
								)
							)
						)
					)
				)
				.WithNamespaceKeyword
				(
					Token
					(
						TriviaList(),
						SyntaxKind.NamespaceKeyword,
						TriviaList
						(
							Space
						)
					)
				)
				.WithOpenBraceToken
				(
					Token
					(
						TriviaList(),
						SyntaxKind.OpenBraceToken,
						TriviaList
						(
							CarriageReturnLineFeed
						)
					)
				)
				.WithMembers
				(
					SingletonList<MemberDeclarationSyntax>
					(
						BuildClassDefinition()
							.WithAttributeLists(List<AttributeListSyntax>
								(
									new AttributeListSyntax[]
									{
										AttributeList
										(
											SingletonSeparatedList<AttributeSyntax>
											(
												new HideTypeAttributeBuilder().Create()
											)
										)
									}
								)
							)
					)
				)
				.WithCloseBraceToken
				(
					Token
					(
						TriviaList(),
						SyntaxKind.CloseBraceToken,
						TriviaList
						(
							CarriageReturnLineFeed
						)
					)
				);
		}

		private ClassDeclarationSyntax BuildClassDefinition()
		{
			return ClassDeclaration
				(
					Identifier
					(
						TriviaList(),
						SerializerTypeName,
						TriviaList
						(
							new[]
							{
								Space,
								CarriageReturnLineFeed
							}
						)
					)
				)
				.WithModifiers
				(
					TokenList
					(
						new[]
						{
							Token
							(
								TriviaList
								(
									new[]
									{
										Tab,
										Comment("//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!"),
										CarriageReturnLineFeed,
										Tab,
										Trivia
										(
											DocumentationCommentTrivia
											(
												SyntaxKind.SingleLineDocumentationCommentTrivia,
												List<XmlNodeSyntax>
												(
													new XmlNodeSyntax[]
													{
														XmlText()
															.WithTextTokens
															(
																TokenList
																(
																	XmlTextLiteral
																	(
																		TriviaList
																		(
																			DocumentationCommentExterior("///")
																		),
																		" ",
																		" ",
																		TriviaList()
																	)
																)
															),
														XmlExampleElement
															(
																XmlText()
																	.WithTextTokens
																	(
																		TokenList
																		(
																			new[]
																			{
																				XmlTextNewLine
																				(
																					TriviaList(),
																					Environment.NewLine,
																					Environment.NewLine,
																					TriviaList()
																				),
																				XmlTextLiteral
																				(
																					TriviaList
																					(
																						DocumentationCommentExterior("	///")
																					),
																					" FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization",
																					" FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization",
																					TriviaList()
																				),
																				XmlTextNewLine
																				(
																					TriviaList(),
																					Environment.NewLine,
																					Environment.NewLine,
																					TriviaList()
																				),
																				XmlTextLiteral
																				(
																					TriviaList
																					(
																						DocumentationCommentExterior("	///")
																					),
																					" code for the Type: ",
																					" code for the Type: ",
																					TriviaList()
																				)
																			}
																		)
																	),
																XmlNullKeywordElement()
																	.WithAttributes
																	(
																		SingletonList<XmlAttributeSyntax>
																		(
																			XmlCrefAttribute
																			(
																				NameMemberCref
																				(
																					IdentifierName(SerializableTypeName)
																				)
																			)
																		)
																	),
																XmlText()
																	.WithTextTokens
																	(
																		TokenList
																		(
																			new[]
																			{
																				XmlTextNewLine
																				(
																					TriviaList(),
																					Environment.NewLine,
																					Environment.NewLine,
																					TriviaList()
																				),
																				XmlTextLiteral
																				(
																					TriviaList
																					(
																						DocumentationCommentExterior("	///")
																					),
																					" ",
																					" ",
																					TriviaList()
																				)
																			}
																		)
																	)
															)
															.WithStartTag
															(
																XmlElementStartTag
																(
																	XmlName
																	(
																		Identifier("summary")
																	)
																)
															)
															.WithEndTag
															(
																XmlElementEndTag
																(
																	XmlName
																	(
																		Identifier("summary")
																	)
																)
															),
														XmlText()
															.WithTextTokens
															(
																TokenList
																(
																	XmlTextNewLine
																	(
																		TriviaList(),
																		Environment.NewLine,
																		Environment.NewLine,
																		TriviaList()
																	)
																)
															)
													}
												)
											)
										),
										Tab
									}
								),
								SyntaxKind.PublicKeyword,
								TriviaList
								(
									Space
								)
							),
							Token
							(
								TriviaList(),
								SyntaxKind.SealedKeyword,
								TriviaList
								(
									Space
								)
							),
							Token
							(
								TriviaList(),
								SyntaxKind.PartialKeyword,
								TriviaList
								(
									Space
								)
							)
						}
					)
				)
				.WithKeyword
				(
					Token
					(
						TriviaList(),
						SyntaxKind.ClassKeyword,
						TriviaList
						(
							Space
						)
					)
				)
				.WithBaseList
				(
					BaseList
						(
							SingletonSeparatedList<BaseTypeSyntax>
							(
								SimpleBaseType
								(
									ComputePolymorphicBaseType()
								)
							)
						)
						.WithColonToken
						(
							Token
							(
								TriviaList
								(
									Whitespace("		")
								),
								SyntaxKind.ColonToken,
								TriviaList
								(
									Space
								)
							)
						)
				)
				.WithOpenBraceToken
				(
					Token
					(
						TriviaList
						(
							Tab
						),
						SyntaxKind.OpenBraceToken,
						TriviaList
						(
							CarriageReturnLineFeed
						)
					)
				)
				.WithMembers
				(
					SingletonList<MemberDeclarationSyntax>
					(
						MethodDeclaration
							(
								IdentifierName
								(
									Identifier
									(
										TriviaList(),
										SerializableTypeName,
										TriviaList
										(
											Space
										)
									)
								),
								Identifier("CreateType")
							)
							.WithModifiers
							(
								TokenList
								(
									new[]
									{
										Token
										(
											TriviaList
											(
												Whitespace("		")
											),
											SyntaxKind.ProtectedKeyword,
											TriviaList
											(
												Space
											)
										),
										Token
										(
											TriviaList(),
											SyntaxKind.OverrideKeyword,
											TriviaList
											(
												Space
											)
										)
									}
								)
							)
							.WithParameterList
							(
								ParameterList
									(
										SeparatedList<ParameterSyntax>
										(
											CreateCreateTypeParameterList()
										)
									)
									.WithCloseParenToken
									(
										Token
										(
											TriviaList(),
											SyntaxKind.CloseParenToken,
											TriviaList
											(
												CarriageReturnLineFeed
											)
										)
									)
							)
							.WithBody
							(
								Block
									(
										SingletonList<StatementSyntax>
										(
											SwitchStatement
												(
													IdentifierName("key")
												)
												.WithSwitchKeyword
												(
													Token
													(
														TriviaList
														(
															Whitespace("			")
														),
														SyntaxKind.SwitchKeyword,
														TriviaList
														(
															Space
														)
													)
												)
												.WithCloseParenToken
												(
													Token
													(
														TriviaList(),
														SyntaxKind.CloseParenToken,
														TriviaList
														(
															CarriageReturnLineFeed
														)
													)
												)
												.WithOpenBraceToken
												(
													Token
													(
														TriviaList
														(
															Whitespace("			")
														),
														SyntaxKind.OpenBraceToken,
														TriviaList
														(
															CarriageReturnLineFeed
														)
													)
												)
												.WithSections
												(
													List<SwitchSectionSyntax>
													(
														BuildSwitchStatements()
													)
												)
												.WithCloseBraceToken
												(
													Token
													(
														TriviaList
														(
															Whitespace("			")
														),
														SyntaxKind.CloseBraceToken,
														TriviaList
														(
															CarriageReturnLineFeed
														)
													)
												)
										)
									)
									.WithOpenBraceToken
									(
										Token
										(
											TriviaList
											(
												Whitespace("		")
											),
											SyntaxKind.OpenBraceToken,
											TriviaList
											(
												CarriageReturnLineFeed
											)
										)
									)
									.WithCloseBraceToken
									(
										Token
										(
											TriviaList
											(
												Whitespace("		")
											),
											SyntaxKind.CloseBraceToken,
											TriviaList
											(
												CarriageReturnLineFeed
											)
										)
									)
							)
					)
				)
				.WithCloseBraceToken
				(
					Token
					(
						TriviaList
						(
							Tab
						),
						SyntaxKind.CloseBraceToken,
						TriviaList
						(
							CarriageReturnLineFeed
						)
					)
				);
		}

		private SyntaxNodeOrToken[] CreateCreateTypeParameterList()
		{
			//Record uses different list
			if (this.TypeSymbol.IsRecord)
			{
				return new SyntaxNodeOrToken[]
				{
					Parameter
						(
							Identifier("key")
						)
						.WithType
						(
							PredefinedType
							(
								Token
								(
									TriviaList(),
									SyntaxKind.IntKeyword,
									TriviaList
									(
										Space
									)
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
					Parameter
						(
							Identifier(CompilerConstants.BUFFER_NAME)
						)
						.WithType
						(
							GenericName
								(
									Identifier("Span")
								)
								.WithTypeArgumentList
								(
									TypeArgumentList
										(
											SingletonSeparatedList<TypeSyntax>
											(
												PredefinedType
												(
													Token(SyntaxKind.ByteKeyword)
												)
											)
										)
										.WithGreaterThanToken
										(
											Token
											(
												TriviaList(),
												SyntaxKind.GreaterThanToken,
												TriviaList
												(
													Space
												)
											)
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
					Parameter
						(
							Identifier(CompilerConstants.OFFSET_NAME)
						)
						.WithModifiers
						(
							TokenList
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
						)
						.WithType
						(
							PredefinedType
							(
								Token
								(
									TriviaList(),
									SyntaxKind.IntKeyword,
									TriviaList
									(
										Space
									)
								)
							)
						)
				};
			}

			return new SyntaxNodeOrToken[]
			{
				Parameter
					(
						Identifier("key")
					)
					.WithType
					(
						PredefinedType
						(
							Token
							(
								TriviaList(),
								SyntaxKind.IntKeyword,
								TriviaList
								(
									Space
								)
							)
						)
					)
			};
		}

		private GenericNameSyntax ComputePolymorphicBaseType()
		{
			return GenericName
				(
					Identifier(TypeSymbol.IsRecord ? "BasePolymorphicAutoGeneratedRecordSerializerStrategy" : "BasePolymorphicAutoGeneratedSerializerStrategy")
				)
				.WithTypeArgumentList
				(
					TypeArgumentList
						(
							SeparatedList<TypeSyntax>
							(
								PolymorphicBaseTypeGenericParameters()
							)
						)
						.WithGreaterThanToken
						(
							Token
							(
								TriviaList(),
								SyntaxKind.GreaterThanToken,
								TriviaList
								(
									CarriageReturnLineFeed
								)
							)
						)
				);
		}

		private IEnumerable<SyntaxNodeOrToken> PolymorphicBaseTypeGenericParameters()
		{
			yield return IdentifierName(SerializerTypeName);
			yield return Token
			(
				TriviaList(),
				SyntaxKind.CommaToken,
				TriviaList
				(
					Space
				)
			);

			yield return IdentifierName(SerializableTypeName);
			yield return Token
			(
				TriviaList(),
				SyntaxKind.CommaToken,
				TriviaList
				(
					Space
				)
			);

			//There is no longer a Bit type so we just sub in Byte instead.
			if (PolymorphicKeySizeType != PrimitiveSizeType.Bit)
				yield return IdentifierName(PolymorphicKeySizeType.ToString());
			else
				yield return IdentifierName(PrimitiveSizeType.Byte.ToString());

			if (PolymorphicKeySizeType == PrimitiveSizeType.Bit)
			{
				yield return Token
				(
					TriviaList(),
					SyntaxKind.CommaToken,
					TriviaList
					(
						Space
					)
				);

				yield return IdentifierName(nameof(BitSerializerStrategy));
			}
		}

		public override IEnumerable<ITypeSymbol> GetRequestedGenericTypes()
		{
			return Array.Empty<ITypeSymbol>();
		}

		private IEnumerable<SwitchSectionSyntax> BuildSwitchStatements()
		{
			if (!this.TypeSymbol.IsRecord)
				foreach (var switchSectionSyntax in EmitClassSwitchStatements()) 
					yield return switchSectionSyntax;
			else
				foreach(var switchSectionSyntax in EmitRecordSwitchStatements())
					yield return switchSectionSyntax;
		}

		private IEnumerable<SwitchSectionSyntax> EmitRecordSwitchStatements()
		{
			//Every type that is a child type in the same assembly
			foreach(PolymorphicTypeInfo typeInfo in GetPolymorphicChildTypes())
			{
				if (CancelToken.IsCancellationRequested)
					yield break;

				yield return SwitchSection()
					.WithLabels
					(
						SingletonList<SwitchLabelSyntax>
						(
							CaseSwitchLabel
								(
									CreateSwitchCase(typeInfo)
								)
								.WithKeyword
								(
									Token
									(
										TriviaList
										(
											Whitespace("				")
										),
										SyntaxKind.CaseKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithColonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.ColonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								)
						)
					)
					.WithStatements
					(
						SingletonList<StatementSyntax>
						(
							ReturnStatement
								(
									new SerializerMethodInvokationEmitter(SerializationMode.Read, new GeneratedSerializerNameStringBuilder(typeInfo.ChildType).BuildName())
										.Create(true)
								)
								.WithReturnKeyword
								(
									Token
									(
										TriviaList
										(
											Whitespace("					")
										),
										SyntaxKind.ReturnKeyword,
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
						)
					);
			}

			if(TypeSymbol.HasAttributeExact<DefaultChildAttribute>())
			{
				//TODO: We need a way to WARN consumers of the library about this.
				//We cannot emit a creation for default child if they're not marked as serializable via the WireDataContract
				//This will produce code that dangerously recurrs into a stack overflow since base WireMessage won't be overrided.
				ITypeSymbol childTypeSymbol = (ITypeSymbol)TypeSymbol.GetAttributeExact<DefaultChildAttribute>().ConstructorArguments.First().Value;

				if(childTypeSymbol.HasAttributeLike<WireDataContractAttribute>())
					yield return SwitchSection()
						.WithLabels
						(
							SingletonList<SwitchLabelSyntax>
							(
								DefaultSwitchLabel()
									.WithKeyword
									(
										Token
										(
											TriviaList
											(
												Whitespace("				")
											),
											SyntaxKind.DefaultKeyword,
											TriviaList()
										)
									)
									.WithColonToken
									(
										Token
										(
											TriviaList(),
											SyntaxKind.ColonToken,
											TriviaList
											(
												CarriageReturnLineFeed
											)
										)
									)
							)
						)
						.WithStatements
						(
							SingletonList<StatementSyntax>
							(
								ReturnStatement
									(
										new SerializerMethodInvokationEmitter(SerializationMode.Read, new GeneratedSerializerNameStringBuilder(childTypeSymbol).BuildName())
											.Create(true)
									)
									.WithReturnKeyword
									(
										Token
										(
											TriviaList
											(
												Whitespace("					")
											),
											SyntaxKind.ReturnKeyword,
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
							)
						);
				else
					yield return EmitNoDefaultChildThrow();
			}
			else
			{
				yield return EmitNoDefaultChildThrow();
			}
		}


		private IEnumerable<SwitchSectionSyntax> EmitClassSwitchStatements()
		{
			//Every type that is a child type in the same assembly
			foreach (PolymorphicTypeInfo typeInfo in GetPolymorphicChildTypes())
			{
				if (CancelToken.IsCancellationRequested)
					yield break;

				yield return SwitchSection()
					.WithLabels
					(
						SingletonList<SwitchLabelSyntax>
						(
							CaseSwitchLabel
								(
									CreateSwitchCase(typeInfo)
								)
								.WithKeyword
								(
									Token
									(
										TriviaList
										(
											Whitespace("				")
										),
										SyntaxKind.CaseKeyword,
										TriviaList
										(
											Space
										)
									)
								)
								.WithColonToken
								(
									Token
									(
										TriviaList(),
										SyntaxKind.ColonToken,
										TriviaList
										(
											CarriageReturnLineFeed
										)
									)
								)
						)
					)
					.WithStatements
					(
						SingletonList<StatementSyntax>
						(
							ReturnStatement
								(
									ObjectCreationExpression
										(
											CreateChildTypeIdentifier(typeInfo.ChildType)
										)
										.WithNewKeyword
										(
											Token
											(
												TriviaList(),
												SyntaxKind.NewKeyword,
												TriviaList
												(
													Space
												)
											)
										)
										.WithArgumentList
										(
											ArgumentList()
										)
								)
								.WithReturnKeyword
								(
									Token
									(
										TriviaList
										(
											Whitespace("					")
										),
										SyntaxKind.ReturnKeyword,
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
						)
					);
			}


			if (CancelToken.IsCancellationRequested)
				yield break;

			if (TypeSymbol.HasAttributeExact<DefaultChildAttribute>())
			{
				//TODO: We need a way to WARN consumers of the library about this.
				//We cannot emit a creation for default child if they're not marked as serializable via the WireDataContract
				//This will produce code that dangerously recurrs into a stack overflow since base WireMessage won't be overrided.
				ITypeSymbol childTypeSymbol = (ITypeSymbol) TypeSymbol.GetAttributeExact<DefaultChildAttribute>().ConstructorArguments.First().Value;

				if (childTypeSymbol.HasAttributeLike<WireDataContractAttribute>())
					yield return SwitchSection()
						.WithLabels
						(
							SingletonList<SwitchLabelSyntax>
							(
								DefaultSwitchLabel()
									.WithKeyword
									(
										Token
										(
											TriviaList
											(
												Whitespace("				")
											),
											SyntaxKind.DefaultKeyword,
											TriviaList()
										)
									)
									.WithColonToken
									(
										Token
										(
											TriviaList(),
											SyntaxKind.ColonToken,
											TriviaList
											(
												CarriageReturnLineFeed
											)
										)
									)
							)
						)
						.WithStatements
						(
							SingletonList<StatementSyntax>
							(
								ReturnStatement
									(
										ObjectCreationExpression
											(
												IdentifierName(childTypeSymbol.Name)
											)
											.WithNewKeyword
											(
												Token
												(
													TriviaList(),
													SyntaxKind.NewKeyword,
													TriviaList
													(
														Space
													)
												)
											)
											.WithArgumentList
											(
												ArgumentList()
											)
									)
									.WithReturnKeyword
									(
										Token
										(
											TriviaList
											(
												Whitespace("					")
											),
											SyntaxKind.ReturnKeyword,
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
							)
						);
				else
					yield return EmitNoDefaultChildThrow();
			}
			else
			{
				yield return EmitNoDefaultChildThrow();
			}
		}

		private SwitchSectionSyntax EmitNoDefaultChildThrow()
		{
			return SwitchSection()
				.WithLabels
				(
					SingletonList<SwitchLabelSyntax>
					(
						DefaultSwitchLabel()
							.WithKeyword
							(
								Token
								(
									TriviaList
									(
										Whitespace("				")
									),
									SyntaxKind.DefaultKeyword,
									TriviaList()
								)
							)
							.WithColonToken
							(
								Token
								(
									TriviaList(),
									SyntaxKind.ColonToken,
									TriviaList
									(
										CarriageReturnLineFeed
									)
								)
							)
					)
				)
				.WithStatements
				(
					SingletonList<StatementSyntax>
					(
						ThrowStatement
							(
								ObjectCreationExpression
									(
										IdentifierName(nameof(NotImplementedException))
									)
									.WithNewKeyword
									(
										Token
										(
											TriviaList(),
											SyntaxKind.NewKeyword,
											TriviaList
											(
												Space
											)
										)
									)
									.WithArgumentList
									(
										ArgumentList
										(
											SingletonSeparatedList<ArgumentSyntax>
											(
												Argument
												(
													InterpolatedStringExpression
														(
															Token(SyntaxKind.InterpolatedStringStartToken)
														)
														.WithContents
														(
															List<InterpolatedStringContentSyntax>
															(
																new InterpolatedStringContentSyntax[]
																{
																	InterpolatedStringText()
																		.WithTextToken
																		(
																			Token
																			(
																				TriviaList(),
																				SyntaxKind.InterpolatedStringTextToken,
																				"Encountered unimplemented sub-type for Type: ",
																				"Encountered unimplemented sub-type for Type: ",
																				TriviaList()
																			)
																		),
																	Interpolation
																	(
																		InvocationExpression
																			(
																				IdentifierName("nameof")
																			)
																			.WithArgumentList
																			(
																				ArgumentList
																				(
																					SingletonSeparatedList<ArgumentSyntax>
																					(
																						Argument
																						(
																							IdentifierName(SerializableTypeName)
																						)
																					)
																				)
																			)
																	),
																	InterpolatedStringText()
																		.WithTextToken
																		(
																			Token
																			(
																				TriviaList(),
																				SyntaxKind.InterpolatedStringTextToken,
																				" with Key: ",
																				" with Key: ",
																				TriviaList()
																			)
																		),
																	Interpolation
																	(
																		IdentifierName("key")
																	)
																}
															)
														)
												)
											)
										)
									)
							)
							.WithThrowKeyword
							(
								Token
								(
									TriviaList(),
									SyntaxKind.ThrowKeyword,
									TriviaList
									(
										Space
									)
								)
							)
					)
				);
		}

		private static ExpressionSyntax CreateSwitchCase(PolymorphicTypeInfo typeInfo)
		{
			if (int.TryParse(typeInfo.Index, out int index))
			{
				return LiteralExpression
				(
					SyntaxKind.NumericLiteralExpression,
					Literal(index)
				);
			}
			else
			{
				return CastExpression
				(
					PredefinedType
					(
						Token(SyntaxKind.IntKeyword)
					),
					IdentifierName(typeInfo.Index)
				);
			}
		}

		private IEnumerable<PolymorphicTypeInfo> GetPolymorphicChildTypes()
		{
			//Records have special handling optionally with WireDataContractBaseRecordLinkAttribute
			if (this.TypeSymbol.IsRecord && TypeSymbol.HasAttributeExact<WireDataContractRecordSemanticLinkAttribute>())
			{
				//Gets child types with link attribute
				//and also types that are directly defined as attributed on the base type itself.
				return CompilationUnit
					.GetAllTypes()
					.Where(t => !t.Equals(TypeSymbol, SymbolEqualityComparer.Default) && t.BaseType != null && t.BaseType.Equals(TypeSymbol, SymbolEqualityComparer.Default))
					.Where(t => t.HasAttributeExact<WireDataContractAttribute>() && t.IsRecord)
					.Select(t =>
					{
						//var collector = new InvocationExpressionCollector();
						//Compiler will emit Record base ctor in IL as last invokation in ctor
						var recordSyntax = t.DeclaringSyntaxReferences
							.First()
							.GetSyntax()
							.DescendantNodesAndTokensAndSelf(node => true, false)
							.Where(n => Test(n))
							.Select(n => (RecordDeclarationSyntax) n.AsNode())
							.First();

						if (recordSyntax.BaseList != null)
						{
							ExpressionSyntax primaryBaseCtorFirstArg = recordSyntax.BaseList
								.DescendantNodesAndSelf(node => true, true)
								.First(n => n is PrimaryConstructorBaseTypeSyntax)
								.DescendantNodesAndSelf(node => true)
								.OfType<LiteralExpressionSyntax>()
								.FirstOrDefault();

							//Maybe it's an enum
							if (primaryBaseCtorFirstArg == null)
								primaryBaseCtorFirstArg = recordSyntax.BaseList
									.DescendantNodesAndSelf(node => true, true)
									.Where(n => n is PrimaryConstructorBaseTypeSyntax)
									.First()
									.DescendantNodesAndSelf(node => true)
									.OfType<MemberAccessExpressionSyntax>()
									.FirstOrDefault();

							return new PolymorphicTypeInfo(primaryBaseCtorFirstArg.ToFullString(), t);
						}

						return null;
					})
					.Where(t => t != null)
					.Distinct();
			}

			//Gets child types with link attribute
			//and also types that are directly defined as attributed on the base type itself.
			return CompilationUnit
				.GetAllTypes()
				.Where(t => !t.Equals(TypeSymbol, SymbolEqualityComparer.Default) && t.BaseType != null && t.BaseType.Equals(TypeSymbol, SymbolEqualityComparer.Default))
				.Where(t => t.HasAttributeExact<WireDataContractAttribute>() && t.HasAttributeLike<WireDataContractBaseLinkAttribute>())
				.Select(t =>
				{
					AttributeData attribute = t.GetAttributeLike<WireDataContractBaseLinkAttribute>();
					return new PolymorphicTypeInfo(attribute.ConstructorArguments.First().ToCSharpString(), t);
				})
				.Concat(TypeSymbol.GetAttributesLike<WireDataContractBaseTypeAttribute>().Select(a =>
				{
					return new PolymorphicTypeInfo(a.ConstructorArguments.First().ToCSharpString(), (ITypeSymbol)a.ConstructorArguments.Last().Value);
				}))
				.Distinct();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool Test(SyntaxNodeOrToken n)
		{
			return n.IsNode && n.AsNode().Kind() == SyntaxKind.RecordDeclaration;
		}

		private IdentifierNameSyntax CreateChildTypeIdentifier([NotNull] ITypeSymbol childType)
		{
			if (childType == null) throw new ArgumentNullException(nameof(childType));

			string baseTypeNameSpace = TypeSymbol
				.ContainingNamespace?.FullNamespaceString();

			if (childType.ContainingNamespace == null || baseTypeNameSpace == null)
				return IdentifierName(childType.Name);

			return childType.ContainingNamespace.FullNamespaceString().StartsWith(baseTypeNameSpace) ? IdentifierName(childType.Name) : IdentifierName($"{childType.ContainingNamespace.FullNamespaceString()}.{childType.Name}");
		}
	}
}
