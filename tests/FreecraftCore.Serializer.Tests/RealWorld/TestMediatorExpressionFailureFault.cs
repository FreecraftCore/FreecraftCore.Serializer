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
		public enum ClientBuild
		{
			
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

			[WireMember(6)]
			public ClientBuild Build { get; private set; }
			[WireMember(4)]
			public AuthRealmListPacketTest.ExpansionType Expansion { get; private set; }
			[EnumString]
			[KnownSize(3)]
			[WireMember(3)]
			public GameType Game { get; private set; }
			[DontTerminate]
			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[WireMember(12)]
			public string Identity { get; private set; }
			public IPAddress IP { get; private set; }
			[DontTerminate]
			[EnumString]
			[KnownSize(4)]
			[ReverseData]
			[WireMember(9)]
			public LocaleType Locale { get; private set; }
			[WireMember(5)]
			public byte MajorPatchVersion { get; private set; }
			[WireMember(5)]
			public byte MinorPatchVersion { get; private set; }
			[EnumString]
			[KnownSize(3)]
			[ReverseData]
			[WireMember(8)]
			public readonly OperatingSystemType OperatingSystem;
			[EnumString]
			[KnownSize(3)]
			[ReverseData]
			[WireMember(7)]
			public PlatformType Platform { get; private set; }
			[WireMember(1)]
			public ProtocolVersion Protocol { get; private set; }
		}
	}
}
