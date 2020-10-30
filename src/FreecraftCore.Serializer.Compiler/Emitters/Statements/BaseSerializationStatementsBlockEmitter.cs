using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public abstract class BaseSerializationStatementsBlockEmitter : IStatementsBlockEmittable
	{
		/// <summary>
		/// The primitive type to emit serialization for.
		/// </summary>
		public Type PrimitiveType { get; }

		/// <summary>
		/// The member to serialize.
		/// </summary>
		public MemberInfo Member { get; }

		protected BaseSerializationStatementsBlockEmitter([NotNull] Type primitiveType, [NotNull] MemberInfo member)
		{
			PrimitiveType = primitiveType ?? throw new ArgumentNullException(nameof(primitiveType));
			Member = member ?? throw new ArgumentNullException(nameof(member));
		}

		public abstract List<StatementSyntax> CreateStatements();
	}
}
