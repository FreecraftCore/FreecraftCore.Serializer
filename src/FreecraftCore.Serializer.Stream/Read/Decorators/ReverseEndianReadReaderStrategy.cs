using System;
using System.Collections.Generic;
using System.Linq;
#if !NET35
using System.Threading.Tasks;
#endif
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorator that reverses byte-order of the read byte chunks.
	/// Has no affect on single bytes. Doesn't reorder stream. Only reverses read chunks.
	/// </summary>
	public class ReverseEndianReadReaderStrategy : WireMemberReaderStrategyDecorator
	{
		public ReverseEndianReadReaderStrategy([NotNull] IWireStreamReaderStrategy decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public override byte ReadByte()
		{
			return DecoratedReader.ReadByte();
		}

		/// <inheritdoc />
		public override byte PeekByte()
		{
			return DecoratedReader.PeekByte();
		}

		/// <inheritdoc />
		public override byte[] ReadAllBytes()
		{
			byte[] bytes = DecoratedReader.ReadAllBytes();
			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public override byte[] ReadBytes(int count)
		{
			byte[] bytes = DecoratedReader.ReadBytes(count);
			Array.Reverse(bytes);

			return bytes;
		}

		/// <inheritdoc />
		public override byte[] PeakBytes(int count)
		{
			byte[] bytes = DecoratedReader.PeakBytes(count);
			Array.Reverse(bytes);

			return bytes;
		}

#if !NET35

		/// <inheritdoc />
		public override Task<byte> ReadByteAsync()
		{
			return DecoratedReader.ReadByteAsync();
		}

		/// <inheritdoc />
		public override Task<byte> PeekByteAsync()
		{
			return DecoratedReader.PeekByteAsync();
		}

		/// <inheritdoc />
		public override Task<byte[]> ReadAllBytesAsync()
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.ReadAllBytesAsync().Result;

				Array.Reverse(bytes);

				return bytes;
			});	
		}

		/// <inheritdoc />
		public override Task<byte[]> ReadBytesAsync(int count)
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.ReadBytesAsync(count).Result;

				Array.Reverse(bytes);

				return bytes;
			});
		}

		/// <inheritdoc />
		public override Task<byte[]> PeakBytesAsync(int count)
		{
			return new Task<byte[]>(() =>
			{
				//Blocks until the bytes are loaded
				byte[] bytes = DecoratedReader.PeakBytesAsync(count).Result;

				Array.Reverse(bytes);

				return bytes;
			});
		}
#endif
	}
}
