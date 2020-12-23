using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public class RootSerializationMethodBlockEmitter : IMethodBlockEmittable, INestedClassesEmittable
	{
		private SortedSet<int> AlreadyCreatedSizeClasses { get; } = new SortedSet<int>();

		//Kinda a hack to make this mutable, but I'd have to rewrite and redesign some stuff otherwise.
		public SerializationMode Mode { get; set; }

		public INamedTypeSymbol Symbol { get; }

		public List<ITypeSymbol> RequestedGenericTypes { get; } = new List<ITypeSymbol>();

		public RootSerializationMethodBlockEmitter([NotNull] INamedTypeSymbol symbol)
		{
			Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
		}

		public BlockSyntax CreateBlock()
		{
			//TODO: Figure out how we should decide which strategy to use, right now only simple and flat are supported.
			//TSerializableType
			var emitter = new FlatComplexTypeSerializationMethodBlockEmitter(Mode, Symbol);
			BlockSyntax block = emitter
				.CreateBlock();

			RequestedGenericTypes.AddRange(emitter.RequestedGenericTypes);
			return block;
		}

		public IEnumerable<ClassDeclarationSyntax> CreateClasses()
		{
			//We need to emit for both the current type symbol
			foreach(var newClass in EnumerateTypeSymbolRequiredClassGenerators(Symbol))
				yield return newClass;

			//and also for all its base types. If we don't, we may have missing locally scoped classes
			//that require being referenced by the serialization code.
			foreach (var baseType in Symbol.GetAllBaseTypes())
				foreach(var newClass in EnumerateTypeSymbolRequiredClassGenerators(baseType))
					yield return newClass;
		}

		private IEnumerable<ClassDeclarationSyntax> EnumerateTypeSymbolRequiredClassGenerators(ITypeSymbol typeSymbol)
		{
			//Find all KnownSize types and emit classes for each one
			foreach(var mi in typeSymbol.GetMembers()
				.Where(m => !m.IsStatic))
			{
				if (!mi.HasAttributeExact<KnownSizeAttribute>())
					continue;

				int size = int.Parse(mi.GetAttributeExact<KnownSizeAttribute>().ConstructorArguments.First().ToCSharpString());
				if (AlreadyCreatedSizeClasses.Contains(size))
					continue;

				AlreadyCreatedSizeClasses.Add(size);

				//This creates a class declaration for the int static type.
				yield return new StaticlyTypedIntegerGenericTypeClassEmitter<int>(size)
					.Create();
			}
		}
	}
}
