using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class EnumStringSerializerDecorator<TEnumType> : SimpleTypeSerializerStrategy<TEnumType>
			where TEnumType : struct
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

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
		public override TEnumType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read the string serialized version of the enum value
			string readString = decoratedSerializer.Read(source);

			//TODO: What should we do if it's empty or null?
			return (TEnumType)Enum.Parse(typeof(TEnumType), readString);
		}

		/// <inheritdoc />
		public override void Write(TEnumType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Just write the string to the stream
			decoratedSerializer.Write(value.ToString(), dest);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TEnumType value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Just write the string to the stream
			await decoratedSerializer.WriteAsync(value.ToString(), dest);
		}

		/// <inheritdoc />
		public override async Task<TEnumType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read the string serialized version of the enum value
			string readString = await decoratedSerializer.ReadAsync(source);

			//TODO: What should we do if it's empty or null?
			return (TEnumType)Enum.Parse(typeof(TEnumType), readString);
		}
	}
}
