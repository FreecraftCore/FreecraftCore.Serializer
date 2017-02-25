using System;
using JetBrains.Annotations;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{

	/// <summary>
	/// Contract for objects that provide wire stream async reading.
	/// </summary>
	public interface IWireStreamReaderStrategyAsync : IWireStreamReaderStrategy
	{
		/// <summary>
		/// Reads a byte from the stream.
		/// </summary>
		Task<byte> ReadByteAsync();

		/// <summary>
		/// Reads a byte from the stream.
		/// Doesn't remove it from the stream or move it forward.
		/// </summary>
		/// <returns>The byte peeked.</returns>
		Task<byte> PeekByteAsync();

		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <returns>Returns all bytes left in the stream. If there are no bytes left it returns an empty non-null array.</returns>
		[NotNull]
		Task<byte[]> ReadAllBytesAsync();

		/// <summary>
		/// Reads <paramref name="count"/> many bytes from the stream.
		/// </summary>
		/// <param name="count">How many bytes to read.</param>
		/// <exception cref="ArgumentOutOfRangeException">If the provided <see cref="count"/> is negative or exceeds the length of the underlying data.</exception>
		/// <returns>A byte array of the read bytes.</returns>
		[NotNull]
		Task<byte[]> ReadBytesAsync(int count);

		/// <summary>
		/// Peeks <paramref name="count"/> many bytes from the stream.
		/// </summary>
		/// <param name="count">How many bytes to peek.</param>
		/// <exception cref="ArgumentOutOfRangeException">If the provided <see cref="count"/> is negative or exceeds the length of the underlying data.</exception>
		/// <returns>A byte array of the peeked bytes.</returns>
		[NotNull]
		Task<byte[]> PeekBytesAsync(int count);
	}
}

