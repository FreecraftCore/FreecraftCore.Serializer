using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public class HideTypeAttributeBuilder
	{
		public AttributeSyntax Create()
		{
			return Attribute
				(
					QualifiedName
					(
						QualifiedName
						(
							IdentifierName("System"),
							IdentifierName("ComponentModel")
						),
						IdentifierName(nameof(EditorBrowsableAttribute))
					)
				)
				.WithArgumentList
				(
					AttributeArgumentList
					(
						SingletonSeparatedList<AttributeArgumentSyntax>
						(
							AttributeArgument
							(
								MemberAccessExpression
								(
									SyntaxKind.SimpleMemberAccessExpression,
									MemberAccessExpression
									(
										SyntaxKind.SimpleMemberAccessExpression,
										MemberAccessExpression
										(
											SyntaxKind.SimpleMemberAccessExpression,
											IdentifierName("System"),
											IdentifierName("ComponentModel")
										),
										IdentifierName(nameof(EditorBrowsableState))
									),
									IdentifierName(EditorBrowsableState.Never.ToString())
								)
							)
						)
					)
				);
		}
	}
}
