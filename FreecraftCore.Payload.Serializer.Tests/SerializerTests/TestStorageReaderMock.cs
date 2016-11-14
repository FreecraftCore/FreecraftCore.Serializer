﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer.Tests
{
	public class TestStorageReaderMock : IWireMemberReaderStrategy
	{
		public bool isDisposed { get; private set; } = false;

		public Stream ReaderStream { get; }

		public TestStorageReaderMock(Stream stream)
		{
			ReaderStream = stream;
		}

		public void Dispose()
		{
			ReaderStream.Dispose();
			isDisposed = true;
		}

		public byte[] ReadAllBytes()
		{
			return ReadBytes((int)(ReaderStream.Length - ReaderStream.Position));
		}

		public byte ReadByte()
		{
			//would be -1 if it's invalid
			return (byte)ReaderStream.ReadByte();
		}

		public byte[] ReadBytes(int count)
		{
			byte[] bytes = new byte[ReaderStream.Length];

			ReaderStream.Read(bytes, 0, count);

			return bytes;
		}
	}
}
