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

		[DontTerminate]
		[SendSize(SendSizeAttribute.SizeType.Int16)]
		[WireMember(4)]
		public string TestStringTwo { get; internal set; }

		[DontTerminate]
		[Encoding(EncodingType.UTF16)]
		[KnownSize(1337)]
		[WireMember(5)]
		public string KnownSizeStringTest { get; internal set; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(6)]
		public string DefaultStringTest { get; private set; }

		[SendSize(SendSizeAttribute.SizeType.Int16)]
		[WireMember(6)]
		public long[] LongArrayTest { get; private set; }

		[KnownSize(1337)]
		[WireMember(7)]
		public ushort[] KnownSizeArrayTest { get; private set; }

		public TypeStub()
		{
			
		}
	}
}
