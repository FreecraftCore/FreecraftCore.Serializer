using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore.Serialization.Tests
{
	[TestFixture]
	public static class StringKnownSizeTests
	{
		[Test]
		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void Can_Serialize_KnownSize_Strings(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			if(shouldTerminate)
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			//assert
			Assert.AreNotEqual(0, offset);
			Assert.AreNotEqual(0, buffer[0]);
		}

		[Test]
		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void KnownSize_Strings_Serialize_Offset_Exact_Size(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			if(shouldTerminate)
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			int characterSize = GetFixedSizeCharacterSize(encoding);
			int terminatorSize = GetTerminatorSize(encoding);

			//assert
			Assert.AreEqual(fixedSize * characterSize + (shouldTerminate ? terminatorSize : 0), offset);
			Assert.AreNotEqual(0, buffer[0]);
		}

		private static int GetFixedSizeCharacterSize(EncodingType encoding)
		{
			switch (encoding)
			{
				case EncodingType.ASCII:
					return 1;
				case EncodingType.UTF16:
					return 4;
				case EncodingType.UTF32:
					return 4;
				case EncodingType.UTF8:
					return 4;
				default:
					throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
			}
		}

		private static int GetTerminatorSize(EncodingType encoding)
		{
			switch (encoding)
			{
				case EncodingType.ASCII:
					return 1;
				case EncodingType.UTF16:
					return 2;
				case EncodingType.UTF32:
					return 4;
				case EncodingType.UTF8:
					return 1;
				default:
					throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
			}
		}

		[Test]
		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void Can_Serializer_Deserialize_To_Equivalent_String(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			if (shouldTerminate)
			{
				switch (encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			offset = 0;
			string result = null;

			//act
			if (shouldTerminate)
			{
				switch (encoding)
				{
					case EncodingType.ASCII:
						result = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF16:
						result = FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF32:
						result = FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF8:
						result = FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (encoding)
				{
					case EncodingType.ASCII:
						result = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF16:
						result = FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF32:
						result = FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF8:
						result = FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			//assert
			Assert.NotNull(result);
			Assert.AreEqual(value, result);
		}

		[Test]
		public static void DontTerminate_UCS2_KnownSize_String_Reads_Embedded_Nulls()
		{
			//arrange
			const string Expected = "A\0B";
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			CustomCharacterEncodingHelpers.UCS2.GetBytes(Expected).CopyTo(buffer);

			//act
			string result = DontTerminateFixedSizeStringTypeSerializerStrategy<UCS2StringTypeSerializerStrategy, Static_Int32_16>
				.Instance.Read(buffer, ref offset);

			//assert
			Assert.True(result.StartsWith(Expected));
			Assert.AreEqual(32, offset);
		}

		[Test]
		public static void DontTerminate_ASCII_KnownSize_String_Reads_Data_After_Embedded_Null()
		{
			//arrange
			const string Expected = "PSO\0BB";
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			Encoding.ASCII.GetBytes(Expected).CopyTo(buffer);

			//act
			string result = DontTerminateFixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_16>
				.Instance.Read(buffer, ref offset);

			//assert
			Assert.True(result.StartsWith(Expected));
			Assert.AreEqual(16, offset);
		}

		[Test]
		public static void DontTerminate_UCS2_KnownSize_String_Roundtrips_Data_After_Embedded_Null()
		{
			//arrange
			const string Expected = "A\0B";
			int offset = 0;
			byte[] sourceBytes = new byte[1024];
			CustomCharacterEncodingHelpers.UCS2.GetBytes(Expected).CopyTo(sourceBytes, 0);
			Span<byte> sourceBuffer = new Span<byte>(sourceBytes);

			//act
			string result = DontTerminateFixedSizeStringTypeSerializerStrategy<UCS2StringTypeSerializerStrategy, Static_Int32_16>
				.Instance.Read(sourceBuffer, ref offset);

			offset = 0;
			byte[] roundtripBytes = new byte[1024];
			Span<byte> roundtripBuffer = new Span<byte>(roundtripBytes);
			DontTerminateFixedSizeStringTypeSerializerStrategy<UCS2StringTypeSerializerStrategy, Static_Int32_16>
				.Instance.Write(result, roundtripBuffer, ref offset);

			//assert
			Assert.AreEqual(32, offset);
			CollectionAssert.AreEqual(sourceBytes, roundtripBytes);
			Assert.True(result.StartsWith(Expected));
		}

		[Test]
		public static void Generated_DontTerminate_UCS2_KnownSize_String_Reads_Embedded_Nulls()
		{
			//arrange
			const string Expected = "A\0B";
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);
			CustomCharacterEncodingHelpers.UCS2.GetBytes(Expected).CopyTo(buffer);

			//act
			DontTerminateKnownSizeUcs2Model result = new SerializerService()
				.Read<DontTerminateKnownSizeUcs2Model>(buffer, ref offset);

			//assert
			Assert.True(result.Value.StartsWith(Expected));
			Assert.AreEqual(32, offset);
		}

		[Test]
		[TestCase("Hello", EncodingType.ASCII, false, 20)]
		[TestCase("Hello!", EncodingType.ASCII, false, 20)]
		[TestCase("Testing", EncodingType.ASCII, false, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, false, 20)]
		[TestCase("Hello", EncodingType.ASCII, true, 20)]
		[TestCase("Hello!", EncodingType.ASCII, true, 20)]
		[TestCase("Testing", EncodingType.ASCII, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.ASCII, true, 20)]
		[TestCase("Hello", EncodingType.UTF32, true, 20)]
		[TestCase("Hello!", EncodingType.UTF32, true, 20)]
		[TestCase("Testing", EncodingType.UTF32, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF32, true, 20)]
		[TestCase("Hello", EncodingType.UTF8, true, 20)]
		[TestCase("Hello!", EncodingType.UTF8, true, 20)]
		[TestCase("Testing", EncodingType.UTF8, true, 20)]
		[TestCase("WooOooOoOo", EncodingType.UTF8, true, 20)]
		public static void KnownSize_Strings_Deserialize_Correct_Offset(string value, EncodingType encoding, bool shouldTerminate, int fixedSize)
		{
			//arrange
			int offset = 0;
			Span<byte> buffer = new Span<byte>(new byte[1024]);

			//act
			if(shouldTerminate)
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF16:
						FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF32:
						FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					case EncodingType.UTF8:
						FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Write(value, buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			offset = 0;
			string result = null;

			//act
			if(shouldTerminate)
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						result = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20, ASCIIStringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF16:
						result = FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20, UTF16StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF32:
						result = FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20, UTF32StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF8:
						result = FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20, UTF8StringTerminatorTypeSerializerStrategy>
							.Instance.Read(buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch(encoding)
				{
					case EncodingType.ASCII:
						result = FixedSizeStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF16:
						result = FixedSizeStringTypeSerializerStrategy<UTF16StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF32:
						result = FixedSizeStringTypeSerializerStrategy<UTF32StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					case EncodingType.UTF8:
						result = FixedSizeStringTypeSerializerStrategy<UTF8StringTypeSerializerStrategy, Static_Int32_20>
							.Instance.Read(buffer, ref offset);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			//assert
			int characterSize = GetFixedSizeCharacterSize(encoding);
			int terminatorSize = GetTerminatorSize(encoding);

			//This checks that we READ to the offset we expected when fixed length reading.
			Assert.AreEqual(fixedSize * characterSize + (shouldTerminate ? terminatorSize : 0), offset);
		}

		private sealed class Static_Int32_20 : StaticTypedNumeric<int>
		{
			public override int Value { get; } = 20;
		}

		private sealed class Static_Int32_16 : StaticTypedNumeric<int>
		{
			public override int Value { get; } = 16;
		}
	}

	[WireMessageType]
	[WireDataContract]
	public sealed partial class DontTerminateKnownSizeUcs2Model
	{
		[Encoding(EncodingType.UCS2)]
		[DontTerminate]
		[KnownSize(16)]
		[WireMember(1)]
		public string Value { get; internal set; }

		public DontTerminateKnownSizeUcs2Model(string value)
			: this()
		{
			Value = value;
		}

		public DontTerminateKnownSizeUcs2Model()
		{

		}
	}
}
