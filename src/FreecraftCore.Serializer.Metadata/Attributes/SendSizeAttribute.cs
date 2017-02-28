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
		public enum SizeType
		{
			Byte,
			UShort,
			Int32
		}

		/// <summary>
		/// Indicates the type of size to send.
		/// </summary>
		public SizeType TypeOfSize { get; }

		public SendSizeAttribute(SizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(SizeType), sizeType))
				throw new ArgumentException($"Provided enum argument {nameof(sizeType)} of Type {typeof(SizeType)} with value {sizeType} was not in valid range.", nameof(sizeType));

			TypeOfSize = sizeType;
		}
	}
}
