using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class DefaultStringSerializerHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Read(Span<byte> source, ref int offset, EncodingType encoding, bool shouldTerminate)
		{
			string value;
			switch (encoding)
			{
				case EncodingType.ASCII:
					value = ASCIIStringTypeSerializerStrategy.Instance.Read(source, ref offset);
					if (shouldTerminate)
						ASCIIStringTerminatorTypeSerializerStrategy.Instance.Read(source, ref offset);
					return value;
				case EncodingType.UTF16:
					value = UTF16StringTypeSerializerStrategy.Instance.Read(source, ref offset);
					if(shouldTerminate)
						UTF16StringTerminatorTypeSerializerStrategy.Instance.Read(source, ref offset);
					return value;
				case EncodingType.UTF32:
					value = UTF32StringTypeSerializerStrategy.Instance.Read(source, ref offset);
					if(shouldTerminate)
						UTF32StringTerminatorTypeSerializerStrategy.Instance.Read(source, ref offset);
					return value;
				case EncodingType.UTF8:
					value = UTF8StringTypeSerializerStrategy.Instance.Read(source, ref offset);
					if(shouldTerminate)
						UTF8StringTerminatorTypeSerializerStrategy.Instance.Read(source, ref offset);
					return value;
				default:
					throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write(string value, Span<byte> destination,  ref int offset, EncodingType encoding, bool shouldTerminate)
		{
			switch(encoding)
			{
				case EncodingType.ASCII:
					ASCIIStringTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					if(shouldTerminate)
						ASCIIStringTerminatorTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					break;
				case EncodingType.UTF16:
					UTF16StringTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					if(shouldTerminate)
						UTF16StringTerminatorTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					break;
				case EncodingType.UTF32:
					UTF32StringTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					if(shouldTerminate)
						UTF32StringTerminatorTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					break;
				case EncodingType.UTF8:
					UTF8StringTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					if(shouldTerminate)
						UTF8StringTerminatorTypeSerializerStrategy.Instance.Write(value, destination, ref offset);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
			}
		}
	}
}
