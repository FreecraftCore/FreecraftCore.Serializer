using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using JetBrains.Annotations;

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
			//The header is not need to deflate
			//but we still need to read it
			uint sizeValue = SizeSerializer.Read(source);
			//byte[] compressionQualityHeaderBytes = source.ReadBytes(2);

			//We do not know how many bytes were compressed
			//So right now we can only support compression on the final field
			using (MemoryStream compressedStream = new MemoryStream(source.ReadAllBytes()))
			{
				using (MemoryStream decompressedStream = new MemoryStream())
				{
					using (ZlibStream decompressionStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Decompress, CompressionLevel.BestCompression))
					{
						decompressionStream.CopyTo(decompressedStream);
					}

					//Return the interpted bytes from the decompressed buffer
					return DecoratedStrategy.Read(new DefaultStreamReaderStrategy(decompressedStream.ToArray()));
				}
			}
		}

		/// <inheritdoc />
		public override void Write(TType value, IWireStreamWriterStrategy dest)
		{
			
			//Expected format header
			//[ssss]: Size (assuming the default size is used)
			//[cc]: Compression quality 78 DA for max

			byte[] decoratedSerializerBytes = null;

			//Create a new writer so we can read the bytes from the decorated serializer
			using (DefaultStreamWriterStrategy defaultWriter = new DefaultStreamWriterStrategy())
			{
				DecoratedStrategy.Write(value, defaultWriter);

				decoratedSerializerBytes = defaultWriter.GetBytes();
			}

			if(decoratedSerializerBytes == null)
				throw new InvalidOperationException($"{nameof(DecoratedStrategy)} produced null bytes in {GetType().FullName}.");

			//Write the uncompressed length
			//The ZLib library on the other side will expect 78 DA as the first type bytes
			//this denotes compression type which is max compression
			SizeSerializer.Write((uint)decoratedSerializerBytes.Length, dest);
			//dest.Write(CompressionTypeSerializerStrategyDecorator.MaxCompressionByteHeader);

			using (MemoryStream contentStream = new MemoryStream(decoratedSerializerBytes))
			{
				//Now we can write the actual content
				using (MemoryStream compressedStream = new MemoryStream())
				{
					using (ZlibStream compressionStream = new ZlibStream(compressedStream, Ionic.Zlib.CompressionMode.Compress, CompressionLevel.BestCompression))
					//using (DeflateStream compressionStream = new DeflateStream(compressedStream, CompressionMode.Compress))
					{
						contentStream.CopyTo(compressionStream);
					}

					//Writes the compressed buffer
					dest.Write(compressedStream.ToArray());
				}
			}
		}
	}
}
