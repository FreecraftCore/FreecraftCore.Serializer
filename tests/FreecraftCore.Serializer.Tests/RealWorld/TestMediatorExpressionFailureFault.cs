using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FreecraftCore.Serializer.Tests.RealWorld
{
	[TestFixture]
	public static class TestMediatorExpressionFailureFault
	{
		[Test]
		public static void Test_Register_Complex_RealWorld_Type_Doesnt_Throw()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			Assert.DoesNotThrow(() => serializer.RegisterType<AuthLogonChallengeRequest>());
		}

		[WireDataContract]
		public enum ClientBuild : ushort
		{
			Test = 1,
		}

		public enum GameType
		{
			
		}

		public enum LocaleType
		{
			
		}

		public enum OperatingSystemType : short
		{
			
		}

		public enum PlatformType : byte
		{
			
		}

		public enum ProtocolVersion : uint
		{
			
		}

		[WireDataContract]
		public interface IAuthenticationPayload
		{
			
		}

		[WireDataContract(WireDataContractAttribute.KeyType.None, InformationHandlingFlags.Default, false)]
		public class AuthLogonChallengeRequest : IAuthenticationPayload
		{
			public AuthLogonChallengeRequest()
			{
				
			}

			/// <summary>
			/// Authentication protocol version to use.
			/// Trinitycore/Mangos has this marked as an error but Ember https://github.com/EmberEmu/Ember/blob/spark-new/src/login/grunt/client/LoginChallenge.h has this
			/// marked as the protocol field.
			/// </summary>
			[WireMember(1)]
			public ProtocolVersion Protocol { get; private set; }

			//We don't need to expose this really. Shouldn't need to be checked. This isn't C++
			/// <summary>
			/// Packet size. Computed by Trinitycore as the size of the payload + the username size.
			/// </summary>
			[WireMember(2)]
			private readonly ushort size;

			//Could be reversed?
			/// <summary>
			/// Game the client is for.
			/// </summary>
			[EnumString]
			[KnownSize(3)]
			[WireMember(3)]
			public GameType Game { get; private set; }

			/// <summary>
			/// Indicates the expansion this client is authenticating for.
			/// </summary>
			[WireMember(4)]
			public AuthRealmListPacketTest.ExpansionType Expansion { get; private set; }

			/// <summary>
			/// Indicates the major patch version (Ex. x.3.x)
			/// </summary>
			[WireMember(5)]
			public byte MajorPatchVersion { get; private set; }

			/// <summary>
			/// Indicates the major patch version (Ex. x.x.5)
			/// </summary>
			[WireMember(6)]
			public byte MinorPatchVersion { get; private set; }

			//TODO: Enumerate this maybe?
			[WireMember(7)]
			public ClientBuild Build { get; private set; }

			/// <summary>
			/// Indicates the platform/arc (Ex. 32bit or 64bit)
			/// </summary>
			[EnumString]
			[ReverseData]
			[KnownSize(3)]
			[WireMember(8)]
			public PlatformType Platform { get; private set; }

			/// <summary>
			/// Indicates the operating system the client is running on (Ex. Win or Mac)
			/// </summary>
			[EnumString]
			[ReverseData]
			[KnownSize(3)]
			[WireMember(9)]
			public OperatingSystemType OperatingSystem { get; private set; }

			/// <summary>
			/// Indicates the Locale of the client. (Ex. En-US)
			/// </summary>
			[EnumString]
			[ReverseData]
			[DontTerminate] //Locale also doesn't terminate. It is a char[4] like "SUne" without a terminator.
			[KnownSize(4)]
			[WireMember(10)]
			public LocaleType Locale { get; private set; }

			//TODO: Timezone bias? Investigate values.
			[WireMember(11)]
			private uint TimeZoneBias { get; set; }

			[KnownSize(4)]
			[WireMember(12)]
			private readonly byte[] ipAddressInBytes;

			//Lazily cached Ip built from wired bytes
			private Lazy<IPAddress> cachedIp { get; }

			//TODO: Thread safety
			/// <summary>
			/// IP Address of the client.
			/// </summary>
			public IPAddress IP => cachedIp.Value;

			/// <summary>
			/// Could be Username or maybe Email.
			/// </summary>
			//TODO: Check Mangos if they look for a null terminator on Identity
			[DontTerminate] //JackPoz doesn't terminate and it looks like Trinitycore doesn't really expect a null terminator either.
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[WireMember(13)]
			public string Identity { get; private set; }
		}
	}
}
