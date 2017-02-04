using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	//TODO: Doc
	public class ReverseStringSerializerDecorator : ITypeSerializerStrategy<string>
	{
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(string);

		[NotNull]
		public ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public ReverseStringSerializerDecorator([NotNull] ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null)
				throw new ArgumentNullException(nameof(stringSerializer), $"Provided argument {nameof(stringSerializer)} was null.");

			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public string Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			string value = decoratedSerializer.Read(source);

			return new string(value.Reverse().ToArray());
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

			decoratedSerializer.Write(new string(value.Reverse().ToArray()), dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public string Read(ref string obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}
	}
}
