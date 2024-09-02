﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serialization.Tests.RealWorld;

namespace FreecraftCore.Serialization.Tests.RealWorld
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial class UnknownPatchPacket
	{
		public override Type SerializableType => typeof(UnknownPatchPacket);
		public override PSOBBPatchPacketPayloadServer Read(Span<byte> buffer, ref int offset)
		{
			UnknownPatchPacket_Serializer.Instance.InternalRead(this, buffer, ref offset);
			return this;
		}

		public override void Write(PSOBBPatchPacketPayloadServer value, Span<byte> buffer, ref int offset)
		{
			UnknownPatchPacket_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serialization.Tests.RealWorld
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "UnknownPatchPacket"/>
	/// </summary>
	public sealed partial class UnknownPatchPacket_Serializer : BaseAutoGeneratedSerializerStrategy<UnknownPatchPacket_Serializer, UnknownPatchPacket>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalRead(UnknownPatchPacket value, Span<byte> buffer, ref int offset)
		{
			//Type: PSOBBPatchPacketPayloadServer Field: 1 Name: OperationCode Type: UInt16
			;
			value.OperationCode = GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Read(buffer, ref offset);
			//Type: UnknownPatchPacket Field: 1 Name: UnknownBytes Type: Byte[]
			;
			value.UnknownBytes = PrimitiveArrayTypeSerializerStrategy<byte>.Instance.Read(buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalWrite(UnknownPatchPacket value, Span<byte> buffer, ref int offset)
		{
			//Type: PSOBBPatchPacketPayloadServer Field: 1 Name: OperationCode Type: UInt16
			;
			GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Write(value.OperationCode, buffer, ref offset);
			//Type: UnknownPatchPacket Field: 1 Name: UnknownBytes Type: Byte[]
			;
			PrimitiveArrayTypeSerializerStrategy<byte>.Instance.Write(value.UnknownBytes, buffer, ref offset);
		}
	}
}