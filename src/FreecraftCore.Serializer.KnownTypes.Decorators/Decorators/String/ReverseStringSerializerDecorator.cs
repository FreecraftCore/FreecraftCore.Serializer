using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	//TODO: Doc
	public class ReverseStringSerializerDecorator : SimpleTypeSerializerStrategy<string>
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		[NotNull]
		public ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public ReverseStringSerializerDecorator([NotNull] ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null)
				throw new ArgumentNullException(nameof(stringSerializer), $"Provided argument {nameof(stringSerializer)} was null.");

			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public override string Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			string value = decoratedSerializer.Read(source);

			return new string(value.Reverse().ToArray());
		}

		/// <inheritdoc />
		public override void Write(string value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			decoratedSerializer.Write(new string(value.Reverse().ToArray()), dest);
		}
	}
}
