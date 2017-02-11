using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Known-type serializer for the <see cref="bool"/> value-type.
	/// </summary>
	[KnownTypeSerializer]
	public class BoolSerializerStrategy : SimpleTypeSerializerStrategy<bool>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		//Trinitycore Bytebuffer implementation
		/*ByteBuffer &operator>>(bool &value)
		{
			value = read<char>() > 0 ? true : false;
			return *this;
		}*/

		/// <inheritdoc />
		public override bool Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Trinitycore could potentially send non-one bytes for a bool
			//See above
			return source.ReadByte() > 0;
		}

		/// <inheritdoc />
		public override void Write(bool value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			dest.Write((byte)(value ? 1 : 0));
		}

		public override byte[] GetBytes(bool obj)
		{
			return new byte[] { (byte)(obj ? 1 : 0) };
		}

		public override bool FromBytes([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			return bytes.First() > 0;
		}
	}
}
