﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore;

namespace FreecraftCore
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial record RecordEnumBaseTestType2
	{
		public override Type SerializableType => typeof(RecordEnumBaseTestType2);
		public override BaseRecordEnumBaseTestType Read(Span<byte> buffer, ref int offset)
		{
			throw new NotSupportedException("Record types do not support WireMessage Read.");
		}

		public override void Write(BaseRecordEnumBaseTestType value, Span<byte> buffer, ref int offset)
		{
			RecordEnumBaseTestType2_Serializer.Instance.Write(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "RecordEnumBaseTestType2"/>
	/// </summary>
	public sealed partial class RecordEnumBaseTestType2_Serializer : BaseAutoGeneratedRecordSerializerStrategy<RecordEnumBaseTestType2_Serializer, RecordEnumBaseTestType2>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override RecordEnumBaseTestType2 Read(Span<byte> buffer, ref int offset)
		{
			var local_OpCode = GenericPrimitiveEnumTypeSerializerStrategy<TestEnumOpcode, Int32>.Instance.Read(buffer, ref offset);
			var local_BaseValue = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
			FreecraftCore.RecordEnumBaseTestType2 value = new FreecraftCore.RecordEnumBaseTestType2(DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Read(buffer, ref offset))
			{OpCode = local_OpCode, BaseValue = local_BaseValue};
			return value;
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void Write(RecordEnumBaseTestType2 value, Span<byte> buffer, ref int offset)
		{
			//Type: BaseRecordEnumBaseTestType Field: 1 Name: OpCode Type: TestEnumOpcode
			;
			GenericPrimitiveEnumTypeSerializerStrategy<TestEnumOpcode, Int32>.Instance.Write(value.OpCode, buffer, ref offset);
			//Type: BaseRecordEnumBaseTestType Field: 2 Name: BaseValue Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.BaseValue, buffer, ref offset);
			//Type: RecordEnumBaseTestType2 Field: 1 Name: Derp Type: String
			;
			DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Write(value.Derp, buffer, ref offset);
		}
	}
}