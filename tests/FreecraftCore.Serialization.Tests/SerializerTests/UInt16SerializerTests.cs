using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public class UInt16SerializerTests
	{
		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)28532)]
		public void Test_UInt16_Serializer_Doesnt_Throw_On_Serialize(UInt16 data)
		{
			var strategy = GenericTypePrimitiveSerializerStrategy<ushort>.Instance;
			int offset = 0;

			Assert.DoesNotThrow(() => strategy.Write(data, new Span<byte>(new byte[sizeof(ushort)]), ref offset));
		}

		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)27532)]
		public void Test_UInt16_Serializer_Writes_Int16s_Int16o_WriterStream(UInt16 data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<ushort>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(ushort)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);

			//assert
			Assert.False(offset == 0);
			Assert.True(offset == sizeof(ushort));
		}

		[Test]
		[TestCase(UInt16.MaxValue)]
		[TestCase(UInt16.MinValue)]
		[TestCase((ushort)500)]
		public void Test_Byte_Serializer_Writes_And_Reads_Same_Byte(UInt16 data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<ushort>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(ushort)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);
			offset = 0;
			UInt16 Int16value = strategy.Read(buffer, ref offset);

			//assert
			Assert.AreEqual(data, Int16value);
		}

		[Test]
		[TestCase((ushort)0)]
		[TestCase((ushort)5456)]
		[TestCase((ushort)23523)]
		[TestCase((ushort)562)]
		public static void Test_UShort_Serializer_Produces_Same_Values_As_Other_Methods(ushort data)
		{
			//arrange
			var strategy = GenericTypePrimitiveSerializerStrategy<ushort>.Instance;
			Span<byte> buffer = new Span<byte>(new byte[sizeof(ushort)]);
			int offset = 0;

			//act
			strategy.Write(data, buffer, ref offset);
			byte[] stratBytes = buffer.ToArray();
			byte[] bitConverted = BitConverter.GetBytes(data);
			byte[] bitConvertedFromInt = BitConverter.GetBytes((uint) data);
			byte[] typeStratConverter = buffer.ToArray();

			int tempOffset = 0;
			ReverseBinaryMutatorStrategy.Instance.Mutate(buffer, ref tempOffset, buffer, ref tempOffset);
			byte[] reversedData = buffer.ToArray();

			tempOffset = 0;
			offset = 0;
			ReverseBinaryMutatorStrategy.Instance.UnMutate(buffer, ref tempOffset, buffer, ref tempOffset);
			ushort data2 = strategy.Read(buffer, ref offset);

			//assert
			Assert.True(BitConverter.IsLittleEndian);
			Assert.AreEqual(data, data2);
			for (int i = 0; i < stratBytes.Length; i++)
			{
				Assert.AreEqual(stratBytes[i], bitConverted[i]);
				Assert.AreEqual(stratBytes[i], bitConvertedFromInt[i]);
				Assert.AreEqual(stratBytes[i], typeStratConverter[i]);
			}

			Assert.AreEqual(stratBytes.First(), reversedData.Last());
			Assert.AreEqual(stratBytes.Last(), reversedData.First());
		}
	}
}
