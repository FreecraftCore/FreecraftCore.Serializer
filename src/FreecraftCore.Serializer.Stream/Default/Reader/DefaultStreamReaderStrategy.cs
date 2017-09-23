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
	/// Default implementation of the <see cref="IWireStreamReaderStrategy"/> that reads bytes from
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamReaderStrategy : DefaultStreamManipulationStrategy<Stream>, IWireStreamReaderStrategy
	{
		//TODO: Overloads that take the byte buffer instead
		public DefaultStreamReaderStrategy([NotNull] byte[] bytes)
			: base(new MemoryStream(bytes), true)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes), $"Provided argument {nameof(bytes)} must not be null.");
		}

		public DefaultStreamReaderStrategy([NotNull] Stream stream)
			: base(stream, false)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream), $"Provided argument {nameof(stream)} must not be null.");
		}

		public byte[] ReadAllBytes()
		{
			return ReadBytes((int)(ManagedStream.Length - ManagedStream.Position));
		}

		public byte ReadByte()
		{
			//would be -1 if it's invalid
			int b = ManagedStream.ReadByte();

			//TODO: Contract interface doesn't mention throwing in this case. Should we throw?
			if (b == -1)
				throw new InvalidOperationException("Failed to read a desired byte from the stream.");

			return (byte)b;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] bytes = new byte[count];

			ManagedStream.Read(bytes, 0, count);

			return bytes;
		}

		public byte PeekByte()
		{
			byte b = ReadByte();

			//Move it back one
			ManagedStream.Position = ManagedStream.Position - 1;

			return b;
		}

		public byte[] PeekBytes(int count)
		{
			byte[] bytes = ReadBytes(count);

			//Now move the stream back
			ManagedStream.Position = ManagedStream.Position - count;

			return bytes;
		}
	}
}
