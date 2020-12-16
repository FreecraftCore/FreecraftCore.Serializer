using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Attribute/metadata that indicates an exact primitive serialization type
	/// for a targeted enum field/property.
	/// (Ex. a byte-enum serialized as a UInt32).
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class EnumSizeAttribute : Attribute
	{
		/// <summary>
		/// Indicates the desired primitive serialization type.
		/// </summary>
		public PrimitiveSizeType SizeType { get; }

		public EnumSizeAttribute(PrimitiveSizeType sizeType)
		{
			if (!Enum.IsDefined(typeof(PrimitiveSizeType), sizeType)) throw new InvalidEnumArgumentException(nameof(sizeType), (int) sizeType, typeof(PrimitiveSizeType));

			SizeType = sizeType;
		}

		internal static PrimitiveSizeType Parse(params string[] args)
		{
			if (args.Length != 1)
				throw new InvalidOperationException($"{nameof(EnumSizeAttribute)} requires updated handling.");

			//This covers case where source generator provides fully qualified version.
			if(args[0].Contains('.'))
				args[0] = args[0].Split('.').Last();

			return (PrimitiveSizeType)Enum.Parse(typeof(PrimitiveSizeType), args[0], true);
		}
	}
}
