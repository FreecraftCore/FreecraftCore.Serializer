using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace FreecraftCore.Serializer
{
	public sealed class TypeNameStringBuilder
	{
		public ITypeSymbol ActualType { get; }

		public TypeNameStringBuilder([NotNull] ITypeSymbol actualType)
		{
			ActualType = actualType ?? throw new ArgumentNullException(nameof(actualType));
		}

		public override string ToString()
		{
			if (ActualType is INamedTypeSymbol namedTypeSymbol)
				return namedTypeSymbol.GetFriendlyName();
			else
				throw new NotImplementedException($"TODO: Support {ActualType.GetType().Name} in {nameof(TypeNameStringBuilder)}");
		}
	}
}
