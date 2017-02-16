using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public abstract class DefaultStreamManipulationStrategy<TStreamType> : IDisposable
		where TStreamType : Stream
	{
		/// <summary>
		/// The managed stream.
		/// </summary>
		[NotNull]
		protected TStreamType ManagedStream { get; }

		/// <summary>
		/// Indicates if the managed stream should be disposed.
		/// </summary>
		protected bool ShouldDisposeStream { get; }

		protected DefaultStreamManipulationStrategy([NotNull] TStreamType managedStream, bool shouldDisposeStream)
		{
			if (managedStream == null) throw new ArgumentNullException(nameof(managedStream));

			ManagedStream = managedStream;
			ShouldDisposeStream = shouldDisposeStream;
		}

		/// <inheritdoc />
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ManagedStream.Dispose();
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(ShouldDisposeStream);
		}
	}
}
