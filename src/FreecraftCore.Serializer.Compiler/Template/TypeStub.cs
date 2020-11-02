using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public enum TestEnum : ulong
	{
		Value1 = 0,
		Value2 = 1,
		Value3 = 2
	}

	[WireDataContract]
	public class NestedTypeStub
	{
		public NestedTypeStub()
		{
			
		}
	}

	[KnownGeneric(typeof(int))]
	[WireDataContract]
	public class TypeStubGeneric<T>
	{
		[WireMember(1)]
		public T Value { get; internal set; }

		public TypeStubGeneric()
		{
			
		}
	}

	[WireDataContract]
	public sealed class TypeStub
	{
		[WireMember(1)]
		public int Hello { get; internal set; }

		[SendSize(PrimitiveSizeType.UInt16)]
		[Encoding(EncodingType.UTF32)]
		[WireMember(2)]
		public string TestString { get; internal set; }

		[WireMember(3)]
		public ushort HelloAgain { get; internal set; }

		[DontTerminate]
		[SendSize(PrimitiveSizeType.Int16)]
		[WireMember(4)]
		public string TestStringTwo { get; internal set; }

		[DontTerminate]
		[Encoding(EncodingType.UTF16)]
		[KnownSize(1337)]
		[WireMember(5)]
		public string KnownSizeStringTest { get; internal set; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(6)]
		public string DefaultStringTest { get; internal set; }

		[SendSize(PrimitiveSizeType.Int16)]
		[WireMember(6)]
		public long[] LongArrayTest { get; internal set; }

		[KnownSize(1337)]
		[WireMember(7)]
		public ushort[] KnownSizeArrayTest { get; internal set; }

		[WireMember(8)]
		public TestEnum EnumTestValue { get; internal set; }

		[SendSize(PrimitiveSizeType.Int16, 3)]
		[WireMember(9)]
		public long[] LongArrayTestAddedSize { get; internal set; }

		[EnumSize(PrimitiveSizeType.Byte)]
		[WireMember(10)]
		public TestEnum EnumTestValueSized { get; internal set; }

		[WireMember(11)]
		public NestedTypeStub ComplexTypeTest { get; internal set; }

		public TypeStub()
		{
			
		}
	}
}
