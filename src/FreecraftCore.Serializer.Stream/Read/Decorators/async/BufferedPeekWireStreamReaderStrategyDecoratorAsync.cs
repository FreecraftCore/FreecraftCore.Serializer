using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class BufferedPeekWireStreamReaderStrategyDecoratorAsync<TReaderType> : BufferedPeekWireStreamReaderStrategyDecorator<TReaderType>, IWireStreamReaderStrategyAsync
		where TReaderType : IWireStreamReaderStrategy, IWireStreamReaderStrategyAsync
	{
		/// <inheritdoc />
		public BufferedPeekWireStreamReaderStrategyDecoratorAsync([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public Task<byte> ReadByteAsync()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadByteAsync();
			else
			{
				return new Task<byte>(() =>
				{
					byte b = BufferedBytes.First();

					BufferedCount--;

					//Reset the LINQ filled enumerable if it's empty
					//If it's not then we use a skip
					if (BufferedCount == 0)
					{
						ReaderState = State.Default;
						BufferedBytes = Enumerable.Empty<byte>();
					}
					else
						BufferedBytes = BufferedBytes.Skip(1);

					return b;
				});
			}
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			//More efficient to get the byte array
			//We need to buffer it anyway
			Task<byte[]> bytes = ReadBytesAsync(1);

			return new Task<byte>(() =>
			{
				//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
				//We avoid messing with the state until after the read.
				ReaderState = State.Peeked;

				//When we peek we must buffer the results.
				BufferedBytes = BufferedBytes.Concat(bytes.Result);

				BufferedCount++;

				return bytes.Result.First();
			});
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadAllBytesAsync();
			else
			{
				//Make sure to read outside the return task
				Task<byte[]> bytes = DecoratedReader.ReadAllBytesAsync();

				return new Task<byte[]>(() =>
				{
					ReaderState = State.Default;
					BufferedCount = 0;

					byte[] array = BufferedBytes.Concat(new EnumerableAsyncBytes(bytes)).ToArray();
					BufferedBytes = Enumerable.Empty<byte>();
					return array;
				});
			}
		}

		/// <inheritdoc />
		public Task<byte[]> ReadBytesAsync(int count)
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadBytesAsync(count);
			else
			{
				Task<byte[]> bytes = null;

				if (count == BufferedCount)
					bytes = new Task<byte[]>(() => BufferedBytes.ToArray());
				else if (count < BufferedCount)
				{
					bytes = new Task<byte[]>(() =>
					{
						byte[] lambdaBytes = BufferedBytes.Take(count).ToArray();

						//Skip the amount pulled out
						BufferedBytes = BufferedBytes.Skip(count);

						return lambdaBytes;
					});
				}
				else
				{
					Task<byte[]> asyncBytes = DecoratedReader.ReadBytesAsync(count - BufferedCount);

					bytes = new Task<byte[]>(() => BufferedBytes.Concat(new EnumerableAsyncBytes(asyncBytes)).ToArray());
				}

				//Setup a continuation to handle the buffered stuff
				return new Task<byte[]>(() =>
				{
					//Set the new size of the buffered byte chunk
					BufferedCount = Math.Max(0, BufferedCount - count);

					if (BufferedCount == 0)
					{
						ReaderState = State.Default;
						BufferedBytes = Enumerable.Empty<byte>();
					}
					return bytes.Result;
				});
			}
		}

		/// <inheritdoc />
		public Task<byte[]> PeakBytesAsync(int count)
		{
			//When we peek we must buffer the results.
			Task<byte[]> bytes = ReadBytesAsync(count);

			return new Task<byte[]>(() =>
			{
				//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
				//We avoid messing with the state until after the read.
				ReaderState = State.Peeked;

				BufferedBytes = BufferedBytes.Concat(new EnumerableAsyncBytes(bytes));
				BufferedCount += bytes.Result.Length;

				return bytes.Result;
			});
		}
	}
}
