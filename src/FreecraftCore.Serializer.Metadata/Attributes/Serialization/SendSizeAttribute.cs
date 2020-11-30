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
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class SendSizeAttribute : Attribute
	{
		/// <summary>
		/// Indicates the type of size to send.
		/// </summary>
		public PrimitiveSizeType TypeOfSize { get; }

		public SendSizeAttribute(PrimitiveSizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));
			TypeOfSize = sizeType;
		}

		internal static PrimitiveSizeType Parse(params string[] args)
		{
			if (args.Length != 1)
				throw new InvalidOperationException($"Must update {nameof(SendSizeAttribute)} handling.");

			return (PrimitiveSizeType)Enum.Parse(typeof(PrimitiveSizeType), args[0], true);
		}
	}
}
