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
			Assert.NotNull(result);
			Assert.AreEqual(value, result);
		}

		private sealed class Static_Int32_20 : StaticTypedNumeric<int>
		{
			public override int Value { get; } = 20;
		}
	}
}
