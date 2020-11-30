﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="IMethodBlockEmittable"/> strategy for Types that are "simple" and flat.
	/// That is that they have no polymorphic or complex serialization and are self-contained.
	/// </summary>
	public sealed class FlatComplexTypeSerializationMethodBlockEmitter : IMethodBlockEmittable
	{
		private SerializationMode Mode { get; }

		private ITypeSymbol Symbol { get; }

		public FlatComplexTypeSerializationMethodBlockEmitter(SerializationMode mode, [NotNull] ITypeSymbol symbol)
		{
			if (!Enum.IsDefined(typeof(SerializationMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(SerializationMode));
			Mode = mode;
			Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
		}

		public BlockSyntax CreateBlock()
		{
			//Create a method scope, and insert statements into it.
			SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();

			List<ITypeSymbol> typeList = new List<ITypeSymbol>(2);
			typeList.Add(Symbol);
			typeList.AddRange(Symbol.GetAllBaseTypes());

			//Iterate backwards from top to bottom first.
			foreach (ITypeSymbol t in typeList
				.AsEnumerable()
				.Reverse()
				.Where(t => t.HasAttributeExact<WireDataContractAttribute>()))
			{
				statements = EmitTypesMemberSerialization(t, statements);
			}

			return SyntaxFactory.Block(statements);
		}

		private SyntaxList<StatementSyntax> EmitTypesMemberSerialization(ITypeSymbol currentType, SyntaxList<StatementSyntax> statements)
		{
			//Conceptually, we need to find ALL serializable members
			foreach (ISymbol mi in currentType
				.GetMembers()
				.Where(m => !m.IsStatic)
				.Where(m => m.HasAttributeExact<WireMemberAttribute>())
				.OrderBy(m =>
				{
					return WireMemberAttribute.Parse(m.GetAttributeExact<WireMemberAttribute>().ConstructorArguments.First().ToCSharpString());
				})) //order is important, we must emit in order!!
			{
				//Basically doesn't matter if it's a field or property, we just wanna know the Type
				//The reason is, setting and getting fields vs members are same syntax
				ITypeSymbol memberType = GetMemberTypeInfo(mi);

				FieldDocumentationStatementsBlockEmitter commentEmitter = new FieldDocumentationStatementsBlockEmitter(memberType, mi);
				statements = statements.AddRange(commentEmitter.CreateStatements());

				//The serializer is requesting we DON'T WRITE THIS! So we skip
				if (Mode == SerializationMode.Write)
				{
					if(mi.HasAttributeExact<DontWriteAttribute>())
						continue;
				}
				else if (Mode == SerializationMode.Read)
				{
					if(mi.HasAttributeExact<DontReadAttribute>())
						continue;
				}
				
				//This handles OPTIONAL fields that may or may not be included
				if (mi.HasAttributeExact<OptionalAttribute>())
				{
					OptionalFieldStatementsBlockEmitter emitter = new OptionalFieldStatementsBlockEmitter(memberType, mi);
					statements = statements.AddRange(emitter.CreateStatements());
				}


				InvocationExpressionSyntax invokeSyntax = null;

				//We know the type, but we have to do special handling depending on on its type
				if (mi.HasAttributeExact<CustomTypeSerializerAttribute>() || memberType.HasAttributeExact<CustomTypeSerializerAttribute>())
				{
					//So TYPES and PROPERTIES may both reference a custom serializer.
					//So we should prefer field/prop attributes over the type.
					AttributeData attribute = mi.GetAttributeExact<CustomTypeSerializerAttribute>();
					if (attribute == null)
						attribute = memberType.GetAttributeExact<CustomTypeSerializerAttribute>();

					//It's DEFINITELY not null.
					OverridenSerializationGenerator emitter = new OverridenSerializationGenerator(memberType, mi, Mode, (INamedTypeSymbol)attribute.ConstructorArguments.First().Value);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.IsPrimitive())
				{
					//Easy case of primitive serialization
					PrimitiveTypeSerializationStatementsBlockEmitter emitter = new PrimitiveTypeSerializationStatementsBlockEmitter(memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.SpecialType == SpecialType.System_String)
				{
					var emitter = new StringTypeSerializationStatementsBlockEmitter(memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.SpecialType == SpecialType.System_Array)
				{
					var emitter = new ArrayTypeSerializationStatementsBlockEmitter((IArrayTypeSymbol)memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.SpecialType == SpecialType.System_Enum)
				{
					var emitter = new EnumTypeSerializerStatementsBlockEmitter(memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.IsReferenceType)
				{
					var emitter = new ComplexTypeSerializerStatementsBlockEmitter((INamedTypeSymbol)memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else
					throw new NotImplementedException($"TODO: Cannot handle Type: {memberType}");

				if (invokeSyntax != null)
				{
					if (Mode == SerializationMode.Write)
						statements = statements.Add(invokeSyntax.ToStatement());
					else if (Mode == SerializationMode.Read)
					{
						//Read generation is abit more complicated.
						//We must emit the assignment too
						ReadAssignmentStatementsBlockEmitter emitter = new ReadAssignmentStatementsBlockEmitter(memberType, mi, Mode, invokeSyntax);
						statements = statements.AddRange(emitter.CreateStatements());
					}
				}

				//TODO: These don't work!!
				//Add 2 line breaks
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
			}

			return statements;
		}

		private static ITypeSymbol GetMemberTypeInfo(ISymbol mi)
		{
			return (mi.GetType() == typeof(IFieldSymbol) ? ((IFieldSymbol) mi).Type : ((IPropertySymbol) mi).Type);
		}
	}
}
