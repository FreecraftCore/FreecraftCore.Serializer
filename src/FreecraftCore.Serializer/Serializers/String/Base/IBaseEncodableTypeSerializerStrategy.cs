using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public sealed class EncodingCharacterSizeData
	{
		/// <summary>
		/// Minimum character size/width in bytes.
		/// </summary>
		public int MinimumCharacterSize { get; }

		/// <summary>
		/// Maximum character size/width in bytes.
		/// </summary>
		public int MaximumCharacterSize { get; }

		/// <summary>
		/// Terminator character size/width in bytes.
		/// </summary>
		public int TerminatorSize { get; }

		public EncodingCharacterSizeData(int minimumCharacterSize, int maximumCharacterSize, int terminatorSize)
		{
			MinimumCharacterSize = minimumCharacterSize;
			MaximumCharacterSize = maximumCharacterSize;
			TerminatorSize = terminatorSize;
		}
	}

	public interface IBaseEncodableTypeSerializerStrategy : ITypeSerializerStrategy<string>
	{
		/// <summary>
		/// The encoding strategy to use for the serialization.
		/// </summary>
		Encoding EncodingStrategy { get; }

		/// <summary>
		/// The character size information for the encoding/serializer.
		/// </summary>
		EncodingCharacterSizeData SizeInfo { get; }
	}
}
