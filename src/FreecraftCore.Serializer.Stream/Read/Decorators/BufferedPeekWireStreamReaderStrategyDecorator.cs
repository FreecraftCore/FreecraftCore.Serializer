using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorates a <see cref="IWireStreamReaderStrategy"/> and provides the ability to peek
	/// and seek through a reader that doesn't support peeking by default.
	/// </summary>
	public class BufferedPeekWireStreamReaderStrategyDecorator : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Enumeration of reader states.
		/// </summary>
		private enum State
		{
			/// <summary>
			/// Indicates the default state.
			/// </summary>
			Default = 0,

			/// <summary>
			/// Indicates that the state is peeked.
			/// </summary>
			Peeked = 1,
		}

		/// <summary>
		/// The decorated reading strategy.
		/// </summary>
		[NotNull]
		private IWireStreamReaderStrategy DecoratedReader { get; }

		/// <summary>
		/// Lazy collection of the buffered bytes.
		/// </summary>
		private IEnumerable<byte> BufferedBytes { get; set; }

		//This is kept seperate for O(1) counting.
		//BufferedBytes.Count() may be an expensive operation
		private int BufferedCount { get; set; } = 0;

		private State ReaderState { get; set; }

		public BufferedPeekWireStreamReaderStrategyDecorator([NotNull] IWireStreamReaderStrategy decoratedReader)
		{
			if (decoratedReader == null) throw new ArgumentNullException(nameof(decoratedReader));

			DecoratedReader = decoratedReader;

			//TODO: Examine how costly this is. We could just use null.
			BufferedBytes = Enumerable.Empty<byte>();
			ReaderState = State.Default;
		}

		public void Dispose()
		{
			//We don't own the decorated reader.
		}

		public byte ReadByte()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadByte();
			else
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
			}
		}

		public byte PeekByte()
		{
			//More efficient to get the byte array
			//We need to buffer it anyway
			byte[] bytes = ReadBytes(1);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			//When we peek we must buffer the results.
			BufferedBytes = BufferedBytes.Concat(bytes);

			BufferedCount++;

			return bytes[0];
		}

		public byte[] PeakBytes(int count)
		{
			//When we peek we must buffer the results.
			byte[] bytes = ReadBytes(count);

			//Don't set peek until AFTER read. We may need to peek into buffered but we may not to.
			//We avoid messing with the state until after the read.
			ReaderState = State.Peeked;

			BufferedBytes = BufferedBytes.Concat(bytes);
			BufferedCount += bytes.Length;


			return bytes;
		}

		public byte[] ReadAllBytes()
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadAllBytes();
			else
			{
				ReaderState = State.Default;
				BufferedCount = 0;

				byte[] bytes = BufferedBytes.Concat(DecoratedReader.ReadAllBytes()).ToArray();

				BufferedBytes = Enumerable.Empty<byte>();

				return bytes;
			}
		}

		public byte[] ReadBytes(int count)
		{
			if (ReaderState == State.Default)
				return DecoratedReader.ReadBytes(count);
			else
			{
				byte[] bytes = null;
				if (count == BufferedCount)
					bytes = BufferedBytes.ToArray();
				else if (count < BufferedCount)
				{
					bytes = BufferedBytes.Take(count).ToArray();

					//Skip the amount pulled out
					BufferedBytes = BufferedBytes.Skip(count);
				}
				else
					bytes = BufferedBytes.Concat(DecoratedReader.ReadBytes(count - BufferedCount)).ToArray();

				//Set the new size of the buffered byte chunk
				BufferedCount = Math.Max(0, BufferedCount - count);

				if (BufferedCount == 0)
				{
					ReaderState = State.Default;
					BufferedBytes = Enumerable.Empty<byte>();
				}

				return bytes;
			}
		}


#if !NET35
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
#endif
	}
}
