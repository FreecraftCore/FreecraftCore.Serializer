using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	public enum TestEnumOpcode
	{
		One,
		Two
	}

	[WireSaneDefaults]
	[WireDataContractRecordSemanticLink]
	[WireDataContract(PrimitiveSizeType.Int16)]
	public abstract partial record BaseRecordEnumBaseTestType(TestEnumOpcode OpCode, int BaseValue = default);

	[WireDataContract]
	public partial record RecordEnumBaseTestType(int TestField, string TestField2, int[] TestArray) : BaseRecordEnumBaseTestType(TestEnumOpcode.One);

	[WireDataContract]
	public partial record RecordEnumBaseTestType2(string Derp) : BaseRecordEnumBaseTestType(TestEnumOpcode.Two, 2);
}
