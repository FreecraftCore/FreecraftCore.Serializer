using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Ionic.Zlib;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Binary mutator for ZLib compression of binary data.
	/// </summary>
	public sealed class ZLibCompressionBinaryMutatorStrategy : StatelessBinaryMutatorStrategy<ZLibCompressionBinaryMutatorStrategy>
	{
		public static readonly byte[] MaxCompressionByteHeader = new byte[2] { 0x78, 0xDA };

		//WRITE
		/// <inheritdoc />
		public sealed override unsafe void Mutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset)
		{
			//Offset
			var sourceAdjusted = source.Slice(sourceOffset);
			var destinationAdjusted = destination.Slice(destinationOffset);

			if(sourceAdjusted.Length > destination.Length)
				ThrowDestinationBufferTooSmall(sourceAdjusted.Length, destinationAdjusted.Length);

			//TODO: Support WebGL Unity3D with custom pooling.
			//TODO: Find a way to use Span without allocation and copy
			//TODO: Find a way to use Span instead of byte array in ZlibCodec
			byte[] tempOutputBuffer = ArrayPool<byte>.Shared.Rent(destinationAdjusted.Length);
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

				ZlibCodec stream = new ZlibCodec(CompressionMode.Compress)
				{
					InputBuffer = tempInputBuffer,
					NextIn = 0,
					AvailableBytesIn = sourceAdjusted.Length, //Must use TRUE input size limit
					OutputBuffer = tempOutputBuffer,
					NextOut = 0,
					AvailableBytesOut = destinationAdjusted.Length, //Must use TRUE output size limit
				};

				stream.InitializeDeflate(CompressionLevel.BestSpeed, true);
				stream.Deflate(FlushType.Finish);
				stream.EndDeflate();

				//Write the uncompressed length
				//WoW expects to know the uncomrpessed length
				GenericTypePrimitiveSerializerStrategy<uint>.Instance.Write((uint)source.Length, destination, ref destinationOffset);

				fixed(byte* outputPtr = &tempOutputBuffer[0])
				fixed(byte* trueDestPtr = &destination.Slice(destinationOffset).GetPinnableReference())
				{
					Unsafe.CopyBlock(trueDestPtr, outputPtr, (uint)stream.TotalBytesOut);
				}

				//At this point all is good, the data is in the destination buffer and we should move it forward to indicate to caller.
				destinationOffset += (int)stream.TotalBytesOut;
				sourceOffset += (int)stream.TotalBytesIn;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tempOutputBuffer);
				ArrayPool<byte>.Shared.Return(tempInputBuffer);
			}
		}

		//READ
		/// <inheritdoc />
		public sealed override unsafe void UnMutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset)
		{
			//UPDATE: We can now use it for something
			//WoW sends a 4 byte uncompressed size. We can't use it for anything
			//We could assert or throw on it though.
			int sizeValue = (int)GenericTypePrimitiveSerializerStrategy<uint>.Instance.Read(source, ref sourceOffset);

			//Temp shift the dest buffer
			Span<byte> destinationAdjusted = destination.Slice(destinationOffset);
			Span<byte> sourceAdjusted = source.Slice(sourceOffset);

			if (sizeValue >= destinationAdjusted.Length)
				ThrowDestinationBufferTooSmall(sizeValue, destinationAdjusted.Length);

			//TODO: Support WebGL Unity3D with custom pooling.
			//TODO: Find a way to use Span without allocation and copy
			//TODO: Find a way to use Span instead of byte array in ZlibCodec
			byte[] tempOutputBuffer = ArrayPool<byte>.Shared.Rent(sizeValue); //The read size is the output size!
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

				fixed (byte* outputPtr = &tempOutputBuffer[0])
				fixed (byte* trueDestPtr = &destinationAdjusted.GetPinnableReference())
				{
					Unsafe.CopyBlock(outputPtr, trueDestPtr, (uint) sizeValue);
				}

				//At this point all is good, the data is in the destination buffer and we should move it forward to indicate to caller.
				destinationOffset += sizeValue;
				sourceOffset += (int)stream.TotalBytesIn;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tempOutputBuffer);
				ArrayPool<byte>.Shared.Return(tempInputBuffer);
			}
		}

		//Throw helper
		private void ThrowDestinationBufferTooSmall(int requestedSize, int availableSize)
		{
			throw new InvalidOperationException($"Tried to copy to destination buffer not large enough Size: {requestedSize} Available: {availableSize}");
		}
	}
}
