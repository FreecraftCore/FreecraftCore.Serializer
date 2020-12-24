using System;
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
    /// code for the Type: <see cref="PrimitiveGenericTypeStub<UInt32>"/>
    /// </summary>
    public sealed partial class PrimitiveGenericTypeStub_UInt32_AutoGeneratedTemplateSerializerStrategy
            : BaseAutoGeneratedSerializerStrategy<PrimitiveGenericTypeStub_UInt32_AutoGeneratedTemplateSerializerStrategy, PrimitiveGenericTypeStub<UInt32>>
    {
        /// <summary>
        /// Auto-generated deserialization/read method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalRead(PrimitiveGenericTypeStub<UInt32> value, Span<byte> buffer, ref int offset)
        {
            //Type: PrimitiveGenericTypeStub Field: 1 Name: Value Type: UInt32;
            value.Value = GenericTypePrimitiveSerializerStrategy<UInt32>.Instance.Read(buffer, ref offset);
        }

        /// <summary>
        /// Auto-generated serialization/write method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalWrite(PrimitiveGenericTypeStub<UInt32> value, Span<byte> buffer, ref int offset)
        {
            //Type: PrimitiveGenericTypeStub Field: 1 Name: Value Type: UInt32;
            GenericTypePrimitiveSerializerStrategy<UInt32>.Instance.Write(value.Value, buffer, ref offset);
        }
    }
}