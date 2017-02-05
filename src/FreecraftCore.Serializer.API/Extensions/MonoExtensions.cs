using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	public static class MonoExtensions
	{
		public static bool isRunningMono()
		{
			return Type.GetType("Mono.Runtime") != null;
		}
	}
}
