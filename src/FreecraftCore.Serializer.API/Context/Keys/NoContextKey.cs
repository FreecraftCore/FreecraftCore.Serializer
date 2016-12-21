using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Represents a contextless context key.
	/// </summary>
	public struct NoContextKey : IContextKey
	{
		public static NoContextKey Value { get; }

		static NoContextKey()
		{
			Value = new NoContextKey();
		}

		/// <summary>
		/// Always 0 when there is no context.
		/// </summary>
		public int Key { get { return 0; } }

		public bool Equals(IContextKey x, IContextKey y)
		{
			return x?.GetType() == y?.GetType() && x?.Key == y?.Key;
		}

		public int GetHashCode(IContextKey obj)
		{
			return $"{obj?.GetType()}-{obj?.Key}".GetHashCode();
		}
	}
}
