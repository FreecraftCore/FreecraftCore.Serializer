using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Default implementation of the <see cref="IWireMemberWriterStrategy"/> that writes bytes into
	/// an internally managed stream.
	/// </summary>
	public class DefaultWireMemberWriterStrategy : IWireMemberWriterStrategy
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
	}
}
