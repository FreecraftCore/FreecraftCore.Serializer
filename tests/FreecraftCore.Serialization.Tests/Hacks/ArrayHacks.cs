using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class ArrayHacks
	{
		/// <summary>
		/// Header for Array types.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ArrayHeader
		{
			public UIntPtr type;
			public UIntPtr length;
		}

		//TODO: This crashes the test runner. Does this also crash the runtime?
		//[Test]
		//[TestCase(5)]
		public static unsafe void Test_Header_Change_Size_Hack(int n)
		{
			//arrange
			byte[] array = new byte[n];

			//Remove following 0 from end (clean)
			fixed (void* bytePtr = &array[0])
			{
				//Grabs the header for the array
				ArrayHeader* arrayHeader = (ArrayHeader*) bytePtr - 1;


				Assert.AreEqual(n, (int)arrayHeader->length);
				//Hacks the array length to be one less than the original size.
				arrayHeader->length = (UIntPtr)(array.Length - 1);
			}

			//assert
			Assert.AreEqual(n - 1, array.LongLength);

			//Check iteration size
			int count = 0;

			foreach (byte b in array)
				count++;

			Assert.AreEqual(n - 1, count);
		}
	}
}
