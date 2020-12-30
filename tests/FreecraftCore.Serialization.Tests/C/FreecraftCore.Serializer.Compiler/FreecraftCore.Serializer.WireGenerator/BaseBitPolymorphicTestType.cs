﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;
namespace FreecraftCore.Serializer.CustomTypes
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial class BaseBitPolymorphicTestType : IWireMessage<BaseBitPolymorphicTestType>
    {
        public virtual Type SerializableType => typeof(BaseBitPolymorphicTestType);
        public virtual BaseBitPolymorphicTestType Read(Span<byte> buffer, ref int offset)
        {
            BaseBitPolymorphicTestType.Instance.InternalRead(this, buffer, ref offset);
            return this;
        }
        public virtual void Write(BaseBitPolymorphicTestType value, Span<byte> buffer, ref int offset)
        {
            BaseBitPolymorphicTestType.Instance.InternalWrite(this, buffer, ref offset);
        }
    }
}

namespace FreecraftCore.Serializer
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    //THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
    /// <summary>
    /// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
    /// code for the Type: <see cref="BaseBitPolymorphicTestType"/>
    /// </summary>
    public sealed partial class BaseBitPolymorphicTestType
            : BasePolymorphicAutoGeneratedSerializerStrategy<BaseBitPolymorphicTestType, BaseBitPolymorphicTestType, Byte, BitSerializerStrategy>
    {
        protected override BaseBitPolymorphicTestType CreateType(int key)
        {
            switch (key)
            {
                default:
                    throw new NotImplementedException($"Encountered unimplemented sub-type for Type: {nameof(BaseBitPolymorphicTestType)} with Key: {key}");
            }
        }
    }
}