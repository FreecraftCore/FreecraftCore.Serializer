using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;
namespace FreecraftCore.Serializer.CustomTypes
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial class ChildEnumWireContractTestType
    {
        public override Type SerializableType => typeof(ChildEnumWireContractTestType);
        public override BaseEnumWireContractTestType Read(Span<byte> buffer, ref int offset)
        {
            ChildEnumWireContractTestType_AutoGeneratedTemplateSerializerStrategy.Instance.InternalRead(this, buffer, ref offset);
            return this;
        }
        public override void Write(BaseEnumWireContractTestType value, Span<byte> buffer, ref int offset)
        {
            ChildEnumWireContractTestType_AutoGeneratedTemplateSerializerStrategy.Instance.InternalWrite(this, buffer, ref offset);
        }
    }
}

namespace FreecraftCore.Serializer
{
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    //THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
    /// <summary>
    /// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
    /// code for the Type: <see cref="ChildEnumWireContractTestType"/>
    /// </summary>
    public sealed partial class ChildEnumWireContractTestType_AutoGeneratedTemplateSerializerStrategy
            : BaseAutoGeneratedSerializerStrategy<ChildEnumWireContractTestType_AutoGeneratedTemplateSerializerStrategy, ChildEnumWireContractTestType>
    {
        /// <summary>
        /// Auto-generated deserialization/read method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalRead(ChildEnumWireContractTestType value, Span<byte> buffer, ref int offset)
        {
            //Type: ChildEnumWireContractTestType Field: 1 Name: Value Type: Int32;
            value.Value = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
        }

        /// <summary>
        /// Auto-generated serialization/write method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        public override void InternalWrite(ChildEnumWireContractTestType value, Span<byte> buffer, ref int offset)
        {
            //Type: ChildEnumWireContractTestType Field: 1 Name: Value Type: Int32;
            GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.Value, buffer, ref offset);
        }
    }
}