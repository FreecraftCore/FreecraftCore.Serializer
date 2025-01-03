using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore
{
	public static class CustomCharacterEncodingHelpers
	{
		/// <summary>
		/// See: https://en.wikipedia.org/wiki/Code_page_437
		/// This is essentially the extended ASCII encoding. Meaning that you can encode characters like ñ.
		/// </summary>
		public static Encoding CodePage437 { get; } = Encoding.GetEncoding(437);
	}
}
