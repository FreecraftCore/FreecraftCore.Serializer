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
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] //members can be fields or props
	public class CompressAttribute : Attribute
	{
		/// <summary>
		/// Indicates the type of size to send compression information as.
		/// </summary>
		public SizeType CompressionSizeType { get; }

		/// <summary>
		/// Enumeration of acceptable types of size to send with the compression result.
		/// </summary>
		public enum SizeType
		{
			Byte,
			Short,
			UShort,
			Int32,
			UInt32,
			Int64,
			UInt64
		}

		/// <summary>
		/// Marks a member with compression metadata.
		/// (WARNING: WE ONLY SUPPORT COMPRESSION FOR THE LAST MEMBER)
		/// </summary>
		/// <param name="sizeType"></param>
		public CompressAttribute(SizeType sizeType = SizeType.UInt32)
		{
			if (!Enum.IsDefined(typeof(SizeType), sizeType))
				throw new ArgumentException($"Provided enum argument {nameof(sizeType)} of Type {typeof(SizeType)} with value {sizeType} was not in valid range.", nameof(sizeType));

				CompressionSizeType = sizeType;
		}
	}
}
