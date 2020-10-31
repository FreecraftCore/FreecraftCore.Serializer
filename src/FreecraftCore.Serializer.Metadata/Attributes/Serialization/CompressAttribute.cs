using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable members to indicate they should be compressed.
	/// (WARNING: WE ONLY SUPPORT COMPRESSION FOR THE LAST MEMBER)
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] //members can be fields or props
	public class CompressAttribute : Attribute
	{
		/// <summary>
		/// Indicates the type of size to send compression information as.
		/// </summary>
		public PrimitiveSizeType CompressionSizeType { get; }

		/// <summary>
		/// Marks a member with compression metadata.
		/// (WARNING: WE ONLY SUPPORT COMPRESSION FOR THE LAST MEMBER)
		/// </summary>
		/// <param name="sizeType"></param>
		public CompressAttribute(PrimitiveSizeType sizeType = PrimitiveSizeType.UInt32)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType))
				throw new ArgumentException($"Provided enum argument {nameof(sizeType)} of Type {typeof(PrimitiveSizeType)} with value {sizeType} was not in valid range.", nameof(sizeType));

			CompressionSizeType = sizeType;
		}
	}
}
