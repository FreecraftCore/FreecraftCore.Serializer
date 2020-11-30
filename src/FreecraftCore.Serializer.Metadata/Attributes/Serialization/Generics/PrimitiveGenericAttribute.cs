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
		/// Roslyn compiler exposed instace of the property.
		/// </summary>
		internal static PrimitiveGenericAttribute Instance { get; } = new PrimitiveGenericAttribute();

		//Do not remove.
		static PrimitiveGenericAttribute()
		{
			
		}

		public PrimitiveGenericAttribute()
		{
		}

		public override IEnumerator<Type> GetEnumerator()
		{
			yield return typeof(sbyte);
			yield return typeof(byte);
			yield return typeof(ushort);
			yield return typeof(short);
			yield return typeof(uint);
			yield return typeof(int);
			yield return typeof(ulong);
			yield return typeof(long);
			yield return typeof(float);
			yield return typeof(bool);
			yield return typeof(double);
		}
	}
}
