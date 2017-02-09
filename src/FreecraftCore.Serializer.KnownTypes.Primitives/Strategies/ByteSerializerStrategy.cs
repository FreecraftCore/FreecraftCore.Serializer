using System;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="byte"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class ByteSerializerStrategy : SimpleTypeSerializerStrategy<byte>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public override void Write(byte value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Similar to read, this is simple; just write a byte
			dest.Write(value);
		}

		/// <inheritdoc />
		public override byte Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is a pretty simple request; just read a byte
			return source.ReadByte();
		}
	}
}
