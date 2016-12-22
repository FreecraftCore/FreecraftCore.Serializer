using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class DontTerminateStringSerializerDecorator : ITypeSerializerStrategy<string>
	{
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		public Type SerializerType { get; } = typeof(string);

		ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public DontTerminateStringSerializerDecorator(ITypeSerializerStrategy<string> stringSerializer)
		{
			//TODO: Null check
			decoratedSerializer = stringSerializer;
		}

		public string Read(IWireMemberReaderStrategy source)
		{
			//This is akward. If to terminator was sent we cannot really fall back to the default string reader.
			//Someone must decorate this and override the read. Otherwise this will almost assuredly fail.
			return decoratedSerializer.Read(source);
		}

		public void Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((string)value, dest);
		}

		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			//TODO: Pointer hack for speed
			byte[] stringBytes = Encoding.ASCII.GetBytes(value);

			dest.Write(stringBytes);

			//Just don't write terminator. Very simple.
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
