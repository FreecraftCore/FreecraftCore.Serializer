using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for reading and writing byte sized child keys from the stream.
	/// </summary>
	public class ByteChildKeyStrategy : IChildKeyStrategy
	{
		public int Read(IWireMemberReaderStrategy source)
		{
			//Read a byte from the stream; should be the byte sized child key
			return source.ReadByte();
		}

		public void Write(int value, IWireMemberWriterStrategy dest)
		{
			//Write the byte sized key to the stream.
			dest.Write((byte)value);
		}
	}
}
