using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for Types that can handle polymorphic runtime registeration of types.
	/// </summary>
	/// <typeparam name="TBaseType"></typeparam>
	public interface IRuntimePolymorphicRegisterable<TBaseType>
	{
		/// <summary>
		/// Tries to link a child type with the base type.
		/// </summary>
		/// <typeparam name="TChildType">Child type to link.</typeparam>
		/// <param name="key">Key to use.</param>
		bool TryLink<TChildType>(int key)
			where TChildType : TBaseType;
	}
}
