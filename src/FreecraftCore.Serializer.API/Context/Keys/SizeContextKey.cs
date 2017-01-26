using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Key that unlocks serializers that have a [*Size(size)] context.
	/// </summary>
	public struct SizeContextKey : IContextKey
	{
		/// <summary>
		/// Size/key
		/// </summary>
		public int Key { get; }

		public SizeContextKey(int fixedSize)
		{
			if (fixedSize < 0)
				throw new ArgumentOutOfRangeException($"Provided size {nameof(fixedSize)} was less than 0 but sizes cannot be negative.");

			Key = fixedSize;
		}

		//TODO: Override rest of methods
		/// <inheritdoc />
		[Pure]
		public bool Equals(IContextKey x, IContextKey y)
		{
			if (x.GetType() != y.GetType())
				return false;

			return x.Key == y.Key;
		}

		/// <inheritdoc />
		[Pure]
		public int GetHashCode(IContextKey obj) => Key;
	}
}
