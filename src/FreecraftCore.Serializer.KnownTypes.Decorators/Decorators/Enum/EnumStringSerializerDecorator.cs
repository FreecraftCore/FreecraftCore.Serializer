using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class EnumStringSerializerDecorator<TEnumType> : ITypeSerializerStrategy<TEnumType>
			where TEnumType : struct
	{
		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(TEnumType);

		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <inheritdoc />
		[NotNull]
		public ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public EnumStringSerializerDecorator([NotNull] ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null)
				throw new ArgumentNullException(nameof(stringSerializer), $"Provided argument {stringSerializer} is null.");

			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public TEnumType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read the string serialized version of the enum value
			string readString = decoratedSerializer.Read(source);

			//TODO: What should we do if it's empty or null?
			return (TEnumType)Enum.Parse(typeof(TEnumType), readString);
		}

		/// <inheritdoc />
		public void Write(TEnumType value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Just write the string to the stream
			decoratedSerializer.Write(value.ToString(), dest);
		}

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			Write((TEnumType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(source);
		}
	}
}
