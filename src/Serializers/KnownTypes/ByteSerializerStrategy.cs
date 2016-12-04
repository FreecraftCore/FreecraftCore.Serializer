using System;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="byte"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class ByteSerializerStrategy : ITypeSerializerStrategy<byte>
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get; } = typeof(byte);

		/// <summary>
		/// Perform the steps necessary to serialize the byte.
		/// </summary>
		/// <param name="value">The byte to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(byte value, IWireMemberWriterStrategy dest)
		{
			//Similar to read, this is simple; just write a byte
			dest.Write(value);
		}

		/// <summary>
		/// Perform the steps necessary to deserialize a byte.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A byte value from the reader.</returns>
		public byte Read(IWireMemberReaderStrategy source)
		{
			//This is a pretty simple request; just read a byte
			return source.ReadByte();
		}

		public ByteSerializerStrategy()
		{
			//this serializer needs no subserializers or services.
		}
	}
}
