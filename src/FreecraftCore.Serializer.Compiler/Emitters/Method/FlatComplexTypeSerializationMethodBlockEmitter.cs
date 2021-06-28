using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
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

		public IList<ITypeSymbol> RequestedGenericTypes { get; } = new List<ITypeSymbol>();

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

			//Here we do the ole legacy serialization callback events.
			bool isSerializationCallbackRegistered = Symbol.ImplementsInterface<ISerializationEventListener>();
			if(isSerializationCallbackRegistered && Mode == SerializationMode.Write)
				statements = statements.Add(new SerializationCallbackInvokationExpressionEmitter(Mode).Create().ToStatement());

			//Iterate backwards from top to bottom first.
			foreach (ITypeSymbol t in typeList
				.AsEnumerable()
				.Reverse()
				.Where(t => t.HasAttributeExact<WireDataContractAttribute>()))
			{
				statements = EmitTypesMemberSerialization(t, statements, t.Equals(Symbol, SymbolEqualityComparer.Default));
			}

			//TODO: This doesn't work with Records
			//If they register Serialization Callbacks then on READ we do an After Deserialization call
			if(isSerializationCallbackRegistered && Mode == SerializationMode.Read)
				statements = statements.Add(new SerializationCallbackInvokationExpressionEmitter(Mode).Create().ToStatement());

			return SyntaxFactory.Block(statements);
		}

		private SyntaxList<StatementSyntax> EmitTypesMemberSerialization(ITypeSymbol currentType, SyntaxList<StatementSyntax> totalStatements, bool mostDerivedSymbol)
		{
			SyntaxList<StatementSyntax> statements = new SyntaxList<StatementSyntax>();

			if (currentType is INamedTypeSymbol namedSymbol)
				if (namedSymbol.IsUnboundGenericType)
					throw new InvalidOperationException($"Cannot emit member serialization for open generic Type: {namedSymbol.Name}");

			//Overriding default behavior.
			bool hasSaneDefaults = currentType.HasAttributeExact<WireSaneDefaultsAttribute>(true);

			int memberCount = 1;
			//Conceptually, we need to find ALL serializable members
			foreach (ISymbol mi in ComputeOrderedSerializableMembers(currentType)) //order is important, we must emit in order!!
			{
				//Basically doesn't matter if it's a field or property, we just wanna know the Type
				//The reason is, setting and getting fields vs members are same syntax
				ITypeSymbol memberType = GetMemberTypeInfo(mi);

				AnalyzeMemberTypeForGenerics(memberType);

				//If record we override doc checking WireMember
				FieldDocumentationStatementsBlockEmitter commentEmitter = new FieldDocumentationStatementsBlockEmitter(memberType, mi)
				{
					OptionalFieldNumber = Symbol.IsRecord ? memberCount : null
				};

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
					AttributeData attribute = mi.HasAttributeExact<CustomTypeSerializerAttribute>()
						? mi.GetAttributeExact<CustomTypeSerializerAttribute>()
						: memberType.GetAttributeExact<CustomTypeSerializerAttribute>();

					//It's DEFINITELY not null.
					OverridenSerializationGenerator emitter = new OverridenSerializationGenerator(memberType, mi, Mode, (ITypeSymbol)attribute.ConstructorArguments.First().Value);
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
				else if (memberType.SpecialType == SpecialType.System_Array || memberType is IArrayTypeSymbol)
				{
					var emitter = new ArrayTypeSerializationStatementsBlockEmitter((IArrayTypeSymbol)memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.IsEnumType()) //Enum type
				{
					var emitter = new EnumTypeSerializerStatementsBlockEmitter(memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else if (memberType.IsReferenceType && memberType.TypeKind == TypeKind.Class)
				{
					var emitter = new ComplexTypeSerializerStatementsBlockEmitter((INamedTypeSymbol)memberType, mi, Mode);
					invokeSyntax = emitter.Create();
				}
				else
				{
					bool isGenericType = currentType is INamedTypeSymbol n && n.IsGenericType;
					bool isUnboundedGenericType = currentType is INamedTypeSymbol n2 && n2.IsUnboundGenericType;
					throw new NotImplementedException($"TODO: Cannot handle Type: {memberType} ContainingType: {currentType} MetadataType: {currentType.GetType().Name} Generic: {isGenericType} UnboundGeneric: {isUnboundedGenericType}");
				}

				//Now we check if compression was requested
				if (invokeSyntax != null && mi.HasAttributeExact<CompressAttribute>())
				{
					TypeNameTypeCollector collector = new TypeNameTypeCollector();
					collector.Visit(invokeSyntax);

					//Replace the invokcation with an invokation to the compression decorator.
					FullSerializerMethodInvokationEmitter emitter = new FullSerializerMethodInvokationEmitter(Mode, $"{WoWZLibCompressionTypeSerializerDecorator.TYPE_NAME}<{collector.Types.First().ToFullString()}, {memberType.ToFullName()}>", mi);
					invokeSyntax = emitter.Create();
				}

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
				memberCount++;
			}

			//Only record mode records have special deserialization
			//We only actually create the constructor for the most derived symbol
			if (mostDerivedSymbol && Symbol.IsRecord && Mode == SerializationMode.Read)
			{
				statements = new SyntaxList<StatementSyntax>(CreateRecordInitializationStatement(statements, totalStatements));
				statements = statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME)));
				return statements;
			}

			return totalStatements.AddRange(statements);
		}

		private List<StatementSyntax> CreateRecordInitializationStatement(SyntaxList<StatementSyntax> statements, SyntaxList<StatementSyntax> baseStatements)
		{
			return new RecordCreationExpressionEmitter(Symbol, Symbol.ToFullName(), statements.ToList(), baseStatements.ToList())
				.CreateStatements();
		}

		private static IEnumerable<ISymbol> ComputeOrderedSerializableMembers(ITypeSymbol currentType)
		{
			//TODO: Record types are broken if they contain normal get/set properties. We don't treat them as initiable.
			//Special handling for records allow us to NOT use any attributes
			if (currentType.IsRecord && !currentType
				.GetMembers()
				.Where(m => !m.IsStatic)
				.Any(m => m.HasAttributeExact<WireMemberAttribute>()))
			{
				//Record types have a virtual Type EqualityContract defined. Just ignore virtuals to make this easier
				return currentType
					.GetMembers()
					.Where(m => m.Kind == SymbolKind.Property && !m.IsStatic && !m.IsVirtual && m.Name != "EqualityContract")
					.Where(t => !t.HasAttributeLike<IgnoreDataMemberAttribute>());
			}

			return currentType
				.GetMembers()
				.Where(m => !m.IsStatic)
				.Where(m => m.HasAttributeExact<WireMemberAttribute>())
				.OrderBy(m =>
				{
					//Seperated lines for debugging purposes.
					AttributeData attri = m.GetAttributeExact<WireMemberAttribute>();
					ImmutableArray<TypedConstant> attriArgs = attri.ConstructorArguments;
					string value = attriArgs.First().ToCSharpString();
					return WireMemberAttribute.Parse(value);
				});
		}

		private void AnalyzeMemberTypeForGenerics([NotNull] ITypeSymbol memberType)
		{
			if (memberType == null) throw new ArgumentNullException(nameof(memberType));

			//This is the part responsible for auto-discovering generic types
			//this allows generation of serialization for generics that haven't been defined as
			//closed types. It also supports generic element type of arrays.
			//TODO: If more generic auto-discovery is needed it should probably go here.
			if (memberType is INamedTypeSymbol nts)
			{
				if (nts.IsGenericType)
					RequestedGenericTypes.Add(nts);
			}
			else if (memberType is IArrayTypeSymbol arrayTypeSymbol)
			{
				if (arrayTypeSymbol.ElementType is INamedTypeSymbol nts2)
					if (nts2.IsGenericType)
						RequestedGenericTypes.Add(nts2);
			}
		}

		private static ITypeSymbol GetMemberTypeInfo(ISymbol mi)
		{
			return (mi is IFieldSymbol symbol) ? symbol.Type : ((IPropertySymbol) mi).Type;
		}
	}
}
