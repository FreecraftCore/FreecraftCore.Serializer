using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Key that unlocks serializers that have a [SendSize(SizeType)] context.
	/// </summary>
	public struct SendSizeContextKey : IContextKey
	{
		/// <summary>
		/// Size/key
		/// </summary>
		public int Key { get; }

		public SendSizeContextKey(SendSizeAttribute.SizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(SendSizeAttribute.SizeType), sizeType))
				throw new ArgumentException($"Provided SizeType {sizeType} was invalid.");

			Key = (int)sizeType;
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
