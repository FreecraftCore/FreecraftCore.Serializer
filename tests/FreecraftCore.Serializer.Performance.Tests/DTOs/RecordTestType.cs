using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	[WireDataContract(PrimitiveSizeType.Int16)]
	public abstract partial record BaseRecordTestType(int BaseValue);

	[WireDataContract]
	public partial record RecordTestType(int TestField, string TestField2) : BaseRecordTestType(1);

	public class tttt
	{
		public void t()
		{
			RecordTestType r = new RecordTestType(TestField: 1, TestField2: "")
			{
				BaseValue = 5
			};
		}
	}
}
