using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	[WireDataContract]
	public sealed class TypeStub
	{
		[WireMember(1)]
		public int Hello { get; internal set; }

		[SendSize(SendSizeAttribute.SizeType.UInt16)]
		[Encoding(EncodingType.UTF32)]
		[WireMember(2)]
		public string TestString { get; internal set; }

		[WireMember(3)]
		public ushort HelloAgain { get; internal set; }

		public TypeStub()
		{
			
		}
	}
}
