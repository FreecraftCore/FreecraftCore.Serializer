﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serialization.Tests.RealWorld;

namespace FreecraftCore.Serialization.Tests.RealWorld
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial class PatchingInformationPayload
	{
		public override Type SerializableType => typeof(PatchingInformationPayload);
		public override PSOBBPatchPacketPayloadServer Read(Span<byte> buffer, ref int offset)
		{
			PatchingInformationPayload_Serializer.Instance.InternalRead(this, buffer, ref offset);
			return this;
		}

		public override void Write(PSOBBPatchPacketPayloadServer value, Span<byte> buffer, ref int offset)
		{
			PatchingInformationPayload_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serializer
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "PatchingInformationPayload"/>
	/// </summary>
	public sealed partial class PatchingInformationPayload_Serializer : BaseAutoGeneratedSerializerStrategy<PatchingInformationPayload_Serializer, PatchingInformationPayload>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalRead(PatchingInformationPayload value, Span<byte> buffer, ref int offset)
		{
			//Type: PSOBBPatchPacketPayloadServer Field: 1 Name: OperationCode Type: UInt16
			;
			value.OperationCode = GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Read(buffer, ref offset);
			//Type: PatchingInformationPayload Field: 1 Name: PatchingByteLength Type: Int32
			;
			value.PatchingByteLength = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
			//Type: PatchingInformationPayload Field: 2 Name: PatchFileCount Type: Int32
			;
			value.PatchFileCount = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalWrite(PatchingInformationPayload value, Span<byte> buffer, ref int offset)
		{
			//Type: PSOBBPatchPacketPayloadServer Field: 1 Name: OperationCode Type: UInt16
			;
			GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Write(value.OperationCode, buffer, ref offset);
			//Type: PatchingInformationPayload Field: 1 Name: PatchingByteLength Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.PatchingByteLength, buffer, ref offset);
			//Type: PatchingInformationPayload Field: 2 Name: PatchFileCount Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.PatchFileCount, buffer, ref offset);
		}
	}
}