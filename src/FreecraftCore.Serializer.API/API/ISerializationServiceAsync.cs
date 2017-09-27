using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public interface ISerializationServiceAsync
	{
		/// <summary>
		/// Attempts to serialize the provided <paramref name="data"/>.
		/// </summary>
		/// <typeparam name="TTypeToSerialize">Type that is being serialized (can be inferred).</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <returns>Byte array representation of the object.</returns>
		[Pure]
		[NotNull]
		Task<byte[]> SerializeAsync<TTypeToSerialize>(TTypeToSerialize data);

		/// <summary>
		/// Attempts to serialize the provided <paramref name="data"/> with a custom writer strategy.
		/// </summary>
		/// <typeparam name="TTypeToSerialize">Type that is being serialized (can be inferred).</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <param name="writer">The writer strategy.</param>
		/// <returns>Byte array representation of the object.</returns>
		[Pure]
		[NotNull]
		Task<byte[]> SerializeAsync<TTypeToSerialize>(TTypeToSerialize data, [NotNull] IWireStreamWriterStrategyAsync writer);

		//We shouldn't expect the deserialize to provide always non-null values.
		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided <see cref="byte[]"/>.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="data">Byte repsentation of <typeparamref name="TTypeToDeserializeTo"/>.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		[Pure]
		Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>([NotNull] byte[] data);

		//We shouldn't expect the deserialize to provide always non-null values.
		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the providec custom
		/// reader.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="source">Custom reader strategy source.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		[Pure]
		Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>([NotNull] IWireStreamReaderStrategyAsync source);
	}
}
