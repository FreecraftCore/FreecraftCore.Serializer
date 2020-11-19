using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public class RootSerializationMethodBlockEmitter<TSerializableType> : IMethodBlockEmittable, INestedClassesEmittable
		where TSerializableType : new()
	{
		private SortedSet<int> AlreadyCreatedSizeClasses { get; } = new SortedSet<int>();

		//Kinda a hack to make this mutable, but I'd have to rewrite and redesign some stuff otherwise.
		public SerializationMode Mode { get; set; }

		public BlockSyntax CreateBlock()
		{
			//TODO: Figure out how we should decide which strategy to use, right now only simple and flat are supported.
			//TSerializableType
			return new FlatComplexTypeSerializationMethodBlockEmitter<TSerializableType>(Mode)
				.CreateBlock();
		}

		public IEnumerable<ClassDeclarationSyntax> CreateClasses()
		{
			//Find all KnownSize types and emit classes for each one
			foreach (var mi in typeof(TSerializableType)
				.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				KnownSizeAttribute sizeAttribute = mi.GetCustomAttribute<KnownSizeAttribute>();
				if (sizeAttribute == null)
					continue;

				if (AlreadyCreatedSizeClasses.Contains(sizeAttribute.KnownSize))
					continue;

				AlreadyCreatedSizeClasses.Add(sizeAttribute.KnownSize);

				//This creates a class declaration for the int static type.
				yield return new StaticlyTypedIntegerGenericTypeClassEmitter<int>(sizeAttribute.KnownSize)
					.Create();
			}
		}
	}
}
