using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for determining the size of a string.
	/// </summary>
	public interface IStringSizeStrategy
	{
		/// <summary>
		/// Determines the size of the string
		/// </summary>
		int Size(string stringValue, IWireMemberWriterStrategy writer);

		/// <summary>
		/// Determines the size of the string fromt he stream.
		/// </summary>
		int Size(IWireMemberReaderStrategy reader);
	}
}
