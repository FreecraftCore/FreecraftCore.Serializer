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
		/// The actual type to emit serialization for.
		/// </summary>
		public Type ActualType { get; }

		/// <summary>
		/// The member to serialize.
		/// </summary>
		public MemberInfo Member { get; }

		protected BaseSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member)
		{
			ActualType = actualType ?? throw new ArgumentNullException(nameof(actualType));
			Member = member ?? throw new ArgumentNullException(nameof(member));
		}

		public abstract List<StatementSyntax> CreateStatements();
	}
}
