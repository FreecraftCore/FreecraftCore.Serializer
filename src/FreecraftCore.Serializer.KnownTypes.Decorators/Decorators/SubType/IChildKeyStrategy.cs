using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for producing the proper key for the subtype child lookup
	/// </summary>
	public interface IChildKeyStrategy
	{
		/// <summary>
		/// Attempts to the read the child key from the source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns>Integer casted child key.</returns>
		int Read(IWireMemberReaderStrategy source);

		/// <summary>
		/// Writes the key to the stream using the size implemented by the strategy.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="dest"></param>
		void Write(int value, IWireMemberWriterStrategy dest);
	}
}
