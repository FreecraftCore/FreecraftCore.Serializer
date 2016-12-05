using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.API
{
#if MONO
	public static class FreecraftCoreSerializerKnownTypesDecoratorMetadata
#else
	internal static class FreecraftCoreSerializerKnownTypesDecoratorMetadata
#endif
	{
#if MONO
		public static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesDecoratorMetadata).Assembly;
#else
		internal static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesDecoratorMetadata).Assembly;
#endif

	}
}
