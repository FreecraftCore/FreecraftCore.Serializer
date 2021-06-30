using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	[WireSaneDefaults]
	[DefaultChild(typeof(UnknownRecordTestType))]
	[WireDataContractRecordSemanticLink]
	[WireDataContract(PrimitiveSizeType.Int16)]
	public abstract partial record BaseRecordTestType(int BaseValue);

	[WireDataContract]
	public partial record RecordTestType(int TestField, string TestField2) : BaseRecordTestType(1)
	{
		[IgnoreDataMember]
		public bool Result => true;
	}

	[WireDataContract]
	public partial record RecordTestType2(string Derp) : BaseRecordTestType(2);

	[WireDataContract]
	public partial record UnknownRecordTestType(string Derp) : BaseRecordTestType(0);
}
