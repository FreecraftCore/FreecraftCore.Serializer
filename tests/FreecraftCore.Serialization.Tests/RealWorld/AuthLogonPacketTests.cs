using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class AuthLogonPacketTests
	{
		private static byte[] realWorldBytes = new byte[]
		{
			1, //opcode

			0, //auth result (success)

			203, //20 byte M2 response
			164,
			231,
			13,
			97,
			45,
			211,
			167,
			253,
			241,
			138,
			250,
			202,
			47,
			151,
			53,
			6,
			192,
			213,
			118,

			0, //auth flags 4 byte uint flags enum
			0,
			128,
			0,

			0, //survey id 4 byte uint
			0,
			0,
			0,

			0, //unk3 ushort
			0
		};
	}
}
