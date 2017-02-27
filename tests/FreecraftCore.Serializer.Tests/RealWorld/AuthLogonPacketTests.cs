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

		[Test]
		public static void Test_Can_Register_AuthProofResponse()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<AuthPacketBaseTest>();
			serializer.Link<AuthLogonProofResponse, AuthPacketBaseTest>();

			serializer.Compile();
		}

		[Test]
		public static void Test_RealWorldBytes_Are_32Bytes()
		{
			//assert
			Assert.AreEqual(32, realWorldBytes.Length);
		}

		[Test]
		public static async Task Test_Can_Deserialize_AuthLogonProofResponse()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<AuthPacketBaseTest>();
			serializer.RegisterType<AuthLogonProofResponse>();
			serializer.Link<AuthLogonProofResponse, AuthPacketBaseTest>();

			serializer.Compile();
			//act
			AuthPacketBaseTest test = await serializer.DeserializeAsync<AuthPacketBaseTest>(new DefaultStreamReaderStrategyAsync(realWorldBytes.Skip(1).ToArray())
				.PeekWithBufferingAsync()
				.PreprendWithBytesAsync(new byte[] { 0x01 })
				.PreprendWithBytesAsync(new byte[] { 2 }));
		}

		/// <summary>
		/// Response payload sent in response to the <see cref="AuthLogonProofRequest"/>.
		/// </summary>
		[WireDataContract]
		[WireDataContractBaseTypeRuntimeLink((0x01 << 8) + 2)]
		public class AuthLogonProofResponse : AuthPacketBaseTest
		{
			//Not a wire member. Pull from proof result. It eats the byte for type info
			/// <summary>
			/// Indicates the result of the Authentication attempt.
			/// </summary>
			public AuthenticationResult AuthResult => ProofResult.Result;

			/// <summary>
			/// Contains the information sent as a response to the Proof attempt.
			/// </summary>
			[WireMember(1)]
			public ILogonProofResult ProofResult { get; private set; }

			//TODO: Add real ctor. Right now we only implement client stuff and this is sent by server.

			public AuthLogonProofResponse()
			{

			}
		}

		public enum AuthenticationResult : byte
		{
			Success = 0,

			FailUnknownAccount = 4
		}

		//TODO: Check mangos 1.12.1 for other possible LoginProof results. Also, we need to implement 1.12.1 result. Only have 3.3.5 at the moment.
		/// <summary>
		/// Contract for all proof results.
		/// </summary>
		[WireDataContract(WireDataContractAttribute.KeyType.Byte)] //the type information is sent as a byte
		[WireDataContractBaseType(0, typeof(LogonProofSuccess))] //0 in the stream means success
		[WireDataContractBaseType(4, typeof(LogonProofFailure))] //4 means token failure.
		public interface ILogonProofResult
		{
			/// <summary>
			/// Indicates the result of the proof.
			/// </summary>
			AuthenticationResult Result { get; }
		}

		/// <summary>
		/// Sent by the server when a logon proof request was failed due to either an invalid SRP6 M sent
		/// or an invalid token (Authenticator pin) sent. (Ex. Invalid authenticator pin or invalid phone pin)
		/// </summary>
		[WireDataContract]
		public class LogonProofFailure : ILogonProofResult
		{
			//This is sent when SRP6 was invalid or Token failed
			/// <summary>
			/// Indicates a failure to authenticate.
			/// </summary>
			public AuthenticationResult Result { get; } = AuthenticationResult.FailUnknownAccount;

			//The below fields are always the same whether it's an invalid token or if it's an invalid SRP6 M sent.

			//TODO: What is this?
			//Trinitycore always sends 3
			[WireMember(1)]
			private readonly byte unknownOne = 3;

			//TODO: What is this?
			[WireMember(2)]
			private readonly byte unknownTwo = 0;

			//TODO: Only doing client stuff. Implement ctor later if/when we build a server.

			public LogonProofFailure()
			{

			}
		}

		public enum AccountAuthorizationFlags : uint
		{
			None = 0,
			GM = 1,
			Trial = 8,
			ProPass = 8388608
		}

		/// <summary>
		/// Sent by the server when a logon proof request was successful.
		/// Only for >= 2.x.x clients. 1.12.1 clients recieve something slightly different.
		/// </summary>
		[WireDataContract]
		public class LogonProofSuccess : ILogonProofResult
		{
			/// <summary>
			/// Indicates that the result of the logon attempt was successful.
			/// </summary>
			public AuthenticationResult Result { get; } = AuthenticationResult.Success;

			/// <summary>
			/// SRP6 M2. See http://srp.stanford.edu/design.html for more information.
			/// (M2 = H(A, M (computed by client), K) where K is H(S) and S is session key. M2 proves server computed same K and recieved M1/M
			/// </summary>
			[WireMember(1)]
			[KnownSize(20)]
			public byte[] M2 { get; private set; }

			//TODO: Accountflags? Trinitycore says this is:  0x01 = GM, 0x08 = Trial, 0x00800000 = Pro pass (arena tournament) but they always send "Pro Pass"?
			/// <summary>
			/// Indicates the authorization the client was granted.
			/// </summary>
			[WireMember(2)] //sent as a uint32
			public AccountAuthorizationFlags AccountAuthorization { get; private set; }

			//TODO: What is survey ID? Always 0 on Trinitycore. Check mangos and EmberEmu
			[WireMember(3)]
			private readonly uint surveyId = 0;

			//TODO: What is this? Always 0 from Trinitycore.
			[WireMember(4)]
			private readonly ushort unk3 = 0;

			//TODO: Proper Ctor. Right now we only implement client stuff. Server sends this.

			public LogonProofSuccess()
			{

			}
		}

		[WireDataContract(WireDataContractAttribute.KeyType.UShort, InformationHandlingFlags.DontWrite, true)]
		public abstract class AuthPacketBaseTest
		{
			
		}
	}
}
