namespace FreecraftCore.Serializer
{
	public enum EncodingType : byte
	{
		/// <summary>
		/// Indicates that the default ASCII serialization/encoding
		/// should be used. Meaning 1 byte per char.
		/// </summary>
		ASCII = 0,

		/// <summary>
		/// Indicates that the Unicode or UTF16 serialization/encoding
		/// should be used. Meaning 2 byte per char.
		/// </summary>
		UTF16 = 1,

		/// <summary>
		/// Indicates that the Unicode or UTF16 serialization/encoding
		/// should be used. Meaning 4 byte per char.
		/// </summary>
		UTF32 = 2,

		UTF8 = 3,

		/// <summary>
		/// See: https://en.wikipedia.org/wiki/Code_page_437
		/// Extended ASCII
		/// </summary>
		CodePage437,
	}
}