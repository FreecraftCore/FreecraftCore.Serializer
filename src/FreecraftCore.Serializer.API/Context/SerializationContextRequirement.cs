using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Enumeration of all potential context requirements.
	/// </summary>
	public enum SerializationContextRequirement
	{
		/// <summary>
		/// Indicates that the serialization can be done without member context.
		/// </summary>
		Contextless = 0,

		/// <summary>
		/// Indicates that a certain amount of context is required for serialization.
		/// </summary>
		RequiresContext = 1
	}
}
