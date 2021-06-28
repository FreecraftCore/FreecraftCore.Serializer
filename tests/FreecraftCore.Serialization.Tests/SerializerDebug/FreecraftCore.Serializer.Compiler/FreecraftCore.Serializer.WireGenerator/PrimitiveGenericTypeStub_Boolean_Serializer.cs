﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;

namespace FreecraftCore.Serializer
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "PrimitiveGenericTypeStub<Boolean>"/>
	/// </summary>
	public sealed partial class PrimitiveGenericTypeStub_Boolean_Serializer : BaseAutoGeneratedSerializerStrategy<PrimitiveGenericTypeStub_Boolean_Serializer, PrimitiveGenericTypeStub<Boolean>>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalRead(PrimitiveGenericTypeStub<Boolean> value, Span<byte> buffer, ref int offset)
		{
			//Type: PrimitiveGenericTypeStub Field: 1 Name: Value Type: Boolean
			;
			value.Value = GenericTypePrimitiveSerializerStrategy<Boolean>.Instance.Read(buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalWrite(PrimitiveGenericTypeStub<Boolean> value, Span<byte> buffer, ref int offset)
		{
			//Type: PrimitiveGenericTypeStub Field: 1 Name: Value Type: Boolean
			;
			GenericTypePrimitiveSerializerStrategy<Boolean>.Instance.Write(value.Value, buffer, ref offset);
		}
	}
}