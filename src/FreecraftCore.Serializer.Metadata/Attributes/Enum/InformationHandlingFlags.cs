using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	[Flags]
	public enum InformationHandlingFlags : byte
	{
		/// <summary>
		/// Default behaviour.
		/// Indicates that type information should be written and read.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Indicates that the Type information should not be consumed when read.
		/// </summary>
		DontConsumeRead = 1 << 0,

		/// <summary>
		/// Indicates that the Type information should not be written.
		/// </summary>
		DontWrite = 1 << 1,
	}
}
