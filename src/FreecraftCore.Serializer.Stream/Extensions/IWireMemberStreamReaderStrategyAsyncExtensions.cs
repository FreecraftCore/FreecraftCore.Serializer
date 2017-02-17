using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	public static class IWireMemberStreamReaderStrategyAsyncExtensions
	{
		/// <summary>
		/// Decorates the async stream reader with single read semantics.
		/// The stream can only be read from a single time after this.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategyAsync WithOneTimeReadingAsync<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
		{
			return new OneTimeReadReaderStrategyAsync<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the async stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategyAsync WithByteReversalAsync<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
		{
			return new ReverseEndianReadReaderStrategyAsync<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the async stream with byte order reversal.
		/// Whenever chunks of the stream are read their byte order's will be reversed.
		/// </summary>
		/// <param name="reader">The reader to decorate.</param>
		/// <param name="bytes">The bytes to prepend.</param>
		/// <returns>Decorated stream.</returns>
		public static IWireStreamReaderStrategyAsync PreprendWithBytesAsync<TReaderType>(this TReaderType reader, byte[] bytes)
			where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync 
		{
			return new PrependBytesStreamReaderStrategyAsync<TReaderType>(reader, bytes);
		}

		/// <summary>
		/// Decorates the async stream with buffering functionality
		/// around readers that cannot seek or buffer themselves.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IWireStreamReaderStrategyAsync PeekWithBufferingAsync<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
		{
			return new BufferedPeekWireStreamReaderStrategyDecoratorAsync<TReaderType>(reader);
		}

		/// <summary>
		/// Decorates the stream with async peek only functionality.
		/// This forces the decorated reader to peek for every peek or read.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IWireStreamReaderStrategyAsync WithOnlyPeekingAsync<TReaderType>(this TReaderType reader)
			where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
		{
			return new PeekOnlyWireStreamReaderStrategyAsync<TReaderType>(reader);
		}
	}
}
