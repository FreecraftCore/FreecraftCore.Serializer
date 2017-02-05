using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Interface that types may implement to recieve events
	/// before or after serialization steps.
	/// </summary>
	public interface ISerializationEventListener
	{
		/// <summary>
		/// Invoked right before serialization occurs.
		/// </summary>
		void OnBeforeSerialization();

		/// <summary>
		/// Invoked right after deserialization.
		/// (all fields have been read at this point)
		/// </summary>
		void OnAfterDeserialization();
	}
}
