using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zlib;
using JetBrains.Annotations;
using CompressionLevel = Ionic.Zlib.CompressionLevel;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Compression decorator for members.
	/// (WARNING: WE ONLY SUPPORT COMPRESSION FOR THE LAST MEMBER)
	/// </summary>
	public class CompressionTypeSerializerStrategyDecorator
	{
		public static readonly byte[] MaxCompressionByteHeader = new byte[2] { 0x78, 0xDA };
	}

	/// <summary>
	/// Compression serializer decorator. Applies compression to the decorated serializer.
	/// </summary>
	/// <typeparam name="TType">The type being serialized.</typeparam>
	public class CompressionTypeSerializerStrategyDecorator<TType> : SimpleTypeSerializerStrategy<TType>
	{ 
		/// <summary>
		/// The decorated serializer strategy.
		/// </summary>
		[NotNull]
		public ITypeSerializerStrategy<TType> DecoratedStrategy { get; }

		/// <summary>
		/// The size serializer strategy.
		/// </summary>
		public ITypeSerializerStrategy<uint> SizeSerializer { get; }

		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		public CompressionTypeSerializerStrategyDecorator([NotNull] ITypeSerializerStrategy<TType> decoratedStrategy, [NotNull] ITypeSerializerStrategy<uint> sizeSerializer)
		{
			if (decoratedStrategy == null) throw new ArgumentNullException(nameof(decoratedStrategy));
			if (sizeSerializer == null) throw new ArgumentNullException(nameof(sizeSerializer));

			DecoratedStrategy = decoratedStrategy;
			SizeSerializer = sizeSerializer;
		}

		/// <inheritdoc />
		public override TType Read(IWireStreamReaderStrategy source)
		{
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			uint sizeValue = SizeSerializer.Read(source);

			return DecoratedStrategy.Read(new DefaultStreamReaderStrategy(ZlibStream.UncompressBuffer(source.ReadAllBytes())));
		}

		/// <inheritdoc />
		public override void Write(TType value, IWireStreamWriterStrategy dest)
		{
			byte[] decoratedSerializerBytes = GetUncompressedRepresentation(value);

			if (decoratedSerializerBytes == null)
				throw new InvalidOperationException($"{nameof(DecoratedStrategy)} produced null bytes in {GetType().FullName}.");

			//Write the uncompressed length
			//WoW expects to know the uncomrpessed length
			SizeSerializer.Write((uint)decoratedSerializerBytes.Length, dest);

			dest.Write(ZlibStream.CompressBuffer(decoratedSerializerBytes));
		}

		private byte[] GetUncompressedRepresentation(TType value)
		{
			//Create a new writer so we can read the bytes from the decorated serializer
			using (DefaultStreamWriterStrategy defaultWriter = new DefaultStreamWriterStrategy())
			{
				DecoratedStrategy.Write(value, defaultWriter);

				return defaultWriter.GetBytes();
			}
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TType value, IWireStreamWriterStrategyAsync dest)
		{
			byte[] decoratedSerializerBytes = GetUncompressedRepresentation(value);

			//Write the uncompressed length
			//WoW expects to know the uncomrpessed length
			await SizeSerializer.WriteAsync((uint)decoratedSerializerBytes.Length, dest);

			using (MemoryStream contentStream = new MemoryStream(decoratedSerializerBytes))
			{
				//Now we can write the actual content
				using (MemoryStream compressedStream = new MemoryStream())
				{
					using (ZlibStream compressionStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression))
					{
						//Wait for compression to finish
						await contentStream.CopyToAsync(compressionStream);
					}

					//Wait until the stream is written
					await dest.WriteAsync(compressedStream.ToArray());
				}
			}
		}

		/// <inheritdoc />
		public override async Task<TType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			uint size = await SizeSerializer.ReadAsync(source);

			using (MemoryStream decompressedStream = new MemoryStream())
			{
				using (ZlibStream decompressionStream = new ZlibStream(decompressedStream, Ionic.Zlib.CompressionMode.Decompress, CompressionLevel.BestCompression))
				{
					//wait for the stream to decompress
					await decompressionStream.CopyToAsync(decompressedStream);
				}

				//wait for the stream to be interpreted
				return await DecoratedStrategy.ReadAsync(new DefaultStreamReaderStrategyAsync(decompressedStream.ToArray()));
			}
		}
	}
}
