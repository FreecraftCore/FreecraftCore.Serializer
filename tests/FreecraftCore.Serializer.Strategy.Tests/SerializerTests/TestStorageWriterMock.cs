using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	public class TestStorageWriterMock : IWireStreamWriterStrategy
	{
		public bool isDisposed { get; private set; } = false;

		public Stream WriterStream { get; } = new MemoryStream();

		public void Dispose()
		{
			WriterStream.Dispose();
			isDisposed = true;
		}

		public byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public void Write(byte data)
		{
			WriterStream.WriteByte(data);
		}

		public void Write(byte[] data)
		{
			WriterStream.Write(data, 0, data.Length);
		}

		public void Write(byte[] data, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
