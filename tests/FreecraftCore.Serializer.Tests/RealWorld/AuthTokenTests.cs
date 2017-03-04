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

		[Test]
		public static void Test_Serializer_Auth_Token()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<AuthenticationToken>();
			serializer.RegisterType<AuthRealmListPacketTest.AuthRealmListResponse>();
			serializer.Compile();

			//act
			AuthRealmListPacketTest.AuthRealmListResponse response = serializer.Deserialize<AuthRealmListPacketTest.AuthRealmListResponse>(new DefaultStreamReaderStrategy(realworldBytes));

			//act
			AuthenticationToken token = serializer.Deserialize<AuthenticationToken>(Encoding.UTF8.GetBytes(realWorldBytestring));

			Assert.AreEqual("GLADER", token.AccountName);
		}

		/// <summary>
		/// Simple issuable token that contains the data required to connect to a gameserver.
		/// </summary>
		[WireDataContract]
		public class AuthenticationToken
		{
			[WireMember(1)]
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			public byte[] SessionKey { get; private set; }

			[WireMember(2)]
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[DontTerminate]
			public string AccountName { get; private set; }

			[WireMember(3)]
			public TestMediatorExpressionFailureFault.ClientBuild Client { get; private set; }

			[WireMember(4)]
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			public AuthRealmListPacketTest.RealmInfo[] RealmInfo { get; private set; }

			public AuthenticationToken(TestMediatorExpressionFailureFault.ClientBuild client, string accountName, byte[] sessionKey, AuthRealmListPacketTest.RealmInfo[] realmInfo)
			{
				if (!Enum.IsDefined(typeof(TestMediatorExpressionFailureFault.ClientBuild), client))
					throw new ArgumentOutOfRangeException(nameof(client), "Value should be defined in the ClientBuild enum.");

				if (string.IsNullOrWhiteSpace(accountName))
					throw new ArgumentException("Value cannot be null or whitespace.", nameof(accountName));

				if (sessionKey == null)
					throw new ArgumentNullException(nameof(sessionKey));
				if (realmInfo == null) throw new ArgumentNullException(nameof(realmInfo));

				Client = client;
				AccountName = accountName;
				SessionKey = sessionKey;
				RealmInfo = realmInfo;
			}

			protected AuthenticationToken()
			{

			}
		}
	}
}
