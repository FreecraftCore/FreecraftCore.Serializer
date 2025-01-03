using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore
{
	public static class CustomCharacterEncodingHelpers
	{
		/// <summary>
		/// See: https://en.wikipedia.org/wiki/Code_page_437
		/// This is pretty useless but for really old applications it may be useful.
		/// </summary>
		public static Encoding CodePage437 { get; } = Encoding.GetEncoding(437);

		/// <summary>
		/// See: https://en.wikipedia.org/wiki/ISO/IEC_8859-1
		/// Use ISO 8859-1 for a standardized Latin-1 text encoding. Another option would be using Codepage 1252 if you're targeting Windows applications.
		/// This is essentially the extended ASCII encoding. Meaning that you can encode characters like ñ.
		/// </summary>
		public static Encoding ISO8859 { get; } = Encoding.GetEncoding(28591);
	}
}
