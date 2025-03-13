using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// A UCS-2 encoding class that extends UnicodeEncoding.
	/// Effectively behaves like UTF-16 but assumes all characters are exactly 2 bytes (no surrogate pairs).
	/// </summary>
	public sealed class UCS2Encoding : UnicodeEncoding
	{

	}
}
