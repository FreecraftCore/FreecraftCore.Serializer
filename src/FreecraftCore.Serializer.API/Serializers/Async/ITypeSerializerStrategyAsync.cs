using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface ITypeSerializerStrategyAsync : ITypeSerializerStrategy
	{
		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		[Pure]
		Task WriteAsync(object value, [NotNull] IWireStreamWriterStrategy dest);

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		[Pure]
		[NotNull]
		Task<object> ReadAsync([NotNull] IWireStreamReaderStrategy source);

		//TODO: Fix doc
		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		Task<object> ReadIntoObjectAsync([CanBeNull] ref object obj, [NotNull] IWireStreamReaderStrategy source);

		//TODO: Fix doc
		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		Task ObjectIntoWriterAsync([CanBeNull] object obj, [NotNull] IWireStreamWriterStrategy dest);
	}

	//This concept is based on JAM (Blizzard's messaging system/protocol and Protobuf-net's serializer strategies https://github.com/mgravell/protobuf-net/tree/master/protobuf-net/Serializers
	/// <summary>
	/// Contract for type that providing serialization strategy for the provided TType.
	/// </summary>
	public interface ITypeSerializerStrategyAsync<TType> : ITypeSerializerStrategy<TType>, ITypeSerializerStrategyAsync
	{
		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		[Pure]
		Task WriteAsync(TType value, [NotNull] IWireStreamWriterStrategy dest);

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		[Pure]
		[NotNull]
		new Task<TType> ReadAsync([NotNull] IWireStreamReaderStrategy source);

		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		Task<TType> ReadIntoObjectAsync([CanBeNull] ref TType obj, [NotNull] IWireStreamReaderStrategy source);

		//TODO: Fix doc
		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		Task ObjectIntoWriterAsync([NotNull] TType obj, [NotNull] IWireStreamWriterStrategy dest);
	}
}
