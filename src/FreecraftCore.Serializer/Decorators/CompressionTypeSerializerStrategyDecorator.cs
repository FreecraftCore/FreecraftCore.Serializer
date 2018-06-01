using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zlib;
using JetBrains.Annotations;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using CompressionMode = Ionic.Zlib.CompressionMode;

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
			//UPDATE: We can now use it for something
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			int sizeValue = (int)SizeSerializer.Read(source);

			byte[] inputBytes = source.ReadAllBytes();
			byte[] outputBytes = new byte[sizeValue];

			ZlibCodec stream = new ZlibCodec(CompressionMode.Decompress)
			{
				InputBuffer = inputBytes,
				NextIn = 0,
				AvailableBytesIn = inputBytes.Length,
				OutputBuffer = outputBytes,
				NextOut = 0,
				AvailableBytesOut = sizeValue,
			};

			stream.Inflate(FlushType.None);
			stream.Inflate(FlushType.Finish);
			stream.EndInflate();

			return DecoratedStrategy.Read(new DefaultStreamReaderStrategy(outputBytes));
		}

		/// <inheritdoc />
		public override void Write(TType value, IWireStreamWriterStrategy dest)
		{
			//TODO: Can we reuse the buffer??
			byte[] decoratedSerializerBytes = GetUncompressedRepresentation(value);
			byte[] decompressedBytes = new byte[decoratedSerializerBytes.Length + 2];

			if(decoratedSerializerBytes == null)
				throw new InvalidOperationException($"{nameof(DecoratedStrategy)} produced null bytes in {GetType().FullName}.");

			ZlibCodec stream = new ZlibCodec(CompressionMode.Compress)
			{
				InputBuffer = decoratedSerializerBytes,
				NextIn = 0,
				AvailableBytesIn = decoratedSerializerBytes.Length,
				OutputBuffer = decompressedBytes,
				NextOut = 0,
				AvailableBytesOut = decompressedBytes.Length,
			};

			stream.InitializeDeflate(CompressionLevel.BestSpeed, true);
			stream.Deflate(FlushType.Finish);
			stream.EndDeflate();

			//Write the uncompressed length
			//WoW expects to know the uncomrpessed length
			SizeSerializer.Write((uint)decoratedSerializerBytes.Length, dest);
			
			dest.Write(stream.OutputBuffer, 0, (int)stream.TotalBytesOut);
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
			//TODO: Can we reuse the buffer??
			byte[] decoratedSerializerBytes = GetUncompressedRepresentation(value);

			if(decoratedSerializerBytes == null)
				throw new InvalidOperationException($"{nameof(DecoratedStrategy)} produced null bytes in {GetType().FullName}.");

			ZlibCodec stream = new ZlibCodec(CompressionMode.Compress)
			{
				InputBuffer = decoratedSerializerBytes,
				NextIn = 0,
				AvailableBytesIn = decoratedSerializerBytes.Length,
				OutputBuffer = decoratedSerializerBytes,
				NextOut = 0,
				AvailableBytesOut = decoratedSerializerBytes.Length,
			};

			stream.InitializeDeflate(CompressionLevel.BestSpeed, true);
			stream.Deflate(FlushType.Finish);
			stream.EndDeflate();

			//Write the uncompressed length
			//WoW expects to know the uncomrpessed length
			await SizeSerializer.WriteAsync((uint)decoratedSerializerBytes.Length, dest);

			await dest.WriteAsync(stream.OutputBuffer, 0, (int)stream.TotalBytesOut);
		}

		/// <inheritdoc />
		public override async Task<TType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			//UPDATE: We can now use it for something
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			int sizeValue = (int)await SizeSerializer.ReadAsync(source);

			byte[] inputBytes = await source.ReadAllBytesAsync();
			byte[] outputBytes = new byte[sizeValue];

			ZlibCodec stream = new ZlibCodec(CompressionMode.Decompress)
			{
				InputBuffer = inputBytes,
				NextIn = 0,
				AvailableBytesIn = inputBytes.Length,
				OutputBuffer = outputBytes,
				NextOut = 0,
				AvailableBytesOut = sizeValue,
			};

			stream.InitializeInflate(true);
			stream.Inflate(FlushType.None);
			stream.Inflate(FlushType.Finish);
			stream.EndInflate();

			return await DecoratedStrategy.ReadAsync(new DefaultStreamReaderStrategyAsync(outputBytes));
		}
	}
}
