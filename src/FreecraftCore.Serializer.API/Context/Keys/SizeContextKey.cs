using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
				throw new ArgumentException($"Provided size {nameof(fixedSize)} was less than 0 but sizes cannot be negative.");

			Key = fixedSize;
		}

		//TODO: Override rest of methods
		public bool Equals(IContextKey x, IContextKey y)
		{
			if (x.GetType() == y.GetType())
				if (x.Key == y.Key)
					return true;

			return false;
		}

		public int GetHashCode(IContextKey obj)
		{
			return Key;
		}
	}
}
