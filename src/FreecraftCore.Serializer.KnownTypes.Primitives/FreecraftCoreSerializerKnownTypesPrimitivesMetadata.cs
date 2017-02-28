using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer.API
{
	//TODO: Covert back to internal once I find VS2017 internalsvisibleto
#if MONO
	public static class FreecraftCoreSerializerKnownTypesPrimitivesMetadata
#else
	public static class FreecraftCoreSerializerKnownTypesPrimitivesMetadata
#endif
	{
#if MONO
		public static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesPrimitivesMetadata).Assembly;
#else
		public static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesPrimitivesMetadata).GetTypeInfo().Assembly;
#endif

	}
}
