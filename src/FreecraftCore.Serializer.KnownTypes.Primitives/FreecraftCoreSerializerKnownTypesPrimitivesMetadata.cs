using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer.API
{
#if MONO
	public static class FreecraftCoreSerializerKnownTypesPrimitivesMetadata
#else
	internal static class FreecraftCoreSerializerKnownTypesPrimitivesMetadata
#endif
	{
#if MONO
		public static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesPrimitivesMetadata).Assembly;
#else
		internal static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesPrimitivesMetadata).Assembly;
#endif

	}
}
