using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data marker that indicates that the size of a member should be sent.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SendSizeAttribute : Attribute
	{
		public enum SizeType : byte //make byte because we need to shift and pack into int key
		{
			Byte,
			UInt16,
			Int16,
			UInt32,
			Int32
		}

		/// <summary>
		/// Indicates the type of size to send.
		/// </summary>
		public SizeType TypeOfSize { get; }

		/// <summary>
		/// Indicates the size to be added when read/written.
		/// For example, if protocols send N but only say N - 1 you can
		/// initialize this to 1 to support that.
		/// </summary>
		public sbyte AddedSize { get; }

		public SendSizeAttribute(SizeType sizeType)
			: this(sizeType, 0)
		{
			
		}

		/// <summary>
		/// Initializes the metadata with the optional added size offset.
		/// This sill increase or decrease the sent size by the provided value <see cref="AddedSize"/>.
		/// </summary>
		/// <param name="sizeType"></param>
		/// <param name="addedSize"></param>
		public SendSizeAttribute(SizeType sizeType, sbyte addedSize)
		{
			if(!Enum.IsDefined(typeof(SizeType), sizeType))
				throw new ArgumentException($"Provided enum argument {nameof(sizeType)} of Type {typeof(SizeType)} with value {sizeType} was not in valid range.", nameof(sizeType));

			TypeOfSize = sizeType;
			AddedSize = addedSize;
		}
	}
}
