using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class ReverseStringSerializerDecorator : ITypeSerializerStrategy<string>
	{
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		public Type SerializerType { get; } = typeof(string);

		public ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public ReverseStringSerializerDecorator(ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null)
				throw new ArgumentNullException(nameof(stringSerializer), $"Provided argument {nameof(stringSerializer)} was null.");

			decoratedSerializer = stringSerializer;
		}

		public string Read(IWireMemberReaderStrategy source)
		{
			string value = decoratedSerializer.Read(source);

			return new string(value.Reverse().ToArray());
		}

		public void Write(object value, IWireMemberWriterStrategy dest)
		{
			throw new NotImplementedException();
		}

		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			decoratedSerializer.Write(new string(value.Reverse().ToArray()), dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
