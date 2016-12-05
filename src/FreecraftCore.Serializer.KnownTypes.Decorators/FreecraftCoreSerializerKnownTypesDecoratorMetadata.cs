using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.API
{
	internal static class FreecraftCoreSerializerKnownTypesDecoratorMetadata
	{
		/// <summary>
		/// Provides the assembly for the decorator library.
		/// </summary>
		internal static Assembly Assembly { get; } = typeof(FreecraftCoreSerializerKnownTypesDecoratorMetadata).Assembly;
	}
}
