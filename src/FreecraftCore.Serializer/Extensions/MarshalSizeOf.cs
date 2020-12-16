using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Static helper that computed and caches the <see cref="Marshal.SizeOf"/> computation
	/// for a specified <typeparamref name="TType"/>.
	/// </summary>
	/// <typeparam name="TType">The structure type to determine the size of.</typeparam>
	internal static class MarshalSizeOf<TType>
		where TType : struct
	{
		/// <summary>
		/// Indicates the size of the <typeparamref name="TType"/> struct.
		/// </summary>
		internal static int SizeOf { get; } = ComputeSizeOf();

#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		private static int ComputeSizeOf()
		{
			if(typeof(TType) == typeof(char))
				return 2;

			if(typeof(TType) == typeof(bool))
				return 1;

			return Marshal.SizeOf(typeof(TType));
		}
	}
}
