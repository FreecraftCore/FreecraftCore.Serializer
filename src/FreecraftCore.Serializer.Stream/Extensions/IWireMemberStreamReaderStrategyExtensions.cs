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
		public static IWireStreamReaderStrategy WithOneTimeReading<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy
		{
			return new OneTimeReadReaderStrategy<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategy WithByteReversal<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy
		{
			return new ReverseEndianReadReaderStrategy<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <param name="bytes">The bytes to prepend.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategy PreprendWithBytes<TReaderType>(this TReaderType reader, byte[] bytes)
			where TReaderType : IWireStreamReaderStrategy
		{
			return new PrependBytesStreamReaderStrategy<TReaderType>(reader, bytes);
		}

		/// <summary>
		/// Decorates the stream with buffering functionality
		/// around readers that cannot seek or buffer themselves.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IWireStreamReaderStrategy PeekWithBuffering<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy
		{
			return new BufferedPeekWireStreamReaderStrategyDecorator<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the stream with peek only functionality.
		/// This forces the decorated reader to peek for every peek or read.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IWireStreamReaderStrategy WithOnlyPeeking<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy
		{
			return new PeekOnlyWireStreamReaderStrategy<TReaderType>(reader);
		}
	}
}
