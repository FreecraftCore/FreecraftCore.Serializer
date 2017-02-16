using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Default implementation of the <see cref="IWireStreamWriterStrategy"/> that writes bytes into
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamWriterStrategy : IWireStreamWriterStrategy
	{
		[NotNull]
		private MemoryStream WriterStream { get; } = new MemoryStream();

		/// <inheritdoc />
		public byte[] GetBytes()
		{
			return WriterStream.ToArray();
		}

		/// <inheritdoc />
		public void Write(byte data)
		{
			WriterStream.WriteByte(data);
		}

		/// <inheritdoc />
		public void Write(byte[] data)
		{
			//Stream handles throwing. Don't need to validate or check.
			WriterStream.Write(data, 0, data.Length);
		}

		/// <inheritdoc />
		public void Write(byte[] data, int offset, int count)
		{
			//Stream handles throwing. Don't need to validate or check.
			WriterStream.Write(data, offset, count);
		}

		public void Dispose()
		{
			WriterStream?.Dispose();
		}

#if !NET35
		/// <inheritdoc />
		public Task WriteAsync(byte[] data)
		{
			return WriteAsync(data, 0, data.Length);
		}

		/// <inheritdoc />
		public Task WriteAsync(byte[] data, int offset, int count)
		{
			return WriterStream.WriteAsync(data, offset, count);
		}

		/// <inheritdoc />
		public Task WriteAsync(byte data)
		{
			//TODO: Can we do this more efficiently?
			return WriteAsync(new byte[1] {data}, 0, 1);
		}

		/// <inheritdoc />
		public Task<byte[]> GetBytesAsync()
		{
			return Task.FromResult(WriterStream.ToArray());
		}
#endif
	}
}
