using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests.RealWorld
{
	[TestFixture]
	public class AuthTokenTests
	{
		static byte[] realworldBytes =
		{
			41,
			0,
			0,
			0,
			0,
			0,

			1,

			0,
			0,
			0,
			0,

			84,
			114,
			105,
			110,
			105,
			116,
			121,
			0,

			49,
			50,
			55,
			46,
			48,
			46,
			48,
			46,
			49,
			58,
			56,
			48,
			56,
			53,
			0,

			0,
			0,
			0,
			0,
			0,

			1,
			1,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
		};

		private static string realWorldBytestring = @"IAVBabsGXv8NjxxvgKQ6ZQyMHeDaMWVja+ifBFZ8KntSBkdMQURFUjQwAQAAVHJpbml0eQAxMjcuMC4wLjE6ODA4NQAAAAAAAgEB";
	}
}
