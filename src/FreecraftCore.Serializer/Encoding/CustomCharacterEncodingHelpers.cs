using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	public static class CustomCharacterEncodingHelpers
	{
		/// <summary>
		/// See: https://en.wikipedia.org/wiki/Code_page_437
		/// This is pretty useless but for really old applications it may be useful.
		/// </summary>
		public static Encoding CodePage437 { get; }

		/// <summary>
		/// See: https://en.wikipedia.org/wiki/ISO/IEC_8859-1
		/// Use ISO 8859-1 for a standardized Latin-1 text encoding. Another option would be using Codepage 1252 if you're targeting Windows applications.
		/// This is essentially the extended ASCII encoding. Meaning that you can encode characters like ñ.
		/// </summary>
		public static Encoding ISO8859 { get; }

		/// <summary>
		/// UCS-2 encoding, which is a fixed-width 16-bit character encoding.
		/// This is essentially a subset of UTF-16, where every character is exactly 2 bytes,
		/// and it does not support surrogate pairs for characters beyond U+FFFF.
		/// Useful for legacy applications that assume fixed 2-byte character sizes.
		/// (Ex. PSOBB uses this for a lot of strings)
		/// </summary>
		public static Encoding UCS2 { get; }

		static CustomCharacterEncodingHelpers()
		{
			// In Unity3D WebGL it's possible some encoding may not actually exist.
			// Therefore instead of auto prop init we must manually init and try to catch exceptions
			// Reason: System.NotSupportedException: Encoding 437 data could not be found. Make sure you have correct international codeset assembly installed and enabled.
			// This type init exception will completely break applications
			try
			{
				CodePage437 = Encoding.GetEncoding(437);
			}
			catch (Exception e)
			{
				CodePage437 = null;
				Console.WriteLine($"Failed to init String Encoding {nameof(CodePage437)}.\n\n{e}");
			}

			try
			{
				ISO8859 = Encoding.GetEncoding("ISO-8859-1");
			}
			catch(Exception e)
			{
				ISO8859 = null;
				Console.WriteLine($"Failed to init String Encoding {nameof(ISO8859)}.\n\n{e}");
			}

			try
			{
				UCS2 = new UCS2Encoding();
			}
			catch(Exception e)
			{
				UCS2 = null;
				Console.WriteLine($"Failed to init String Encoding {nameof(UCS2)}.\n\n{e}");
			}
		}
	}
}
