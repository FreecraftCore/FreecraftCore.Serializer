using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class SkipSomeBytesWireStreamWriterStrategyDecorator : IWireStreamWriterStrategy
	{
		/// <summary>
		/// The writer to be decorated.
		/// </summary>
		private IWireStreamWriterStrategy DecoratedWriter { get; }

		/// <summary>
		/// The number of bytes to skip in the stream
		/// writing.
		/// </summary>
		private int ByteNumberToSkip { get; set; }

		/// <inheritdoc />
		public SkipSomeBytesWireStreamWriterStrategyDecorator([NotNull] IWireStreamWriterStrategy decoratedWriter, int byteNumberToSkip)
		{
			if(decoratedWriter == null) throw new ArgumentNullException(nameof(decoratedWriter));
			if(byteNumberToSkip < 0) throw new ArgumentOutOfRangeException(nameof(byteNumberToSkip));

			DecoratedWriter = decoratedWriter;
			ByteNumberToSkip = byteNumberToSkip;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			DecoratedWriter.Dispose();
		}

		/// <inheritdoc />
		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}

		/// <inheritdoc />
		public void Write(byte[] data, int offset, int count)
		{
			if(ByteNumberToSkip > 0)
				if(count > ByteNumberToSkip)
				{
					DecoratedWriter.Write(data, ByteNumberToSkip, count - ByteNumberToSkip);
					ByteNumberToSkip = 0;
				}
				else
				{
					//Just decrement
					ByteNumberToSkip = ByteNumberToSkip - count;
				}
			else
				DecoratedWriter.Write(data, offset, count);
		}

		/// <inheritdoc />
		public void Write(byte data)
		{
			if(ByteNumberToSkip > 0)
			{
				ByteNumberToSkip--;
				return;
			}

			DecoratedWriter.Write(data);
		}

		/// <inheritdoc />
		public byte[] GetBytes()
		{
			return DecoratedWriter.GetBytes();
		}
	}
}
