using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for types that provide a singleton instance of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The singleton type.</typeparam>
	public interface ISingletonInstanceProvidable<out T>
	{
		/// <summary>
		/// Returns the singleton instance of a type.
		/// </summary>
		/// <returns>The singleton instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		T GetInstance();
	}
}
