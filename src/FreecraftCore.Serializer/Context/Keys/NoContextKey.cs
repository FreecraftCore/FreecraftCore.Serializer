using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Represents a contextless context key.
	/// </summary>
	public struct NoContextKey : IContextKey
	{
		/// <summary>
		/// Singleton empty key.
		/// </summary>
		public static NoContextKey Value { get; } = new NoContextKey();

		/// <summary>
		/// Always 0 when there is no context.
		/// </summary>
		public int Key => 0;

		/// <inheritdoc />
		[Pure]
		public bool Equals(IContextKey x, IContextKey y)
		{
			return x?.GetType() == y?.GetType() && x?.Key == y?.Key;
		}

		/// <inheritdoc />
		[Pure]
		public int GetHashCode(IContextKey obj)
		{
			return $"{obj?.GetType()}-{obj?.Key}".GetHashCode();
		}
	}
}
