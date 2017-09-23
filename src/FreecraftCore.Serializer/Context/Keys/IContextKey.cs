using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for a key that "unlocks" a context.
	/// Basically the missing component between ContextFlags and the information associated with the context.
	/// </summary>
	public interface IContextKey : IEqualityComparer<IContextKey>
	{
		/// <summary>
		/// The integer the information maps to.
		/// Should be unique per state of context.
		/// </summary>
		int Key { get; }
	}
}
