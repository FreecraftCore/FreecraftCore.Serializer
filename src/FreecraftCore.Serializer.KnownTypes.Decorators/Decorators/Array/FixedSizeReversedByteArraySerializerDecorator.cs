using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Special serializer that supports fixed size reversed byte arrays.
	/// This isn't a feature supported for most array types, only byte array, and the purpose
	/// is to support manual custom structures with endianness difference.
	/// </summary>
	public class FixedSizeReversedByteArraySerializerDecorator : SimpleTypeSerializerStrategy<byte[]>
	{
		[NotNull]
		private ICollectionSizeStrategy SizeStrategy { get; }

		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <inheritdoc />
		public FixedSizeReversedByteArraySerializerDecorator([NotNull] ICollectionSizeStrategy sizeStrategy)
		{
			if (sizeStrategy == null) throw new ArgumentNullException(nameof(sizeStrategy));

			SizeStrategy = sizeStrategy;
		}

		/// <inheritdoc />
		public override byte[] Read([NotNull] IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Must read size first before reverseing
			int size = SizeStrategy.Size(source);

			//Then reverse the stream with 1 time reverse/read semantics
			return source.WithOneTimeReading()
				.WithByteReversal()
				.ReadBytes(size);
		}

		/// <inheritdoc />
		public override void Write(byte[] value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//MUST copy or it will modify the external objects
			byte[] bytes = value.ToArray();
			Array.Reverse(bytes);
			dest.Write(bytes);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//MUST copy or it will modify the external objects
			byte[] bytes = value.ToArray();
			Array.Reverse(bytes);
			await dest.WriteAsync(bytes);
		}

		/// <inheritdoc />
		public override async Task<byte[]> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Must read size first before reverseing
			int size = SizeStrategy.Size(source);

			return await source.WithOneTimeReadingAsync()
				.WithByteReversalAsync()
				.ReadBytesAsync(size);
		}
	}
}
