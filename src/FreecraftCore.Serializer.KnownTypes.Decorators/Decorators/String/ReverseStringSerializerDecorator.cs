using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public override string Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			string value = decoratedSerializer.Read(source);

			return new string(value.Reverse().ToArray());
		}

		/// <inheritdoc />
		public override void Write(string value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			decoratedSerializer.Write(new string(value.Reverse().ToArray()), dest);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			await decoratedSerializer.WriteAsync(new string(value.Reverse().ToArray()), dest);
		}

		/// <inheritdoc />
		public override async Task<string> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			string value = await decoratedSerializer.ReadAsync(source);

			return new string(value.Reverse().ToArray());
		}
	}
}
