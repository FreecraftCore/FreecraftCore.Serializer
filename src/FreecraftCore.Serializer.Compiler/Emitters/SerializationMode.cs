using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Enumeration of serialization modes.
	/// </summary>
	public enum SerializationMode
	{
		None = 0,

		/// <summary>
		/// The write mode.
		/// (Serialization)
		/// </summary>
		Write = 1,

		/// <summary>
		/// The read mode.
		/// (Deserialization)
		/// </summary>
		Read = 2,
	}
}
