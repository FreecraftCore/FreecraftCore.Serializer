using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Flags that indicate the context of a serializable member.
	/// </summary>
	[Flags]
	public enum ContextTypeFlags : int
	{
		None = 0,
		Packed = 1 << 0,
		FixedSize = 1 << 1,
		//We could add more types of context but this should be fine for now
	}
}
