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
		public BlockSyntax CreateBlock()
		{
			//TODO: Figure out how we should decide which strategy to use, right now only simple and flat are supported.
			//TSerializableType
			return new FlatComplexTypeSerializationMethodBlockEmitter<TSerializableType>()
				.CreateBlock();
		}

		public IEnumerable<ClassDeclarationSyntax> CreateClasses()
		{
			SortedSet<int> alreadyCreatedSizeClasses = new SortedSet<int>();

			//Find all KnownSize types and emit classes for each one
			foreach (var mi in typeof(TSerializableType)
				.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				KnownSizeAttribute sizeAttribute = mi.GetCustomAttribute<KnownSizeAttribute>();
				if (sizeAttribute == null)
					continue;

				if (alreadyCreatedSizeClasses.Contains(sizeAttribute.KnownSize))
					continue;

				alreadyCreatedSizeClasses.Add(sizeAttribute.KnownSize);

				//This creates a class declaration for the int static type.
				yield return new StaticlyTypedIntegerGenericTypeClassEmitter<int>(sizeAttribute.KnownSize)
					.Create();
			}
		}
	}
}
