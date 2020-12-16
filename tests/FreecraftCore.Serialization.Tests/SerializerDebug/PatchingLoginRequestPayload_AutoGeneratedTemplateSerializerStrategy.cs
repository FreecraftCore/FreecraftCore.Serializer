using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serialization.Tests.RealWorld;
namespace FreecraftCore.Serialization.Tests.RealWorld
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial class PatchingLoginRequestPayload
    {
        public override Type SerializableType => typeof(PatchingLoginRequestPayload);
        public override PSOBBPatchPacketPayloadClient Read(Span<byte> buffer, ref int offset)
        {
            PatchingLoginRequestPayload_AutoGeneratedTemplateSerializerStrategy.Instance.InternalRead(this, buffer, ref offset);
            return this;
        }
        public override void Write(PSOBBPatchPacketPayloadClient value, Span<byte> buffer, ref int offset)
        {
            PatchingLoginRequestPayload_AutoGeneratedTemplateSerializerStrategy.Instance.InternalWrite(this, buffer, ref offset);
        }
    }
}

namespace FreecraftCore.Serializer
{
    //THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
    /// <summary>
    /// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
    /// code for the Type: <see cref="PatchingLoginRequestPayload"/>
    /// </summary>
    public sealed partial class PatchingLoginRequestPayload_AutoGeneratedTemplateSerializerStrategy
        : BaseAutoGeneratedSerializerStrategy<PatchingLoginRequestPayload_AutoGeneratedTemplateSerializerStrategy, PatchingLoginRequestPayload>
    {
        /// <summary>
        /// Auto-generated deserialization/read method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalRead(PatchingLoginRequestPayload value, Span<byte> buffer, ref int offset)
        {
            //Type: PSOBBPatchPacketPayloadClient Field: 1 Name: OperationCode Type: Int16;
            value.OperationCode = GenericTypePrimitiveSerializerStrategy<Int16>.Instance.Read(buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 1 Name: Padding2 Type: Byte[];
            value.Padding2 = FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_64>.Instance.Read(buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 2 Name: UserName Type: String;
            value.UserName = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, StaticTypedNumeric_Int32_16, ASCIIStringTerminatorTypeSerializerStrategy>.Instance.Read(buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 3 Name: Password Type: String;
            value.Password = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, StaticTypedNumeric_Int32_16, ASCIIStringTerminatorTypeSerializerStrategy>.Instance.Read(buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 4 Name: Padding Type: Byte[];
            value.Padding = FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_12>.Instance.Read(buffer, ref offset);
        }

        /// <summary>
        /// Auto-generated serialization/write method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalWrite(PatchingLoginRequestPayload value, Span<byte> buffer, ref int offset)
        {
            //Type: PSOBBPatchPacketPayloadClient Field: 1 Name: OperationCode Type: Int16;
            GenericTypePrimitiveSerializerStrategy<Int16>.Instance.Write(value.OperationCode, buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 1 Name: Padding2 Type: Byte[];
            FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_64>.Instance.Write(value.Padding2, buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 2 Name: UserName Type: String;
            FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, StaticTypedNumeric_Int32_16, ASCIIStringTerminatorTypeSerializerStrategy>.Instance.Write(value.UserName, buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 3 Name: Password Type: String;
            FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, StaticTypedNumeric_Int32_16, ASCIIStringTerminatorTypeSerializerStrategy>.Instance.Write(value.Password, buffer, ref offset);
            //Type: PatchingLoginRequestPayload Field: 4 Name: Padding Type: Byte[];
            FixedSizePrimitiveArrayTypeSerializerStrategy<byte, StaticTypedNumeric_Int32_12>.Instance.Write(value.Padding, buffer, ref offset);
        }
        private sealed class StaticTypedNumeric_Int32_16 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 16; }
        private sealed class StaticTypedNumeric_Int32_64 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 64; }
        private sealed class StaticTypedNumeric_Int32_12 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 12; }
    }
}