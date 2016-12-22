using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Strategy.Tests
{
	[TestFixture]
	public class SendSizeContextKeyTests
	{
		[Test]
		public static void Test_SendSizeContextKey_Doesnt_Throw_On_Ctor()
		{
			//Originally I took the typeof() the wrong type to check if the enum was defined. This was causing exceptions
			//arrange
			Assert.DoesNotThrow(() => new SendSizeContextKey(SendSizeAttribute.SizeType.Int32));
		}
	}
}
