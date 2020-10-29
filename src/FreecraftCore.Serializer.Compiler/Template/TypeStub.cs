using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	[WireDataContract]
	internal sealed class TypeStub
	{
		[WireMember(1)]
		public int Hello { get; internal set; }

		public TypeStub()
		{
			
		}
	}
}
