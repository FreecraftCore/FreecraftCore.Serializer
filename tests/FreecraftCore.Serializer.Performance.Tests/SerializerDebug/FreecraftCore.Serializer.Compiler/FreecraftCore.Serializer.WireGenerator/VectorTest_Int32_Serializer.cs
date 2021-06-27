﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.Perf;

namespace FreecraftCore.Serializer
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "VectorTest<Int32>"/>
	/// </summary>
	public sealed partial class VectorTest_Int32_Serializer : BaseAutoGeneratedSerializerStrategy<VectorTest_Int32_Serializer, VectorTest<Int32>>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalRead(VectorTest<Int32> value, Span<byte> buffer, ref int offset)
		{
			//Type: VectorTest Field: 1 Name: Value Type: Int32
			;
			value.Value = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalWrite(VectorTest<Int32> value, Span<byte> buffer, ref int offset)
		{
			//Type: VectorTest Field: 1 Name: Value Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.Value, buffer, ref offset);
		}
	}
}