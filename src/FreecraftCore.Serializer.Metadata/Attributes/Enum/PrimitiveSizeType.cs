using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Enumeration of supported size (primitive) types.
	/// </summary>
	public enum PrimitiveSizeType : byte
	{
		/// <summary>
		/// Special case that should only be used for polymorphic serialization.
		/// </summary>
		Bit,
		Byte,
		UInt16,
		Int16,
		UInt32,
		Int32,
		UInt64,
		Int64
	}
}
