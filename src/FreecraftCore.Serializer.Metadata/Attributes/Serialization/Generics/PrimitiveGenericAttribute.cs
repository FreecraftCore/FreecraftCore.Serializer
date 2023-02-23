using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Attribute that marks a generic serializable type
	/// as having a specific known/forward declared closed generic Type.
	/// Specifies that serialization code should be emitted for every closed generic primitive type combination.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class PrimitiveGenericAttribute : BaseGenericListAttribute
	{
		/// <summary>
		/// Roslyn compiler exposed instance of the property.
		/// </summary>
		internal static PrimitiveGenericAttribute Instance { get; } = new PrimitiveGenericAttribute();

		internal static List<Type> CachedTypes { get; } = new()
		{
			typeof(sbyte),
			typeof(byte),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(bool),
			typeof(double),
		};

		//Do not remove.
		static PrimitiveGenericAttribute()
		{
			
		}

		public PrimitiveGenericAttribute()
		{
		}

		/// <inheritdoc />
		public override IEnumerator<Type> GetEnumerator()
		{
			return CachedTypes.GetEnumerator();
		}
	}
}
