using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Encapsulates the information of a polymorphic child type linking.
	/// </summary>
	public sealed class PolymorphicTypeInfo
	{
		/// <inheritdoc />
		public string Index { get; }

		/// <summary>
		/// The child type.
		/// </summary>
		public ITypeSymbol ChildType { get; }

		public PolymorphicTypeInfo(string index, [NotNull] ITypeSymbol childType)
		{
			Index = index;
			ChildType = childType ?? throw new ArgumentNullException(nameof(childType));
		}
	}
}
