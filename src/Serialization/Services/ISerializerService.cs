using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Contract for a serialization service.
	/// </summary>
	public interface ISerializerService : ISerializationContractRegister
	{
		/// <summary>
		/// Attempts to serialize the provided <paramref name="data"/>.
		/// </summary>
		/// <typeparam name="TTypeToSerialize">Type that is being serialized (can be inferred).</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <returns>Byte array representation of the object.</returns>
		byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
			where TTypeToSerialize : new();

		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided <see cref="byte[]"/>.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="data">Byte repsentation of <typeparamref name="TTypeToDeserializeTo"/>.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data)
			where TTypeToDeserializeTo : new();

		/// <summary>
		/// Call to finalize the serialization service.
		/// This is required to serialize or deserialize types.
		/// </summary>
		void Compile();
	}
}
