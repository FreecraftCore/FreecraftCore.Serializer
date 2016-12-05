using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Known-type serializer for the <see cref="bool"/> value-type.
	/// </summary>
	[KnownTypeSerializer]
	public class BoolSerializerStrategy : ITypeSerializerStrategy<bool>
	{
		//All primitive serializer stragies are contextless
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public Type SerializerType { get { return typeof(bool); } }

		/*ByteBuffer &operator>>(bool &value)
		{
			value = read<char>() > 0 ? true : false;
			return *this;
		}*/
		public bool Read(IWireMemberReaderStrategy source)
		{
			//Trinitycore could potentially send non-one bytes for a bool
			//See above
			return source.ReadByte() > 0;
		}

		public void Write(bool value, IWireMemberWriterStrategy dest)
		{
			dest.Write((byte)(value ? 1 : 0));
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((bool)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
