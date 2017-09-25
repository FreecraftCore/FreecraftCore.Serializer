using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for Types that can handle polymorphic runtime registeration of types.
	/// </summary>
	/// <typeparam name="TBaseType"></typeparam>
	public interface IRuntimePolymorphicRegisterable<TBaseType> : IRuntimePolymorphicRegisterable
	{
		/// <summary>
		/// Tries to link a child type with the base type.
		/// </summary>
		/// <typeparam name="TChildType">Child type to link.</typeparam>
		/// <param name="key">Key to use.</param>
		bool TryLink<TChildType>(int key)
			where TChildType : TBaseType;
	}

	public interface IRuntimePolymorphicRegisterable
	{
		/// <summary>
		/// Tries to link a child type with the base type.
		/// </summary>
		/// <param name="childType">Child type to link.</param>
		/// <param name="key">Key to use.</param>
		bool TryLink(Type childType, int key);
	}
}
