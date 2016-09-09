using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="int"/>.
	/// </summary>
	public class IntSerializerStrategy : UnsafeByteConverterTypeSerializer<int> 
	{
		public IntSerializerStrategy()
		{
			//this serializer needs no subserializers or services.
		}
	}
}
