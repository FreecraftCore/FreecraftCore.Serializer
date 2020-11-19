using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private SerializationMode Mode { get; }

		public FlatComplexTypeSerializationMethodBlockEmitter(SerializationMode mode)
		{
			if (!Enum.IsDefined(typeof(SerializationMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(SerializationMode));
			Mode = mode;
		}

		public BlockSyntax CreateBlock()
		{
			//Create a method scope, and insert statements into it.
			SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();

			List<Type> typeList = new List<Type>();
			typeList.Add(typeof(TSerializableType));
			Type currentType = typeof(TSerializableType).BaseType;

			while (currentType != null && currentType != typeof(System.Object))
			{
				typeList.Add(currentType);
				currentType = currentType.BaseType;
			}

			//Iterate backwards from top to bottom first.
			foreach (Type t in typeList
				.AsEnumerable()
				.Reverse()
				.Where(t => t.GetCustomAttribute<WireDataContractAttribute>() != null))
			{
				statements = EmitTypesMemberSerialization(t, statements);
			}

			return SyntaxFactory.Block(statements);
		}

		private SyntaxList<StatementSyntax> EmitTypesMemberSerialization(Type currentType, SyntaxList<StatementSyntax> statements)
		{
			//Conceptually, we need to find ALL serializable members
			foreach (MemberInfo mi in currentType
				.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
				.Where(m => m.GetCustomAttribute<WireMemberAttribute>() != null)
				.OrderBy(m => m.GetCustomAttribute<WireMemberAttribute>().MemberOrder)) //order is important, we must emit in order!!
			{
				//Basically doesn't matter if it's a field or property, we just wanna know the Type
				//The reason is, setting and getting fields vs members are same syntax
				Type memberType = mi.GetType() == typeof(FieldInfo) ? ((FieldInfo) mi).FieldType : ((PropertyInfo) mi).PropertyType;

				FieldDocumentationStatementsBlockEmitter commentEmitter = new FieldDocumentationStatementsBlockEmitter(memberType, mi);
				statements = statements.AddRange(commentEmitter.CreateStatements());

				//The serializer is requesting we DON'T WRITE THIS! So we skip
				if (Mode == SerializationMode.Write)
				{
					if(mi.GetCustomAttribute<DontWriteAttribute>() != null)
						continue;
				}
				else if (Mode == SerializationMode.Read)
				{
					if(mi.GetCustomAttribute<DontReadAttribute>() != null)
						continue;
				}
				
				//This handles OPTIONAL fields that may or may not be included
				if (mi.GetCustomAttribute<OptionalAttribute>() != null)
				{
					OptionalFieldStatementsBlockEmitter emitter = new OptionalFieldStatementsBlockEmitter(memberType, mi);
					statements = statements.AddRange(emitter.CreateStatements());
				}

				//We know the type, but we have to do special handling depending on on its type
				if (mi.GetCustomAttribute<CustomTypeSerializerAttribute>() != null || memberType.GetCustomAttribute<CustomTypeSerializerAttribute>() != null)
				{
					//So TYPES and PROPERTIES may both reference a custom serializer.
					//So we should prefer field/prop attributes over the type.
					CustomTypeSerializerAttribute attribute = mi.GetCustomAttribute<CustomTypeSerializerAttribute>();
					if (attribute == null)
						attribute = memberType.GetCustomAttribute<CustomTypeSerializerAttribute>();

					//It's DEFINITELY not null.
					OverridenSerializationGenerator emitter = new OverridenSerializationGenerator(memberType, mi, Mode, attribute.TypeSerializerType);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType.IsPrimitive)
				{
					//Easy case of primitive serialization
					PrimitiveTypeSerializationStatementsBlockEmitter emitter = new PrimitiveTypeSerializationStatementsBlockEmitter(memberType, mi, Mode);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType == typeof(string))
				{
					var emitter = new StringTypeSerializationStatementsBlockEmitter(memberType, mi, Mode);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType.IsArray)
				{
					var emitter = new ArrayTypeSerializationStatementsBlockEmitter(memberType, mi, Mode);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType.IsEnum)
				{
					var emitter = new EnumTypeSerializerStatementsBlockEmitter(memberType, mi, Mode);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else if (memberType.IsClass)
				{
					var emitter = new ComplexTypeSerializerStatementsBlockEmitter(memberType, mi, Mode);
					statements = statements.AddRange(emitter.CreateStatements());
				}
				else
					throw new NotImplementedException($"TODO: Cannot handle Type: {memberType}");

				//TODO: These don't work!!
				//Add 2 line breaks
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
				//statements = statements.AddRange(new EmptyLineStatementBlockEmitter().CreateStatements());
			}

			return statements;
		}
	}
}
