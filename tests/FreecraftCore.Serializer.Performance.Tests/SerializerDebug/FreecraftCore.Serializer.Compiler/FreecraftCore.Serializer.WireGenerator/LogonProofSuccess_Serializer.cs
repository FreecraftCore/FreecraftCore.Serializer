﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.Perf;

namespace FreecraftCore.Serializer.Perf
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial class LogonProofSuccess
	{
		public override Type SerializableType => typeof(LogonProofSuccess);
		public override BaseLogonProofResult Read(Span<byte> buffer, ref int offset)
		{
			LogonProofSuccess_Serializer.Instance.InternalRead(this, buffer, ref offset);
			return this;
		}

		public override void Write(BaseLogonProofResult value, Span<byte> buffer, ref int offset)
		{
			LogonProofSuccess_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serializer.Perf
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "LogonProofSuccess"/>
	/// </summary>
	public sealed partial class LogonProofSuccess_Serializer : BaseAutoGeneratedSerializerStrategy<LogonProofSuccess_Serializer, LogonProofSuccess>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalRead(LogonProofSuccess value, Span<byte> buffer, ref int offset)
		{
			//Type: BaseLogonProofResult Field: 1 Name: Result Type: AuthenticationResult
			;
			value.Result = GenericPrimitiveEnumTypeSerializerStrategy<AuthenticationResult, Byte>.Instance.Read(buffer, ref offset);
			//Type: LogonProofSuccess Field: 1 Name: M2 Type: Byte[]
			;
			value.M2 = FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_20>.Instance.Read(buffer, ref offset);
			//Type: LogonProofSuccess Field: 2 Name: AccountAuthorization Type: AccountAuthorizationFlags
			;
			value.AccountAuthorization = GenericPrimitiveEnumTypeSerializerStrategy<AccountAuthorizationFlags, UInt32>.Instance.Read(buffer, ref offset);
			//Type: LogonProofSuccess Field: 3 Name: SurveyId Type: UInt32
			;
			value.SurveyId = GenericTypePrimitiveSerializerStrategy<UInt32>.Instance.Read(buffer, ref offset);
			//Type: LogonProofSuccess Field: 4 Name: unk3 Type: UInt16
			;
			value.unk3 = GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Read(buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void InternalWrite(LogonProofSuccess value, Span<byte> buffer, ref int offset)
		{
			//Type: BaseLogonProofResult Field: 1 Name: Result Type: AuthenticationResult
			;
			GenericPrimitiveEnumTypeSerializerStrategy<AuthenticationResult, Byte>.Instance.Write(value.Result, buffer, ref offset);
			//Type: LogonProofSuccess Field: 1 Name: M2 Type: Byte[]
			;
			FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_20>.Instance.Write(value.M2, buffer, ref offset);
			//Type: LogonProofSuccess Field: 2 Name: AccountAuthorization Type: AccountAuthorizationFlags
			;
			GenericPrimitiveEnumTypeSerializerStrategy<AccountAuthorizationFlags, UInt32>.Instance.Write(value.AccountAuthorization, buffer, ref offset);
			//Type: LogonProofSuccess Field: 3 Name: SurveyId Type: UInt32
			;
			GenericTypePrimitiveSerializerStrategy<UInt32>.Instance.Write(value.SurveyId, buffer, ref offset);
			//Type: LogonProofSuccess Field: 4 Name: unk3 Type: UInt16
			;
			GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Write(value.unk3, buffer, ref offset);
		}

		private sealed class StaticTypedNumeric_Int32_20 : StaticTypedNumeric<Int32>
		{
			public sealed override Int32 Value => 20;
		}
	}
}