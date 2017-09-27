using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class DontTerminateStringSerializerDecorator : BaseStringSerializerStrategy
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		[NotNull]
		ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public DontTerminateStringSerializerDecorator([NotNull] ITypeSerializerStrategy<string> stringSerializer, [NotNull] Encoding encodingStrategy)
			: base(encodingStrategy)
		{
			if (stringSerializer == null) throw new ArgumentNullException(nameof(stringSerializer));
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public override string Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is akward. If to terminator was sent we cannot really fall back to the default string reader.
			//Someone must decorate this and override the read. Otherwise this will almost assuredly fail.
			return decoratedSerializer.Read(source);
		}

		/// <inheritdoc />
		public override void Write(string value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Pointer hack for speed
			byte[] stringBytes = EncodingStrategy.GetBytes(value);

			dest.Write(stringBytes); //Just don't write terminator. Very simple.
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Pointer hack for speed
			byte[] stringBytes = EncodingStrategy.GetBytes(value);

			await dest.WriteAsync(stringBytes); //Just don't write terminator. Very simple.
		}

		/// <inheritdoc />
		public override async Task<string> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is akward. If to terminator was sent we cannot really fall back to the default string reader.
			//Someone must decorate this and override the read. Otherwise this will almost assuredly fail.
			return await decoratedSerializer.ReadAsync(source);
		}
	}
}
