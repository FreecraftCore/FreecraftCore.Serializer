using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Known-type serializer for the <see cref="bool"/> value-type.
	/// </summary>
	[KnownTypeSerializer]
	public class BoolSerializerStrategy : ITypeSerializerStrategy<bool>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(bool);

		//Trinitycore Bytebuffer implementation
		/*ByteBuffer &operator>>(bool &value)
		{
			value = read<char>() > 0 ? true : false;
			return *this;
		}*/

		/// <inheritdoc />
		public bool Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Trinitycore could potentially send non-one bytes for a bool
			//See above
			return source.ReadByte() > 0;
		}

		/// <inheritdoc />
		public void Write(bool value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			dest.Write((byte)(value ? 1 : 0));
		}

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((bool)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
