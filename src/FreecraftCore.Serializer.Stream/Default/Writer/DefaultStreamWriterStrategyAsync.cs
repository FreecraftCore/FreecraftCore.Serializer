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
		public Task WriteAsync(byte[] data)
		{
			return WriteAsync(data, 0, data.Length);
		}

		/// <inheritdoc />
		public Task WriteAsync(byte[] data, int offset, int count)
		{
			return ManagedStream.WriteAsync(data, offset, count);
		}

		/// <inheritdoc />
		public Task WriteAsync(byte data)
		{
			//TODO: Can we do this more efficiently?
			return WriteAsync(new byte[1] { data }, 0, 1);
		}

		/// <inheritdoc />
		public Task<byte[]> GetBytesAsync()
		{
			//Do this syncronously
			return Task.FromResult(ManagedStream.ToArray());
		}
	}
}
