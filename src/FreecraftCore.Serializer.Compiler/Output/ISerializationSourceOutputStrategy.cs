using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for types that provide output strategy
	/// for serialization content.
	/// </summary>
	public interface ISerializationSourceOutputStrategy
	{
		/// <summary>
		/// Outputs serialization source.
		/// </summary>
		/// <param name="name">The name of the unit.</param>
		/// <param name="content">The string content of the serialization source.</param>
		void Output(string name, string content);
	}
}
