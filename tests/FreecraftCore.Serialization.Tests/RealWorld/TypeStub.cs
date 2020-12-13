using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;
using JetBrains.Annotations;
using MyNamespace;

namespace FreecraftCore.Serializer.CustomTypes
{
	[PrimitiveGeneric]
	[WireDataContract]
	public class PrimitiveGenericTypeStub<T>
	{
		[WireMember(1)]
		public T Value { get; internal set; }

		public PrimitiveGenericTypeStub(T value)
		{
			Value = value;
		}

		public PrimitiveGenericTypeStub()
		{
			
		}
	}

	public enum TestEnum : ulong
	{
		Value1 = 0,
		Value2 = 1,
		Value3 = 2
	}

	[WireDataContract]
	public class NestedTypeStub
	{
		public NestedTypeStub()
		{
			
		}
	}

	[KnownGeneric(typeof(uint))]
	[KnownGeneric(typeof(int))]
	[WireDataContract]
	public class TypeStubGeneric<T>
	{
		[WireMember(1)]
		public T Value { get; internal set; }

		public TypeStubGeneric()
		{
			
		}
	}

	[KnownGeneric(typeof(uint), typeof(string))]
	[KnownGeneric(typeof(int), typeof(string))]
	[WireDataContract]
	public class TypeStubGenericDouble<T, A>
	{
		[WireMember(1)]
		public T Value { get; internal set; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(2)]
		public A Value2 { get; internal set; }

		public TypeStubGenericDouble()
		{

		}
	}

	[WireDataContractBaseType(2, typeof(TypeStub2))]
	[WireDataContract(PrimitiveSizeType.UInt16)]
	public abstract partial class BaseTypeStub
	{
		[WireMember(1)]
		public int BaseIntField { get; internal set; }

		protected BaseTypeStub()
		{

		}
	}

	public sealed class TestCustomSerializer : StatelessTypeSerializerStrategy<TestCustomSerializer, int>
	{
		public override int Read(Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}

		public override void Write(int value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}
	}

	[WireDataContract]
	public sealed partial class TypeStub2 : BaseTypeStub
	{
		[WireMember(1)]
		public int Hello { get; internal set; }

		public TypeStub2(int hello)
		{
			Hello = hello;
		}

		public TypeStub2()
		{
			
		}
	}

	[WireMessageType]
	[CustomTypeSerializer(typeof(TestCustomSerializerReferenceTypeSerializer))]
	[WireDataContract]
	public sealed partial class TestCustomSerializerReferenceType
	{
		[WireMember(1)]
		public int Value { get; internal set; }

		public TestCustomSerializerReferenceType()
		{
			
		}
	}

	[KnownGeneric(typeof(byte))]
	[WireDataContract]
	public partial class OpenGenericVector<T>
	{
		[WireMember(1)]
		public T Value1 { get; internal set; }

		[WireMember(2)]
		public T Value2 { get; internal set; }

		public OpenGenericVector()
		{
			
		}
	}

	public enum BaseEnumType
	{
		Test1 = 1,
	}

	public class TestEnumAttribute : WireDataContractBaseLinkAttribute
	{
		public TestEnumAttribute(BaseEnumType uniqueIndex) 
			: base((int)uniqueIndex)
		{

		}
	}

	[WireDataContract]
	[TestEnumAttribute(BaseEnumType.Test1)]
	public sealed partial class ChildEnumWireContractTestType : BaseEnumWireContractTestType
	{
		[WireMember(1)]
		public int Value { get; internal set; }

		public ChildEnumWireContractTestType()
		{
			
		}
	}

	[WireDataContract(PrimitiveSizeType.Int32)]
	public abstract partial class BaseEnumWireContractTestType
	{
		public BaseEnumWireContractTestType()
		{
			
		}
	}

	[WireDataContract(PrimitiveSizeType.Bit)]
	public abstract partial class BaseBitPolymorphicTestType
	{
		public BaseBitPolymorphicTestType()
		{

		}
	}

	public class NestedTestType
	{
		public enum TestEnumNested : ulong
		{
			Value1 = 0,
			Value2 = 1,
			Value3 = 2
		}
	}


	[WireMessageType]
	[WireDataContract]
	[WireDataContractBaseLink(1)]
	public sealed partial class TypeStub : BaseTypeStub, ISerializationEventListener
	{
		[WireMember(1)]
		public int Hello { get; internal set; }

		[SendSize(PrimitiveSizeType.UInt16)]
		[Encoding(EncodingType.UTF32)]
		[WireMember(2)]
		public string TestString { get; internal set; }

		[WireMember(3)]
		public ushort HelloAgain { get; internal set; }

		[DontTerminate]
		[SendSize(PrimitiveSizeType.Int16)]
		[WireMember(4)]
		public string TestStringTwo { get; internal set; }

		[DontTerminate]
		[Encoding(EncodingType.UTF16)]
		[KnownSize(1337)]
		[WireMember(5)]
		public string KnownSizeStringTest { get; internal set; }

		[Encoding(EncodingType.UTF16)]
		[WireMember(6)]
		public string DefaultStringTest { get; internal set; }

		[DontTerminate]
		[Encoding(EncodingType.UTF16)]
		[WireMember(6)]
		public string DefaultStringTestNotTerminated { get; internal set; }

		[SendSize(PrimitiveSizeType.Int16)]
		[WireMember(97)] //don't want to shift... But there was a typo
		public long[] LongArrayTest { get; internal set; }

		[KnownSize(1337)]
		[WireMember(98)]
		public ushort[] KnownSizeArrayTest { get; internal set; }

		[WireMember(8)]
		public TestEnum EnumTestValue { get; internal set; }

		[SendSize(PrimitiveSizeType.Int16)]
		[WireMember(9)]
		public long[] LongArrayTestAddedSize { get; internal set; }

		[EnumSize(PrimitiveSizeType.Byte)]
		[WireMember(10)]
		public TestEnum EnumTestValueSized { get; internal set; }

		[KnownSize(69)]
		[WireMember(11)]
		public NestedTypeStub ComplexTypeTest { get; internal set; }

		[KnownSize(1776)]
		[WireMember(12)]
		public NestedTypeStub[] ComplexArrayTest { get; internal set; }

		[Optional(nameof(OptionalBoolCheck))]
		[WireMember(13)]
		public int OptionalValue { get; internal set; }

		public bool OptionalBoolCheck { get; }

		[CustomTypeSerializer(typeof(TestCustomSerializer))]
		[WireMember(14)]
		public int TestCustomSerializerInt { get; internal set; }

		[EnumString]
		[WireMember(15)]
		public TestEnum EnumStringTestValue { get; internal set; }

		[WireMember(16)]
		public BaseTypeStub NestedPolymorphicTypeValue { get; internal set; }

		[ReverseData]
		[WireMember(17)]
		public int BigEndianIntTestValue { get; internal set; }

		[ReverseData]
		[EnumString]
		[WireMember(18)]
		public TestEnum EnumStringTestValueReversed { get; internal set; }

		[WireMember(19)]
		public TestCustomSerializerReferenceType CustomTypeSerializerTest { get; internal set; }

		[WireMember(20)]
		public int FieldTest;

		[WireMember(21)]
		public OpenGenericVector<int> OpenGenericTest1 { get; internal set; }

		[WireMember(22)]
		public OpenGenericVector<long> OpenGenericTest2 { get; internal set; }

		[WireMember(22)]
		public OpenGenericVector<byte> OpenGenericTest3 { get; internal set; }

		[WireMember(23)]
		public OpenGenericVector<byte>[] OpenGenericArray { get; internal set; }

		[WireMember(24)]
		public TestCustomSerializerReferenceType[] CustomTypeSerializerTestArray { get; internal set; }

		[CustomTypeSerializer(typeof(TestCustomSerializerReferenceTypeSerializerForPropertyTest))]
		[WireMember(25)]
		public TestCustomSerializerReferenceType CustomTypeSerializerPropertyAttri { get; internal set; }

		[WireMember(26)]
		public string[] StringArrayTest { get; internal set; }

		[SendSize(PrimitiveSizeType.Int64)]
		[WireMember(27)]
		public TestEnum[] EnumArrayTest { get; internal set; }

		[WireMember(28)]
		public NestedTestType.TestEnumNested NestedEnumTest { get; internal set; }

		//ALWAYS LAST, USES REMAINING BUFFER!
		[WireMember(99)]
		public int[] FinalArrayMemberWriteToEnd { get; internal set; }

		public TypeStub()
		{
			
		}

		public void OnBeforeSerialization()
		{

		}

		public void OnAfterDeserialization()
		{

		}
	}
}

namespace MyNamespace
{
	public sealed class TestCustomSerializerReferenceTypeSerializer : CustomTypeSerializerStrategy<TestCustomSerializerReferenceTypeSerializer, TestCustomSerializerReferenceType>
	{
		public override void InternalRead(TestCustomSerializerReferenceType value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}

		public override void InternalWrite(TestCustomSerializerReferenceType value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}
	}


	public sealed class TestCustomSerializerReferenceTypeSerializerForPropertyTest : CustomTypeSerializerStrategy<TestCustomSerializerReferenceTypeSerializerForPropertyTest, TestCustomSerializerReferenceType>
	{
		public override void InternalRead(TestCustomSerializerReferenceType value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}

		public override void InternalWrite(TestCustomSerializerReferenceType value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}
	}
}
