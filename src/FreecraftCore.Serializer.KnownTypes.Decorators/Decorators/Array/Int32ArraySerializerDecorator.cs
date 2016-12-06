using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/*unchecked
{
	fixed (int* intPtr = &value[0])
	{
		//Profiled and this is the fastest solution I could come up with.
		//There isn't a way to directly set the int array to the byte array.

		int size = value.Length * sizeof(int);
		byte* bPtr = (byte*)intPtr;
		byte[] bytes = new byte[size];

		for (int i = 0; i < size; i++)
		{
			bytes[i] = *(bPtr);

			bPtr++;
		}

		dest.Write(bytes);
	}
}*/

	/*public class Int32ArraySerializerDecorator : ArraySerializerDecorator<int>
	{
		public Int32ArraySerializerDecorator(IGeneralSerializerProvider serializerProvider) : base(serializerProvider)
		{

		}

		public unsafe override void Write(int[] value, IWireMemberWriterStrategy dest)
		{
			//Write byte as size
			dest.Write((byte)value.Length);

			//Believe or not this is an order of magnitude faster than the below pointer hacking
			//Even the pointer hacking was like 300% faster than manual writing
			FastArraySerializer.AsByteArray(value, bs => dest.Write(bs));
		}

		public unsafe override int[] Read(IWireMemberReaderStrategy source)
		{
			byte size = source.ReadByte();

			byte[] bytes = source.ReadBytes(size * sizeof(int));

			return FastArraySerializer.AsIntArrayPerm(bytes);
		}
	}

	//Based on hack from: http://stackoverflow.com/questions/619041/what-is-the-fastest-way-to-convert-a-float-to-a-byte
	public static unsafe class FastArraySerializer
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct Union
		{
			[FieldOffset(0)] public byte[] bytes;
			[FieldOffset(0)] public int[] ints;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ArrayHeader
		{
			public UIntPtr type;
			public UIntPtr length;
		}

		private static readonly UIntPtr BYTE_ARRAY_TYPE;
		private static readonly UIntPtr FLOAT_ARRAY_TYPE;

		static FastArraySerializer()
		{
			fixed (void* pBytes = new byte[1])
			fixed (void* pInts = new int[1])
			{
				BYTE_ARRAY_TYPE = getHeader(pBytes)->type;
				FLOAT_ARRAY_TYPE = getHeader(pInts)->type;
			}
		}

		public static void AsByteArray(this int[] intsArray, Action<byte[]> callback)
		{
			var union = new Union {ints = intsArray };
			union.ints.toByteArray();

			callback(union.bytes);

			union.bytes.toIntArray();
		}

		public static void AsIntArray(this byte[] bytes, Action<int[]> callback)
		{
			var union = new Union {bytes = bytes};
			union.bytes.toIntArray();

			callback(union.ints);

			union.ints.toByteArray();
		}

		public static int[] AsIntArrayPerm(this byte[] bytes)
		{
			var union = new Union { bytes = bytes };
			union.bytes.toIntArray();

			return union.ints;
		}

		public static bool handleNullOrEmptyArray<TSrc,TDst>(this TSrc[] array, Action<TDst[]> action)
		{
			if (array == null)
			{
				action(null);
				return true;
			}

			if (array.Length == 0)
			{
				action(new TDst[0]);
				return true;
			}

			return false;
		}

		private static ArrayHeader* getHeader(void* pBytes)
		{
			return (ArrayHeader*)pBytes - 1;
		}

		private static void toIntArray(this byte[] bytes)
		{
			fixed (void* pArray = bytes)
			{
				var pHeader = getHeader(pArray);

				pHeader->type = FLOAT_ARRAY_TYPE;
				pHeader->length = (UIntPtr)(bytes.Length / sizeof(int));
			}
		}

		private static void toByteArray(this int[] intArray)
		{
			fixed(void* pArray = intArray)
			{
				var pHeader = getHeader(pArray);

				pHeader->type = BYTE_ARRAY_TYPE;
				pHeader->length = (UIntPtr)(intArray.Length * sizeof(int));
			}
		}
	}*/
}
