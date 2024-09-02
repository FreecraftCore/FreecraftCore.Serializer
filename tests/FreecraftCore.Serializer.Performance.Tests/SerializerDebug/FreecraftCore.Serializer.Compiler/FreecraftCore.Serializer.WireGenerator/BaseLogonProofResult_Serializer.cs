﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.Perf;

namespace FreecraftCore.Serializer.Perf
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial class BaseLogonProofResult : IWireMessage<BaseLogonProofResult>
	{
		public virtual Type SerializableType => typeof(BaseLogonProofResult);
		public virtual BaseLogonProofResult Read(Span<byte> buffer, ref int offset)
		{
			BaseLogonProofResult_Serializer.Instance.InternalRead(this, buffer, ref offset);
			return this;
		}

		public virtual void Write(BaseLogonProofResult value, Span<byte> buffer, ref int offset)
		{
			BaseLogonProofResult_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serializer.Perf
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "BaseLogonProofResult"/>
	/// </summary>
	public sealed partial class BaseLogonProofResult_Serializer : BasePolymorphicAutoGeneratedSerializerStrategy<BaseLogonProofResult_Serializer, BaseLogonProofResult, Byte>
	{
		protected override BaseLogonProofResult CreateType(int key)
		{
			switch (key)
			{
				case 4:
					return new LogonProofFailure();
				case 0:
					return new LogonProofSuccess();
				default:
					throw new NotImplementedException($"Encountered unimplemented sub-type for Type: {nameof(BaseLogonProofResult)} with Key: {key}");
			}
		}
	}
}