﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore;
namespace FreecraftCore
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial record BaseRecordTestType : IWireMessage<BaseRecordTestType>
    {
        public virtual Type SerializableType => typeof(BaseRecordTestType);
        public virtual BaseRecordTestType Read(Span<byte> buffer, ref int offset)
        {
            throw new NotSupportedException("Record types do not support WireMessage Read.");
        }

        public virtual void Write(BaseRecordTestType value, Span<byte> buffer, ref int offset)
        {
            BaseRecordTestType_Serializer.Instance.Write(this, buffer, ref offset);
        }
    }
}

namespace FreecraftCore.Serializer
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    //THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
    /// <summary>
    /// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
    /// code for the Type: <see cref="BaseRecordTestType"/>
    /// </summary>
    public sealed partial class BaseRecordTestType_Serializer
            : BasePolymorphicAutoGeneratedRecordSerializerStrategy<BaseRecordTestType_Serializer, BaseRecordTestType, Int16>
    {
        protected override BaseRecordTestType CreateType(int key, Span<byte> buffer, ref int offset)
        {
            switch (key)
            {
                default:
                    throw new NotImplementedException($"Encountered unimplemented sub-type for Type: {nameof(BaseRecordTestType)} with Key: {key}");
            }
        }
    }
}