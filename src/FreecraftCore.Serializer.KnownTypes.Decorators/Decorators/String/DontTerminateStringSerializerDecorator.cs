using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class DontTerminateStringSerializerDecorator : ITypeSerializerStrategy<string>
	{
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(string);

		[NotNull]
		ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public DontTerminateStringSerializerDecorator([NotNull] ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null) throw new ArgumentNullException(nameof(stringSerializer));

			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public string Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is akward. If to terminator was sent we cannot really fall back to the default string reader.
			//Someone must decorate this and override the read. Otherwise this will almost assuredly fail.
			return decoratedSerializer.Read(source);
		}

		/// <inheritdoc />
		public void Write(object value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			Write((string)value, dest);
		}

		/// <inheritdoc />
		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Pointer hack for speed
			byte[] stringBytes = Encoding.ASCII.GetBytes(value);

			dest.Write(stringBytes); //Just don't write terminator. Very simple.
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(source);
		}

		public string Read(ref string obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}
	}
}
