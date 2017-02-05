using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface ITypeSerializerStrategy
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		[NotNull]
		Type SerializerType { get; }

		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serializer for multiple members)
		/// </summary>
		SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		[Pure]
		void Write(object value, [NotNull] IWireMemberWriterStrategy dest);

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		[Pure]
		[NotNull]
		object Read([NotNull] IWireMemberReaderStrategy source);

		//TODO: Fix doc
		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		object ReadIntoObject([CanBeNull] ref object obj, [NotNull] IWireMemberReaderStrategy source);

		//TODO: Fix doc
		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		void ObjectIntoWriter([CanBeNull] object obj, [NotNull] IWireMemberWriterStrategy dest);
	}

	//This concept is based on JAM (Blizzard's messaging system/protocol and Protobuf-net's serializer strategies https://github.com/mgravell/protobuf-net/tree/master/protobuf-net/Serializers
	/// <summary>
	/// Contract for type that providing serialization strategy for the provided TType.
	/// </summary>
	public interface ITypeSerializerStrategy<TType> : ITypeSerializerStrategy
	{
		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		[Pure]
		void Write(TType value, [NotNull] IWireMemberWriterStrategy dest);

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		[Pure]
		[NotNull]
		new TType Read([NotNull] IWireMemberReaderStrategy source);

		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		TType ReadIntoObject([CanBeNull] ref TType obj, [NotNull] IWireMemberReaderStrategy source);

		/// <summary>
		/// Preform the steps necessary to deserialize the data into the provided <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Possibly null ref object of type the serializer handles.</param>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A non-null instance of the <typeparamref name="TType"/> object.</returns>
		[NotNull]
		void ObjectIntoWriter([NotNull] TType obj, [NotNull] IWireMemberWriterStrategy dest);
	}
}
