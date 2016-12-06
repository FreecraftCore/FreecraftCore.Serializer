using System;
using System.IO;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// WireMember writer strategy contract.
	/// </summary>
	public interface IWireMemberWriterStrategy : IDisposable
	{
		/*/// <summary>
		/// The <see cref="Stream"/> object the <see cref="IWireMemberWriterStrategy"/> is writing to.
		/// </summary>
		Stream WriterStream { get; }*/
		
		/// <summary>
		/// Writes the byte array.
		/// </summary>
		/// <param name="data"></param>
		void Write(byte[] data);

		/// <summary>
		/// Overload for writes that require subarray writing.
		/// </summary>
		/// <param name="data">The array to subsect.</param>
		/// <param name="offset">Offset from the begining.</param>
		/// <param name="count">Bytes to be writtern starting from the offset.</param>
		void Write(byte[] data, int offset, int count);
		
		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="data"></param>
		void Write(byte data);

		byte[] GetBytes();
	}
}
