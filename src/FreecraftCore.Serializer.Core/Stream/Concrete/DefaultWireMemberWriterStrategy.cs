using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public class DefaultWireMemberWriterStrategy : IWireMemberWriterStrategy
	{
		public bool isDisposed { get; private set; } = false;

		private MemoryStream WriterStream { get; } = new MemoryStream();

		public void Dispose()
		{
			WriterStream.Dispose();
			isDisposed = true;
		}

		public byte[] GetBytes()
		{
			return WriterStream.ToArray();
		}

		public void Write(byte data)
		{
			WriterStream.WriteByte(data);
		}

		public void Write(byte[] data)
		{
			WriterStream.Write(data, 0, data.Length);
		}
	}
}
