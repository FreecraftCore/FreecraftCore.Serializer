﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore;

namespace FreecraftCore
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial record UnknownRecordTestType
	{
		public override Type SerializableType => typeof(UnknownRecordTestType);
		public override BaseRecordTestType Read(Span<byte> buffer, ref int offset)
		{
			throw new NotSupportedException("Record types do not support WireMessage Read.");
		}

		public override void Write(BaseRecordTestType value, Span<byte> buffer, ref int offset)
		{
			UnknownRecordTestType_Serializer.Instance.Write(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "UnknownRecordTestType"/>
	/// </summary>
	public sealed partial class UnknownRecordTestType_Serializer : BaseAutoGeneratedRecordSerializerStrategy<UnknownRecordTestType_Serializer, UnknownRecordTestType>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override UnknownRecordTestType Read(Span<byte> buffer, ref int offset)
		{
			var local_BaseValue = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
			FreecraftCore.UnknownRecordTestType value = new FreecraftCore.UnknownRecordTestType(DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Read(buffer, ref offset))
			{BaseValue = local_BaseValue};
			return value;
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void Write(UnknownRecordTestType value, Span<byte> buffer, ref int offset)
		{
			//Type: BaseRecordTestType Field: 1 Name: BaseValue Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.BaseValue, buffer, ref offset);
			//Type: UnknownRecordTestType Field: 1 Name: Derp Type: String
			;
			DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Write(value.Derp, buffer, ref offset);
		}
	}
}