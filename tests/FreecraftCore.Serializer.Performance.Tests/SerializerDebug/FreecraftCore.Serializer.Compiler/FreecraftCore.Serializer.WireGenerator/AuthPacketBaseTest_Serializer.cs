﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.Perf;

namespace FreecraftCore.Serializer.Perf
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial class AuthPacketBaseTest : IWireMessage<AuthPacketBaseTest>
	{
		public virtual Type SerializableType => typeof(AuthPacketBaseTest);
		public virtual AuthPacketBaseTest Read(Span<byte> buffer, ref int offset)
		{
			AuthPacketBaseTest_Serializer.Instance.InternalRead(this, buffer, ref offset);
			return this;
		}

		public virtual void Write(AuthPacketBaseTest value, Span<byte> buffer, ref int offset)
		{
			AuthPacketBaseTest_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serializer.Perf
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "AuthPacketBaseTest"/>
	/// </summary>
	public sealed partial class AuthPacketBaseTest_Serializer : BasePolymorphicAutoGeneratedSerializerStrategy<AuthPacketBaseTest_Serializer, AuthPacketBaseTest, Byte>
	{
		protected override AuthPacketBaseTest CreateType(int key)
		{
			switch (key)
			{
				case 1:
					return new AuthLogonProofResponse();
				default:
					throw new NotImplementedException($"Encountered unimplemented sub-type for Type: {nameof(AuthPacketBaseTest)} with Key: {key}");
			}
		}
	}
}