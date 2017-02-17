using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class AsyncRandomlyBlockingStream : MemoryStream
	{
		[NotNull]
		public Stream DecoratedStream { get; }

		public AsyncRandomlyBlockingStream([NotNull] Stream decoratedStream)
		{
			if (decoratedStream == null) throw new ArgumentNullException(nameof(decoratedStream));

			DecoratedStream = decoratedStream;
		}

		public AsyncRandomlyBlockingStream()
			: this(new MemoryStream())
		{
			
		}

		public AsyncRandomlyBlockingStream(byte[] bytes)
			: this(new MemoryStream(bytes))
		{

		}

		/// <inheritdoc />
		public override void Close()
		{
			DecoratedStream.Close();
		}

		/// <inheritdoc />
		public override void Flush()
		{
			DecoratedStream.Flush();
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			await Task.Delay(50, cancellationToken);

			return await DecoratedStream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			await Task.Delay(30, cancellationToken);

			await DecoratedStream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			return DecoratedStream.Seek(offset, origin);
		}

		/// <inheritdoc />
		public override void SetLength(long value)
		{
			DecoratedStream.SetLength(value);
		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			return DecoratedStream.Read(buffer, offset, count);
		}

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
		{
			DecoratedStream.Write(buffer, offset, count);
		}

		/// <inheritdoc />
		public override bool CanRead
		{
			get { return DecoratedStream.CanRead; }
		}

		/// <inheritdoc />
		public override bool CanSeek
		{
			get { return DecoratedStream.CanSeek; }
		}

		/// <inheritdoc />
		public override bool CanWrite
		{
			get { return DecoratedStream.CanWrite; }
		}

		/// <inheritdoc />
		public override long Length
		{
			get { return DecoratedStream.Length; }
		}

		/// <inheritdoc />
		public override long Position
		{
			get { return DecoratedStream.Position; }
			set { DecoratedStream.Position = value; }
		}
	}
}
