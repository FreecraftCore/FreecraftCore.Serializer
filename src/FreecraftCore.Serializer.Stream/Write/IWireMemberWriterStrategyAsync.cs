using System;
using System.IO;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for objects that provide wire stream writing.
	/// </summary>
	public interface IWireStreamWriterStrategyAsync : IWireStreamWriterStrategy
	{
		/// <summary>
		/// Writes the byte array.
		/// </summary>
		/// <param name="data"></param>
		/// <exception cref="ArgumentNullException">Throws if the provided <see cref="data"/> is null.</exception>
		Task WriteAsync([NotNull] byte[] data);

		/// <summary>
		/// Overload for writes that require subarray writing.
		/// </summary>
		/// <param name="data">The array to subsect.</param>
		/// <param name="offset">Offset from the begining.</param>
		/// <param name="count">Bytes to be writtern starting from the offset.</param>
		/// <exception cref="ArgumentNullException">Throws if the provided <see cref="data"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Throws if (offset + count - 1) exceed <see cref="data"/>'s length.</exception>
		Task WriteAsync([NotNull] byte[] data, int offset, int count);

		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="data"></param>
		Task WriteAsync(byte data);

		/// <summary>
		/// Returns all bytes written.
		/// </summary>
		/// <returns>An array of bytes representing all written bytes at the time of calling or an empty array of bytes if the writer hasn't written anything.</returns>
		[Pure] //do not modify the internal representation
		[NotNull]
		Task<byte[]> GetBytesAsync();
	}
}
