using System;
using System.CodeDom;
using System.IO;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for objects that provide wire stream reading.
	/// </summary>
	public interface IWireStreamReaderStrategy : IDisposable
	{	
		/// <summary>
		/// Reads a byte from the stream.
		/// </summary>
		byte ReadByte();

		/// <summary>
		/// Reads a byte from the stream.
		/// Doesn't remove it from the stream or move it forward.
		/// </summary>
		/// <returns>The byte peeked.</returns>
		byte PeekByte();
		
		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <returns>Returns all bytes left in the stream. If there are no bytes left it returns an empty non-null array.</returns>
		[NotNull]
		byte[] ReadAllBytes();

		/// <summary>
		/// Reads <paramref name="count"/> many bytes from the stream.
		/// </summary>
		/// <param name="count">How many bytes to read.</param>
		/// <exception cref="ArgumentOutOfRangeException">If the provided <see cref="count"/> is negative or exceeds the length of the underlying data.</exception>
		/// <returns>A byte array of the read bytes.</returns>
		[NotNull]
		byte[] ReadBytes(int count);

		/// <summary>
		/// Peeks <paramref name="count"/> many bytes from the stream.
		/// </summary>
		/// <param name="count">How many bytes to peek.</param>
		/// <exception cref="ArgumentOutOfRangeException">If the provided <see cref="count"/> is negative or exceeds the length of the underlying data.</exception>
		/// <returns>A byte array of the peeked bytes.</returns>
		[NotNull]
		byte[] PeekBytes(int count);
	}
}
