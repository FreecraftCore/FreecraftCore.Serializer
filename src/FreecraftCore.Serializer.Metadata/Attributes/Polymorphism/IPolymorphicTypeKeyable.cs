using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for anything that provide a key that links
	/// a base and child type for polymorphism in serialization.
	/// </summary>
	public interface IPolymorphicTypeKeyable
	{
		/// <summary>
		/// Unique index/key for to link the base type to a child type.
		/// </summary>
		int Index { get; }
	}
}
