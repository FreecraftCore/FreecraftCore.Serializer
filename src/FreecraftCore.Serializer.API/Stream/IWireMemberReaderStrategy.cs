using System;
using System.IO;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Description of IWireMemberReader.
	/// </summary>
	public interface IWireMemberReaderStrategy : IDisposable
	{
		/*/// <summary>
		/// The <see cref="Stream"/> object the <see cref="IWireMemberReaderStrategy"/> is reading from.
		/// </summary>
		Stream ReaderStream { get; }*/
		
		/// <summary>
		/// Reads a byte from the stream.
		/// </summary>
		byte ReadByte();
		
		/// <summary>
		/// Reads all bytes from the stream.
		/// </summary>
		/// <returns>Returns all bytes left in the stream.</returns>
		byte[] ReadAllBytes();
		
		/// <summary>
		/// Reads <paramref name="count"/> many bytes from the stream.
		/// </summary>
		/// <param name="count">How many bytes to read.</param>
		/// <returns>A byte array of the read bytes.</returns>
		byte[] ReadBytes(int count);
	}
}
