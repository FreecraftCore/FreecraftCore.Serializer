using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class SizeStringSerializerDecorator : SimpleTypeSerializerStrategy<string>
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <summary>
		/// Provides the size of the fixed string.
		/// </summary>
		[NotNull]
		public IStringSizeStrategy sizeProvider { get; }

		/// <summary>
		/// The string serializer that is being decorated.
		/// </summary>
		[NotNull]
		private ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public SizeStringSerializerDecorator([NotNull] IStringSizeStrategy size, [NotNull] ITypeSerializerStrategy<string> stringSerializer)
		{
			if (size == null) throw new ArgumentNullException(nameof(size));
			if (stringSerializer == null) throw new ArgumentNullException(nameof(stringSerializer));

			sizeProvider = size;
			decoratedSerializer = stringSerializer;
		}

		/// <inheritdoc />
		public override string Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//The size must come from the strategy provided
			int size = sizeProvider.Size(source);

			byte[] bytes = source.ReadBytes(size);

			//TODO: Pointer hack for preformance
			//This is the only way to remove padding that I know of
			//There may be a more efficient way of removing the padding
			//There is actually an unsafe pointer hack to improve preformance here too.
			//profile and add later.
			return Encoding.ASCII.GetString(bytes).TrimEnd('\0'); 
		}

		/// <inheritdoc />
		public override void Write([NotNull] string value, IWireStreamWriterStrategy dest)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			int size = sizeProvider.Size(value, dest);

			//Now that we know the size, and the header will be written if it was needed, we can write it
			//Don't write the size. Leave it up to the strategy above
			decoratedSerializer.Write(value, dest);

			//the tricky part here is that the serializer just wrote the string plus the null terminator
			//So, if the length of the string was less than the expected size write some more 0s.
			//However, DO NOT write another null terminator either way because we already have one.
			if (value.Length < size)
				dest.Write(new byte[(size - value.Length)]);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string value, IWireStreamWriterStrategyAsync dest)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			int size = await sizeProvider.SizeAsync(value, dest);

			await decoratedSerializer.WriteAsync(value, dest);

			if (value.Length < size)
				await dest.WriteAsync(new byte[(size - value.Length)]);
		}

		/// <inheritdoc />
		public override async Task<string> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//The size must come from the strategy provided
			int size = await sizeProvider.SizeAsync(source);

			byte[] bytes = await source.ReadBytesAsync(size);

			return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
		}
	}
}
