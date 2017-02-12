using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	public static class IWireMemberStreamReaderStrategyExtensions
	{
		/// <summary>
		/// Decorates the stream reader with single read semantics.
		/// The stream can only be read from a single time after this.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategy WithOneTimeReading(this IWireStreamReaderStrategy reader)
		{
			return new OneTimeReadReaderStrategy(reader);
		}

		/// <summary>
		/// Decorates the stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategy WithByteReversal(this IWireStreamReaderStrategy reader)
		{
			return new ReverseEndianReadReaderStrategy(reader);
		}

		/// <summary>
		/// Decorates the stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <param name="bytes">The bytes to prepend.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategy PreprendWithBytes(this IWireStreamReaderStrategy reader, byte[] bytes)
		{
			return new PrependBytesStreamReaderStrategy(reader, bytes);
		}
	}
}
