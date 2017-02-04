using System;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="byte"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class ByteSerializerStrategy : ITypeSerializerStrategy<byte>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(byte);

		/// <inheritdoc />
		public void Write(byte value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Similar to read, this is simple; just write a byte
			dest.Write(value);
		}

		/// <inheritdoc />
		public byte Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is a pretty simple request; just read a byte
			return source.ReadByte();
		}

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((byte)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public byte Read(ref byte obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}
	}
}
