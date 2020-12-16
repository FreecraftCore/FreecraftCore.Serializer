using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable members to indicate they should be compressed.
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] //members can be fields or props
	public class CompressAttribute : Attribute
	{
		/// <summary>
		/// Indicates the compression algorithm to use.
		/// </summary>
		public CompressionType Type { get; }

		/// <summary>
		/// Marks a property or field for compression.
		/// Defaults to WoW's ZLib length-prefixed compression.
		/// </summary>
		/// <param name="type"></param>
		public CompressAttribute(CompressionType type = CompressionType.WoWZLib)
		{
			if (!Enum.IsDefined(typeof(CompressionType), type)) throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(CompressionType));
			Type = type;
		}
	}
}
