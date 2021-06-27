using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	[WireDataContractRecordSemanticLink]
	[WireDataContract(PrimitiveSizeType.Int16)]
	public abstract partial record BaseRecordTestType(int BaseValue);

	[WireDataContract]
	public partial record RecordTestType(int TestField, string TestField2) : BaseRecordTestType(1);

	[WireDataContract]
	public partial record RecordTestType2(string Derp) : BaseRecordTestType(2);
}
