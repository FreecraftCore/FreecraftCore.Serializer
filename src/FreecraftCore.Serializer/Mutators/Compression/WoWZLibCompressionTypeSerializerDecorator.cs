using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Ionic.Zlib;
using Microsoft.Win32.SafeHandles;

namespace FreecraftCore.Serializer
{
	internal sealed class WoWZLibCompressionTypeSerializerDecorator
	{
		public const string TYPE_NAME = nameof(WoWZLibCompressionTypeSerializerDecorator<GenericTypePrimitiveSerializerStrategy<int>, int>);
	}

	/// <summary>
	/// Compression decorator that decorates a <see cref="ITypeSerializerStrategy{T}"/>
	/// with a binary mutation strategy.
	/// </summary>
	/// <typeparam name="TSerializerType">The decorated serializer type.</typeparam>
	/// <typeparam name="T">The serialized type.</typeparam>
	[CompressionSerializer(CompressionType.WoWZLib)]
	public sealed class WoWZLibCompressionTypeSerializerDecorator<TSerializerType, T> 
		: StatelessTypeSerializerStrategy<WoWZLibCompressionTypeSerializerDecorator<TSerializerType, T>, T>
		where TSerializerType : StatelessTypeSerializerStrategy<TSerializerType, T>, new()
	{
		private static TSerializerType DecoratedSerializer { get; } = new TSerializerType().GetInstance();

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override unsafe T Read(Span<byte> buffer, ref int offset)
		{
			//UPDATE: We can now use it for something
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			int sizeValue = (int)GenericTypePrimitiveSerializerStrategy<uint>.Instance.Read(buffer, ref offset);

			//TODO: There are cases where this can leak.
			//We must allocate a span for the destination buffer (temporarily)
			//TODO: Support WebGL Unity3D with custom pooling.
			//TODO: Find a way to use Span without allocation and copy
			//TODO: Find a way to use Span instead of byte array in ZlibCodec
			byte[] tempOutputBuffer = ArrayPool<byte>.Shared.Rent(sizeValue); //The read size is the output size!
			Span<byte> sourceAdjusted = buffer.Slice(offset);
			byte[] tempInputBuffer = ArrayPool<byte>.Shared.Rent(sourceAdjusted.Length);

			try
			{
				//So dumb, but we MUST copy bytes
				fixed(byte* inputPtr = &tempInputBuffer[0])
				fixed(byte* trueInputPtr = &sourceAdjusted.GetPinnableReference())
				{
					//DO NOT USE tempInputBuffer length!! It's not ACTUALLY the correct size always
					Unsafe.CopyBlock(inputPtr, trueInputPtr, (uint)sourceAdjusted.Length);
				}

				ZlibCodec stream = new ZlibCodec(CompressionMode.Decompress)
				{
					InputBuffer = tempInputBuffer, //TODO: Find a way to avoid this HUGE HUGE WASTE!
					NextIn = 0,
					AvailableBytesIn = sourceAdjusted.Length, //Must use TRUE input size limit
					OutputBuffer = tempOutputBuffer,
					NextOut = 0,
					AvailableBytesOut = sizeValue, //Must use TRUE output size limit
				};

				stream.Inflate(FlushType.None);
				stream.Inflate(FlushType.Finish);
				stream.EndInflate();

				//At this point all is good, we've read ZLib TotalBytesIn amount of bytes from the source buffer.
				//This means we should provide a Span of this length to the decorated serializer so it knows how much can be read.
				offset += (int)stream.TotalBytesIn;

				//The idea here is we use the new "destination buffer" at starting index 0
				//and read the uncompressed data from it.
				return DecoratedSerializer.Read(new Span<byte>(tempOutputBuffer, 0, (int) stream.TotalBytesOut), 0);
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tempOutputBuffer);
				ArrayPool<byte>.Shared.Return(tempInputBuffer);
			}
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override unsafe void Write(T value, Span<byte> buffer, ref int offset)
		{
			int originalOffset = offset;
			DecoratedSerializer.Write(value, buffer, ref offset);
			int totalUncompressedSize = offset - originalOffset;

			//Offset to the original offset and span for the total written size.
			Span<byte> writtenDataBuffer = buffer.Slice(originalOffset, totalUncompressedSize);

			//TODO: Technically potential leak here
			//TODO: Support WebGL Unity3D with custom pooling.
			//TODO: Find a way to use Span without allocation and copy
			//TODO: Find a way to use Span instead of byte array in ZlibCodec
			byte[] tempOutputBuffer = ArrayPool<byte>.Shared.Rent(totalUncompressedSize);
			byte[] tempInputBuffer = ArrayPool<byte>.Shared.Rent(totalUncompressedSize);

			try
			{
				//So dumb, but we MUST copy bytes
				fixed(byte* inputPtr = &tempInputBuffer[0])
				fixed(byte* trueInputPtr = &writtenDataBuffer.GetPinnableReference())
				{
					//DO NOT USE tempInputBuffer length!! It's not ACTUALLY the correct size always
					Unsafe.CopyBlock(inputPtr, trueInputPtr, (uint)totalUncompressedSize);
				}

				ZlibCodec stream = new ZlibCodec(CompressionMode.Compress)
				{
					InputBuffer = tempInputBuffer,
					NextIn = 0,
					AvailableBytesIn = totalUncompressedSize, //Must use TRUE input size limit
					OutputBuffer = tempOutputBuffer,
					NextOut = 0,
					AvailableBytesOut = totalUncompressedSize, //Must use TRUE output size limit
				};

				stream.InitializeDeflate(CompressionLevel.BestSpeed, true);
				stream.Deflate(FlushType.Finish);
				stream.EndDeflate();

				//Write the uncompressed length
				//WoW expects to know the uncomrpessed length
				GenericTypePrimitiveSerializerStrategy<uint>.Instance.Write((uint) totalUncompressedSize, buffer, originalOffset);

				fixed(byte* outputPtr = &tempOutputBuffer[0])
				fixed(byte* trueDestPtr = &buffer.Slice(originalOffset + sizeof(uint)).GetPinnableReference()) //4 bytes after unit size is written
					Unsafe.CopyBlock(trueDestPtr, outputPtr, (uint)stream.TotalBytesOut);

				// Initial Offset + Bytes Read (Compression + 4 Byte Size
				//At first it might seem like we should use BytesIn but actually we shouldn't
				//because we override the buffer data with the tempe output buffer meaning
				//we want the size of "output" bytes.
				offset = originalOffset + (int) stream.TotalBytesOut + sizeof(uint);
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tempOutputBuffer);
				ArrayPool<byte>.Shared.Return(tempInputBuffer);
			}
		}
	}
}
