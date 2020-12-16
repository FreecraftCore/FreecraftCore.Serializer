using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Marks a serializer type as being a compression serializer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class CompressionSerializerAttribute : Attribute
	{
		/// <summary>
		/// The compression algorithm.
		/// </summary>
		public CompressionType Type { get; }

		public CompressionSerializerAttribute(CompressionType type)
		{
			if (!Enum.IsDefined(typeof(CompressionType), type)) throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(CompressionType));
			Type = type;
		}
	}
}
