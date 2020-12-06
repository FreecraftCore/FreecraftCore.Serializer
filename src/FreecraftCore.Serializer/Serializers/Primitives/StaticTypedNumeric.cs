using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for static numerics.
	/// </summary>
	/// <typeparam name="TNumericType"></typeparam>
	public abstract class StaticTypedNumeric<TNumericType>
		where TNumericType : unmanaged
	{
		/// <summary>
		/// The literal/constant value.
		/// </summary>
		public abstract TNumericType Value { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

		protected StaticTypedNumeric()
		{
			
		}
	}
}
