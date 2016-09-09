using System;

namespace FreeCore.Payload.Serializer
{
	//This concept is based on JAM (Blizzard's messaging system/protocol and Protobuf-net's serializer strategies https://github.com/mgravell/protobuf-net/tree/master/protobuf-net/Serializers
	/// <summary>
	/// Contract for type that providing serialization strategy for the provided TType.
	/// </summary>
	public interface ITypeSerializerStrategy<TType>
	{
		/// <summary>
		/// Serializes the <typeparamref="TType">The Type this serializer can serialize.</typeparamref>
		/// </summary>
		/// <param name="toSerialize">The value/instance to serialize.</param>
		/// <returns>Array of bytes representation of the serialized value/instance.</returns>
		byte[] Serialize(TType toSerialize);
		
		/// <summary>
		/// Deserializes the byte array to a <typeparamref="TType">The Type this serializer can deserialize.</typeparamref>
		/// instance/value.
		/// </summary>
		/// <param name="toDeserialize">The value/instance to deserialize.</param>
		/// <returns>An instance/value of the Type.</returns>
		TType Deserialize(byte[] toDeserialize);
	}
}
