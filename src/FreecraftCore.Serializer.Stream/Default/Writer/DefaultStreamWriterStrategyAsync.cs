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
	/// Default async implementation of the <see cref="IWireStreamWriterStrategy"/> that writes bytes into
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamWriterStrategyAsync : DefaultStreamWriterStrategy, IWireStreamWriterStrategyAsync
	{
		public DefaultStreamWriterStrategyAsync([NotNull] MemoryStream stream) 
			: base(stream)
		{

		}

		public DefaultStreamWriterStrategyAsync()
			: base()
		{

		}

		/// <inheritdoc />
		public async Task WriteAsync(byte[] data)
		{
			await WriteAsync(data, 0, data.Length);
		}

		/// <inheritdoc />
		public async Task WriteAsync(byte[] data, int offset, int count)
		{
			await ManagedStream.WriteAsync(data, offset, count);
		}

		/// <inheritdoc />
		public async Task WriteAsync(byte data)
		{
			//TODO: Can we do this more efficiently?
			await WriteAsync(new byte[1] {data}, 0, 1);
		}

		/// <inheritdoc />
		public Task<byte[]> GetBytesAsync()
		{
			//Do this syncronously
			return Task.FromResult(ManagedStream.ToArray());
		}
	}
}
