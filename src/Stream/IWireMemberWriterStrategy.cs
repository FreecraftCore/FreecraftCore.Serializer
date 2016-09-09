using System;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// WireMember writer strategy contract.
	/// </summary>
	public interface IWireMemberWriterStrategy : IDisposable
	{
		/// <summary>
		/// The <see cref="Stream"/> object the <see cref="IWireMemberWriterStrategy"/> is writing to.
		/// </summary>
		Stream WriterStream { get; }
		
		/// <summary>
		/// Writes the byte array.
		/// </summary>
		/// <param name="data"></param>
		void Write(byte[] data);
		
		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="data"></param>
		void Write(byte data);
	}
}
