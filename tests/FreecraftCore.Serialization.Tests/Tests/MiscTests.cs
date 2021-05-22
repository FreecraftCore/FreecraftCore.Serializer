using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Reinterpret.Net;

namespace FreecraftCore
{
	[TestFixture]
	public static class MiscTests
	{
		[Test]
		[TestCase(1)]
		[TestCase(0)]
		[TestCase(byte.MaxValue)]
		[TestCase(byte.MinValue)]
		[TestCase(55)]
		[TestCase(66)]
		public static void Test_UnsafeAs_Safe_Byte_To_Int_Cast(byte value)
		{
			Assert.AreEqual((int)value, value.Reinterpret<byte, int>());
		}

		[Test]
		[TestCase(1)]
		[TestCase(0)]
		[TestCase((int)byte.MaxValue)]
		[TestCase((int)byte.MinValue)]
		[TestCase(55)]
		[TestCase(66)]
		public static void Test_UnsafeAs_Safe_Int_To_Byte_Cast(int value)
		{
			Assert.AreEqual((byte)value, value.Reinterpret<int, byte>());
		}
	}
}
