using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="IMethodBlockEmittable"/> strategy for Types that are "simple" and flat.
	/// That is that they have no polymorphic or complex serialization and are self-contained.
	/// </summary>
	/// <typeparam name="TSerializableType">The serializable type.</typeparam>
	public sealed class FlatComplexTypeSerializationMethodBlockEmitter<TSerializableType> : IMethodBlockEmittable
		where TSerializableType : new()
	{
		public BlockSyntax CreateBlock()
		{
			//Create a method scope, and insert statements into it.
			SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();

			//Conceptually, we need to find ALL serializable members
			foreach (MemberInfo mi in typeof(TSerializableType)
				.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(m => m.GetCustomAttribute<WireMemberAttribute>() != null)
				.OrderBy(m => m.GetCustomAttribute<WireMemberAttribute>().MemberOrder)) //order is important, we must emit in order!!
			{
				//Basically doesn't matter if it's a field or property, we just wanna know the Type
				//The reason is, setting and getting fields vs members are same syntax
				Type memberType = mi.GetType() == typeof(FieldInfo) ? ((FieldInfo) mi).FieldType : ((PropertyInfo) mi).PropertyType;

				FieldDocumentationStatementsBlockEmitter commentEmitter = new FieldDocumentationStatementsBlockEmitter(memberType, mi);
				statements = statements.AddRange(commentEmitter.CreateStatements());

				//The serializer is requesting we DON'T WRITE THIS! So we skip
				if (mi.GetCustomAttribute<DontWriteAttribute>() != null)
					continue;

				//We know the type, but we have to do special handling depending on on its type
				if (memberType.IsPrimitive)
				{
					//Easy case of primitive serialization
					PrimitiveTypeSerializationStatementsBlockEmitter emitter = new PrimitiveTypeSerializationStatementsBlockEmitter(memberType, mi);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType == typeof(string))
				{
					var emitter = new StringTypeSerializationStatementsBlockEmitter(memberType, mi);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType.IsArray && memberType.GetElementType().IsPrimitive)
				{
					//This is special case for primitive arrays
					var emitter = new PrimitiveArrayTypeSerializationStatementsBlockEmitter(memberType, mi);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else
					throw new NotImplementedException($"TODO: Cannot handle Type: {memberType}");

				//TODO: These don't work!!
				//Add 2 line breaks
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
			}

			return SyntaxFactory.Block(statements);
		}
	}
}
