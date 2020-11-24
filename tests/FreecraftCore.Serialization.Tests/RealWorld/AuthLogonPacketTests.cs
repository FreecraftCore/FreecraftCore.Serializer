using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		/*[Test]
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
		}*/
	}

	/// <summary>
	/// Response payload sent in response to the <see cref="AuthLogonProofRequest"/>.
	/// </summary>
	[WireDataContract]
	[WireDataContractBaseLink((0x01 << 8) + 2)]
	public partial class AuthLogonProofResponse : AuthPacketBaseTest
	{
		/// <summary>
		/// Contains the information sent as a response to the Proof attempt.
		/// </summary>
		[WireMember(1)]
		public BaseLogonProofResult ProofResult { get; internal set; }

		//TODO: Add real ctor. Right now we only implement client stuff and this is sent by server.

		public AuthLogonProofResponse()
			: base((0x01 << 8) + 2)
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
	[WireDataContract(PrimitiveSizeType.Byte)] //the type information is sent as a byte
	public abstract partial class BaseLogonProofResult
	{
		/// <summary>
		/// Indicates the result of the proof.
		/// </summary>
		[EnumSize(PrimitiveSizeType.Byte)]
		[WireMember(1)]
		public AuthenticationResult Result { get; internal set; }

		public BaseLogonProofResult(AuthenticationResult result)
		{
			if(!Enum.IsDefined(typeof(AuthenticationResult), result)) throw new InvalidEnumArgumentException(nameof(result), (int)result, typeof(AuthenticationResult));
			Result = result;
		}
	}

	/// <summary>
	/// Sent by the server when a logon proof request was failed due to either an invalid SRP6 M sent
	/// or an invalid token (Authenticator pin) sent. (Ex. Invalid authenticator pin or invalid phone pin)
	/// </summary>
	[WireDataContract]
	[WireDataContractBaseLink(4)]
	public partial class LogonProofFailure : BaseLogonProofResult
	{
		//The below fields are always the same whether it's an invalid token or if it's an invalid SRP6 M sent.

		//TODO: What is this?
		//Trinitycore always sends 3
		[WireMember(1)]
		internal byte UnknownOne { get; set; } = 3;

		//TODO: What is this?
		[WireMember(2)]
		internal byte UnknownTwo { get; set; } = 0;

		//TODO: Only doing client stuff. Implement ctor later if/when we build a server.
		public LogonProofFailure()
			: base(AuthenticationResult.FailUnknownAccount)
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
	[WireDataContractBaseLink(0)]
	public partial class LogonProofSuccess : BaseLogonProofResult
	{
		/// <summary>
		/// SRP6 M2. See http://srp.stanford.edu/design.html for more information.
		/// (M2 = H(A, M (computed by client), K) where K is H(S) and S is session key. M2 proves server computed same K and recieved M1/M
		/// </summary>
		[WireMember(1)]
		[KnownSize(20)]
		public byte[] M2 { get; internal set; }

		//TODO: Accountflags? Trinitycore says this is:  0x01 = GM, 0x08 = Trial, 0x00800000 = Pro pass (arena tournament) but they always send "Pro Pass"?
		/// <summary>
		/// Indicates the authorization the client was granted.
		/// </summary>
		[EnumSize(PrimitiveSizeType.UInt32)]
		[WireMember(2)] //sent as a uint32
		public AccountAuthorizationFlags AccountAuthorization { get; internal set; }

		//TODO: What is survey ID? Always 0 on Trinitycore. Check mangos and EmberEmu
		[WireMember(3)]
		internal uint SurveyId { get; set; } = 0;

		//TODO: What is this? Always 0 from Trinitycore.
		[WireMember(4)]
		internal ushort unk3 { get; set; } = 0;

		//TODO: Proper Ctor. Right now we only implement client stuff. Server sends this.
		public LogonProofSuccess()
			: base(AuthenticationResult.Success)
		{

		}
	}

	[WireDataContract(PrimitiveSizeType.UInt16)]
	public abstract partial class AuthPacketBaseTest
	{
		[WireMember(1)]
		public ushort OperationCode { get; internal set; }

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		public AuthPacketBaseTest(ushort operationCode)
		{
			OperationCode = operationCode;
		}
	}
}