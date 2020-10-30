using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class KnownSizeStringSerializerHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Read(Span<byte> source, ref int offset,
			int fixedSize, EncodingType encodingType, bool shouldTerminate) //optional args
		{
			if (fixedSize == 0)
				return String.Empty;

			switch (encodingType)
			{
				case EncodingType.ASCII:
					return ReadFixedLength(ASCIIStringTypeSerializerStrategy.Instance, ASCIIStringTerminatorTypeSerializerStrategy.Instance, source, ref offset, fixedSize, shouldTerminate);
				case EncodingType.UTF16:
					return ReadFixedLength(UTF16StringTypeSerializerStrategy.Instance, UTF16StringTerminatorTypeSerializerStrategy.Instance, source, ref offset, fixedSize, shouldTerminate);
				case EncodingType.UTF32:
					return ReadFixedLength(UTF32StringTypeSerializerStrategy.Instance, UTF32StringTerminatorTypeSerializerStrategy.Instance, source, ref offset, fixedSize, shouldTerminate);
				case EncodingType.UTF8:
					return ReadFixedLength(UTF8StringTypeSerializerStrategy.Instance, UTF8StringTerminatorTypeSerializerStrategy.Instance, source, ref offset, fixedSize, shouldTerminate);
				default:
					throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write(string value, Span<byte> destination, ref int offset, 
			int fixedSize, EncodingType encodingType, bool shouldTerminate) //optional args
		{
			if (fixedSize == 0)
				return;

			//Fix the destination buffer based on char size.
			switch (encodingType)
			{
				case EncodingType.ASCII:
					WriteFixedLength(ASCIIStringTypeSerializerStrategy.Instance, ASCIIStringTerminatorTypeSerializerStrategy.Instance, value, destination, ref offset, fixedSize, shouldTerminate);
					break;
				case EncodingType.UTF16:
					WriteFixedLength(UTF16StringTypeSerializerStrategy.Instance, UTF16StringTerminatorTypeSerializerStrategy.Instance, value, destination, ref offset, fixedSize, shouldTerminate);
					break;
				case EncodingType.UTF32:
					WriteFixedLength(UTF32StringTypeSerializerStrategy.Instance, UTF32StringTerminatorTypeSerializerStrategy.Instance, value, destination, ref offset, fixedSize, shouldTerminate);
					break;
				case EncodingType.UTF8:
					WriteFixedLength(UTF8StringTypeSerializerStrategy.Instance, UTF8StringTerminatorTypeSerializerStrategy.Instance, value, destination, ref offset, fixedSize, shouldTerminate);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ReadFixedLength<TStringSerializerStrategy, TTerminatorStrategy>(TStringSerializerStrategy serializer, TTerminatorStrategy terminatorSerializer,
			Span<byte> source, ref int offset, int fixedSize, bool shouldTerminate)
			where TStringSerializerStrategy : BaseStringTypeSerializerStrategy<TStringSerializerStrategy>, new()
			where TTerminatorStrategy : BaseStringTerminatorSerializerStrategy<TTerminatorStrategy>, new()
		{
			int initialOffset = offset;
			int fixedSizeLength = serializer.CharacterSize * fixedSize;
			if(shouldTerminate)
				fixedSizeLength -= serializer.CharacterSize;

			string value = serializer.Read(source.Slice(0, offset + fixedSizeLength + offset), ref offset);

			//Did we read enough?? Doesn't matter, we can direct set to new position.
			offset = initialOffset + fixedSizeLength;

			if(shouldTerminate)
				terminatorSerializer.Read(source, ref offset);

			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteFixedLength<TStringSerializerStrategy, TTerminatorStrategy>(TStringSerializerStrategy serializer, TTerminatorStrategy terminatorSerializer, 
			string value, Span<byte> destination, ref int offset, int fixedSize, bool shouldTerminate)
			where TStringSerializerStrategy : BaseStringTypeSerializerStrategy<TStringSerializerStrategy>, new()
			where TTerminatorStrategy : BaseStringTerminatorSerializerStrategy<TTerminatorStrategy>, new()
		{
			int fixedSizeLength = serializer.CharacterSize * fixedSize;
			if (shouldTerminate)
				fixedSizeLength -= serializer.CharacterSize;

			int lastOffset = offset;
			serializer.Write(value, destination.Slice(0, fixedSizeLength + offset), ref offset);

			//Force the fixed length buffer write, and null the buffer out (could be data in it otherwise)
			while(offset < lastOffset)
				GenericTypePrimitiveSerializerStrategy<byte>.Instance.Write(0, destination, ref offset);

			if (shouldTerminate)
				terminatorSerializer.Write(value, destination, ref offset);
		}
	}
}
