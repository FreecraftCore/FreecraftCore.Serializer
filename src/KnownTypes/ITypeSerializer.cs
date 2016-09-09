using System;

namespace FreeCore.Payload.Serializer
{
	/// <summary>
	/// Description of ITypeSerializer.
	/// </summary>
	public interface ITypeSerializer<TType>
	{
		/// <summary>
		/// Serializes the <typeparamref="TType">The Type this serializer can serialize.</typeparamref>
		/// </summary>
		/// <param name="toSerialize">The value/instance to serialize.</param>
		/// <returns>Array of bytes representation of the serialized value/instance.</returns>
		byte[] Serialize(TType toSerialize);
	}
}
