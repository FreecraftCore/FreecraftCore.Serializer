using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;

namespace FreecraftCore.Serializer.CustomTypes
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial class TypeStub : IWireMessage<TypeStub>
    {
        public Type SerializableType => typeof(TypeStub);
        public TypeStub Read(Span<byte> buffer, ref int offset)
        {
            TypeStub_AutoGeneratedTemplateSerializerStrategy.Instance.InternalRead(this, buffer, ref offset);
            return this;
        }
        public void Write(TypeStub value, Span<byte> buffer, ref int offset)
        {
            TypeStub_AutoGeneratedTemplateSerializerStrategy.Instance.InternalWrite(this, buffer, ref offset);
        }
    }
}

namespace FreecraftCore.Serializer
{
    //THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
    /// <summary>
    /// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
    /// code for the Type: <see cref="TypeStub"/>
    /// </summary>
    public sealed partial class TypeStub_AutoGeneratedTemplateSerializerStrategy
        : BaseAutoGeneratedSerializerStrategy<TypeStub_AutoGeneratedTemplateSerializerStrategy, TypeStub>
    {
        /// <summary>
        /// Auto-generated deserialization/read method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        protected internal override void InternalRead(TypeStub value, Span<byte> buffer, ref int offset)
        {
            //Type: BaseTypeStub Field: 1 Name: BaseIntField Type: Int32;
            value.BaseIntField = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 1 Name: Hello Type: Int32;
            value.Hello = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 2 Name: TestString Type: String;
            value.TestString = LengthPrefixedStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, UTF32StringTerminatorTypeSerializerStrategy, UInt16>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 3 Name: HelloAgain Type: UInt16;
            value.HelloAgain = GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 4 Name: TestStringTwo Type: String;
            value.TestStringTwo = DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int16>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 5 Name: KnownSizeStringTest Type: String;
            value.KnownSizeStringTest = KnownSizeStringSerializerHelper.Read(buffer, ref offset, 1337, EncodingType.UTF16, false);
            //Type: TypeStub Field: 6 Name: DefaultStringTest Type: String;
            value.DefaultStringTest = DefaultStringSerializerHelper.Read(buffer, ref offset, EncodingType.UTF16, true);
            //Type: TypeStub Field: 6 Name: LongArrayTest Type: Int64[];
            value.LongArrayTest = SendSizePrimitiveArrayTypeSerializerStrategy<Int64, Int16>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 7 Name: KnownSizeArrayTest Type: UInt16[];
            value.KnownSizeArrayTest = FixedSizePrimitiveArrayTypeSerializerStrategy<UInt16, StaticTypedNumeric_Int32_1337>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 8 Name: EnumTestValue Type: TestEnum;
            value.EnumTestValue = GenericPrimitiveEnumTypeSerializerStrategy<TestEnum, UInt64>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 9 Name: LongArrayTestAddedSize Type: Int64[];
            value.LongArrayTestAddedSize = SendSizePrimitiveArrayTypeSerializerStrategy<Int64, Int16>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 10 Name: EnumTestValueSized Type: TestEnum;
            value.EnumTestValueSized = GenericPrimitiveEnumTypeSerializerStrategy<TestEnum, Byte>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 11 Name: ComplexTypeTest Type: NestedTypeStub;
            value.ComplexTypeTest = NestedTypeStub_AutoGeneratedTemplateSerializerStrategy.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 12 Name: ComplexArrayTest Type: NestedTypeStub[];
            value.ComplexArrayTest = FixedSizeComplexArrayTypeSerializerStrategy<NestedTypeStub_AutoGeneratedTemplateSerializerStrategy, NestedTypeStub, StaticTypedNumeric_Int32_1776>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 13 Name: OptionalValue Type: Int32;
            if (value.OptionalBoolCheck)value.OptionalValue = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 14 Name: TestCustomSerializerInt Type: Int32;
            value.TestCustomSerializerInt = TestCustomSerializer.Instance.Read(buffer, ref offset);
            //Type: TypeStub Field: 15 Name: EnumStringTestValue Type: TestEnum;
            value.EnumStringTestValue = InternalEnumExtensions.Parse<TestEnum>(DefaultStringSerializerHelper.Read(buffer, ref offset, EncodingType.ASCII, true), true);
            //Type: TypeStub Field: 99 Name: FinalArrayMemberWriteToEnd Type: Int32[];
            value.FinalArrayMemberWriteToEnd = PrimitiveArrayTypeSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
        }

        /// <summary>
        /// Auto-generated serialization/write method.
        /// Partial method implemented from shared partial definition.
        /// </summary>
        /// <param name="value">See external doc.</param>
        /// <param name="buffer">See external doc.</param>
        /// <param name="offset">See external doc.</param>
        protected internal override void InternalWrite(TypeStub value, Span<byte> buffer, ref int offset)
        {
            //Type: BaseTypeStub Field: 1 Name: BaseIntField Type: Int32;
            GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.BaseIntField, buffer, ref offset);
            //Type: TypeStub Field: 1 Name: Hello Type: Int32;
            GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.Hello, buffer, ref offset);
            //Type: TypeStub Field: 2 Name: TestString Type: String;
            LengthPrefixedStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, UTF32StringTerminatorTypeSerializerStrategy, UInt16>.Instance.Write(value.TestString, buffer, ref offset);
            //Type: TypeStub Field: 3 Name: HelloAgain Type: UInt16;
            GenericTypePrimitiveSerializerStrategy<UInt16>.Instance.Write(value.HelloAgain, buffer, ref offset);
            //Type: TypeStub Field: 4 Name: TestStringTwo Type: String;
            DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int16>.Instance.Write(value.TestStringTwo, buffer, ref offset);
            //Type: TypeStub Field: 5 Name: KnownSizeStringTest Type: String;
            KnownSizeStringSerializerHelper.Write(value.KnownSizeStringTest, buffer, ref offset, 1337, EncodingType.UTF16, false);
            //Type: TypeStub Field: 6 Name: DefaultStringTest Type: String;
            DefaultStringSerializerHelper.Write(value.DefaultStringTest, buffer, ref offset, EncodingType.UTF16, true);
            //Type: TypeStub Field: 6 Name: LongArrayTest Type: Int64[];
            SendSizePrimitiveArrayTypeSerializerStrategy<Int64, Int16>.Instance.Write(value.LongArrayTest, buffer, ref offset);
            //Type: TypeStub Field: 7 Name: KnownSizeArrayTest Type: UInt16[];
            FixedSizePrimitiveArrayTypeSerializerStrategy<UInt16, StaticTypedNumeric_Int32_1337>.Instance.Write(value.KnownSizeArrayTest, buffer, ref offset);
            //Type: TypeStub Field: 8 Name: EnumTestValue Type: TestEnum;
            GenericPrimitiveEnumTypeSerializerStrategy<TestEnum, UInt64>.Instance.Write(value.EnumTestValue, buffer, ref offset);
            //Type: TypeStub Field: 9 Name: LongArrayTestAddedSize Type: Int64[];
            SendSizePrimitiveArrayTypeSerializerStrategy<Int64, Int16>.Instance.Write(value.LongArrayTestAddedSize, buffer, ref offset);
            //Type: TypeStub Field: 10 Name: EnumTestValueSized Type: TestEnum;
            GenericPrimitiveEnumTypeSerializerStrategy<TestEnum, Byte>.Instance.Write(value.EnumTestValueSized, buffer, ref offset);
            //Type: TypeStub Field: 11 Name: ComplexTypeTest Type: NestedTypeStub;
            NestedTypeStub_AutoGeneratedTemplateSerializerStrategy.Instance.Write(value.ComplexTypeTest, buffer, ref offset);
            //Type: TypeStub Field: 12 Name: ComplexArrayTest Type: NestedTypeStub[];
            FixedSizeComplexArrayTypeSerializerStrategy<NestedTypeStub_AutoGeneratedTemplateSerializerStrategy, NestedTypeStub, StaticTypedNumeric_Int32_1776>.Instance.Write(value.ComplexArrayTest, buffer, ref offset);
            //Type: TypeStub Field: 13 Name: OptionalValue Type: Int32;
            if (value.OptionalBoolCheck)GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.OptionalValue, buffer, ref offset);
            //Type: TypeStub Field: 14 Name: TestCustomSerializerInt Type: Int32;
            TestCustomSerializer.Instance.Write(value.TestCustomSerializerInt, buffer, ref offset);
            //Type: TypeStub Field: 15 Name: EnumStringTestValue Type: TestEnum;
            DefaultStringSerializerHelper.Write(value.EnumStringTestValue, buffer, ref offset, EncodingType.ASCII, true);
            //Type: TypeStub Field: 99 Name: FinalArrayMemberWriteToEnd Type: Int32[];
            PrimitiveArrayTypeSerializerStrategy<Int32>.Instance.Write(value.FinalArrayMemberWriteToEnd, buffer, ref offset);
        }
        private sealed class StaticTypedNumeric_Int32_1337 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 1337; }
        private sealed class StaticTypedNumeric_Int32_69 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 69; }
        private sealed class StaticTypedNumeric_Int32_1776 : StaticTypedNumeric<Int32> { public sealed override Int32 Value => 1776; }
    }
}