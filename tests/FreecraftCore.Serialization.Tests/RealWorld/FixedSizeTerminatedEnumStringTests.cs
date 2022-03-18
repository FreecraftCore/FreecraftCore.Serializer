using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests.RealWorld
{
	[TestFixture]
	public class FixedSizeTerminatedEnumStringTests
	{
		[Test]
		public void Test_Expected_Size_Is_Correct()
		{
			SerializerService service = new SerializerService();
			Span<byte> span = new Span<byte>(new byte[100]);
			int offset = 0;
			service.Write(new TestModel(TestEnum.WoW), span, ref offset);

			var array = span.Slice(0, offset).ToArray();
			Assert.AreEqual(4, array.Length);
		}
	}

	public enum TestEnum
	{
		WoW,
		WCA
	}

	[WireMessageType]
	[WireDataContract]
	public sealed partial class TestModel
	{
		[DontTerminate]
		[Encoding(EncodingType.ASCII)]
		[EnumString]
		[KnownSize(4)]
		[WireMember(1)]
		public TestEnum TestEnumField;

		public TestModel(TestEnum testEnumField)
		{
			TestEnumField = testEnumField;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		public TestModel()
		{
			
		}
	}
}
