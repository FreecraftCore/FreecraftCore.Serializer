using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Lookup key for serializers that contain the context of the lookup.
	/// </summary>
	public struct ContextualSerializerLookupKey : IEqualityComparer<ContextualSerializerLookupKey>, IEquatable<ContextualSerializerLookupKey>
	{
		/// <summary>
		/// Indicates the context type for this lookup.
		/// </summary>
		public ContextTypeFlags ContextFlags { get; }

		/// <summary>
		/// Represents the context specific key.
		/// </summary>
		public IContextKey ContextSpecificKey { get; }

		/// <summary>
		/// The <see cref="Type"/> of the context.
		/// </summary>
		public Type ContextType { get; }

		public ContextualSerializerLookupKey(ContextTypeFlags flags, IContextKey contextSpecificKey, Type type)
		{
			if (contextSpecificKey == null)
				throw new ArgumentNullException(nameof(contextSpecificKey), $"Provided argument {nameof(contextSpecificKey)} is null.");

			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided argument {type} is null.");

			ContextType = type;
			ContextFlags = flags;
			ContextSpecificKey = contextSpecificKey;
		}

		public bool Equals(ContextualSerializerLookupKey x, ContextualSerializerLookupKey y)
		{
			return x.ContextType == y.ContextType && x.ContextFlags == y.ContextFlags && x.ContextSpecificKey?.GetType() == y.ContextSpecificKey?.GetType() 
				&& x.ContextSpecificKey?.Key == y.ContextSpecificKey?.Key;
		}

		public int GetHashCode(ContextualSerializerLookupKey obj)
		{
			//Hack to have multi dimensional key values to hashe to unique values, or probably hash to unique values 
			//Mentioned by the Eric Lippert: http://stackoverflow.com/questions/7924892/how-can-i-make-a-hashcode-for-a-custom-data-structure
			return obj.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return $"{this.ContextFlags.ToString()}-{this.ContextSpecificKey?.GetType()?.Name}-{this.ContextSpecificKey.Key}-{ContextType?.FullName}";
		}

		public override int GetHashCode()
		{
			return GetHashCode(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj.GetType() != this.GetType()) //call GetType because someone may dervive from this type
				return false;

			return Equals((ContextualSerializerLookupKey)obj);
		}

		public bool Equals(ContextualSerializerLookupKey other)
		{
			return Equals(this, other);
		}
	}
}
