using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FreecraftCore.Serializer;

namespace FreecraftCore.Serialization.Tests.RealWorld
{
	[TestFixture]
	public class BoomaProxyPacketPayloads
	{
		[Test]
		public void Test_Can_Register_Payload()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadClient_Serializer>();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadServer, PSOBBPatchPacketPayloadServer_Serializer>();
		}

		[Test]
		public void Test_Can_Serialize_Payload()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadClient_Serializer>();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadServer, PSOBBPatchPacketPayloadServer_Serializer>();
			byte[] bytes = Enumerable.Repeat((byte)55, 200).ToArray();

			//act
			int offset = 0;
			UnknownPatchPacket payload = serializer.Read<PSOBBPatchPacketPayloadServer>(new Span<byte>(bytes), ref offset)
				as UnknownPatchPacket;

			Assert.True(payload.UnknownBytes.Length == (200 - 2));

			for(int i = 0; i < bytes.Length - 2; i++)
				Assert.AreEqual(bytes[i], payload.UnknownBytes[i]);
		}

		[Test]
		public void Test_No_Stack_Overflow_On_Deserializing_PacketHeader()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadClient_Serializer>();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadServer, PSOBBPatchPacketPayloadServer_Serializer>();

			byte[] bytes = new byte[] { 55, 78 };

			serializer.Read<PSOBBPacketHeader>(new Span<byte>(bytes), 0);
		}

		[Test]
		public void Test_Patching_Payload_Deserializes_To_Correct_Values()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadClient_Serializer>();
			serializer.RegisterPolymorphicSerializer<PSOBBPatchPacketPayloadServer, PSOBBPatchPacketPayloadServer_Serializer>();
			Span<byte> buffer = new Span<byte>(new byte[500]); 

			PatchingWelcomePayload payload = new PatchingWelcomePayload("Patch Server. Copyright SonicTeam, LTD. 2001", 506953426, 214005626);

			//assert
			int offset = 0;
			serializer.Write(payload, buffer, ref offset);
			int size = offset;
			offset = 0;
			PatchingWelcomePayload deserializedPayload = (PatchingWelcomePayload)serializer.Read<PSOBBPatchPacketPayloadServer>(buffer.Slice(0, size), ref offset);

			//assert
			Assert.AreEqual(payload.PatchCopyrightMessage, deserializedPayload.PatchCopyrightMessage);
			Assert.AreEqual(payload.ClientVector, deserializedPayload.ClientVector);
			Assert.AreEqual(payload.ServerVector, deserializedPayload.ServerVector);
			Assert.AreEqual(0x99, deserializedPayload.OperationCode);
		}
	}

	/// <summary>
	/// The header for PSOBB packets.
	/// This looks non-standard according to documentation but for serialization purposes it's easier to
	/// write 
	/// </summary>
	[WireMessageType]
	[WireDataContract]
	public partial class PSOBBPacketHeader
	{
		/// <summary>
		/// The size of the packet.
		/// </summary>
		[WireMember(1)]
		public short PacketSize { get; internal set; }

		//The PacketSize contains the whole size of the packet
		//So the payload is just the size minus the size of the packetsize field.
		/// <summary>
		/// Indicates the size of the payload.
		/// </summary>
		public int PayloadSize => PacketSize - sizeof(short);

		/// <summary>
		/// Creates a new packet header with the specified size.
		/// </summary>
		/// <param name="packetSize">The packet size</param>
		public PSOBBPacketHeader(short packetSize)
		{
			PacketSize = packetSize;
		}

		/// <summary>
		/// Creates a new packet header with the specified size.
		/// </summary>
		/// <param name="packetSize">The packet size</param>
		public PSOBBPacketHeader(int packetSize)
		{
			PacketSize = (short)packetSize;
		}

		//serializer ctor
		public PSOBBPacketHeader()
		{

		}
	}

	//0x0C 0x00 0x11 0x00
	//PatchingByteLength{4} PatchFileCount{4}
	//Tethella implementation: https://github.com/justnoxx/psobb-tethealla/blob/master/patch_server/patch_server.c#L578
	//Sylverant implementation: https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.c#L237 and structure https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.h#L106
	[WireDataContract]
	[WireDataContractBaseLink(0x05)]
	public sealed partial class PatchingInformationPayload : PSOBBPatchPacketPayloadServer
	{
		//0x0C 0x00 Size
		//0x11 0x00 Type

		//If there is patching information it will send
		/// <summary>
		/// Indicates the length and size of the patching data.
		/// </summary>
		[WireMember(1)]
		public int PatchingByteLength { get; internal set; }

		/// <summary>
		/// Not 100% sure but looks like the number of files that need to be patched.
		/// </summary>
		[WireMember(2)]
		public int PatchFileCount { get; internal set; }

		public PatchingInformationPayload(int patchingByteLength, int patchFileCount)
			: this()
		{
			if(patchingByteLength < 0) throw new ArgumentOutOfRangeException(nameof(patchingByteLength));
			if(patchFileCount < 0) throw new ArgumentOutOfRangeException(nameof(patchFileCount));

			PatchingByteLength = patchingByteLength;
			PatchFileCount = patchFileCount;
		}

		//Serializer ctor
		public PatchingInformationPayload()
			: base(0x05)
		{

		}
	}

	/// <summary>
	/// The base type for PSOBB patching packet payloads that the server sends. This isn't for ship payloads. The child
	/// type based on a 2 byte opcode <see cref="ushort"/> that comes over the network.
	/// </summary>
	[DefaultChild(typeof(UnknownPatchPacket))] //this will be the default deserialized packet when we don't know what it is.
	[WireDataContract(PrimitiveSizeType.UInt16)]
	public abstract partial class PSOBBPatchPacketPayloadServer
	{
		//We really only add this because sometimes we'll get a packet we don't know about and we'll want to log about it.
		/// <summary>
		/// The operation code of the packet.
		/// </summary>
		[WireMember(1)]
		public ushort OperationCode { get; internal set; }

		//Nothing, only the 2 byte Type is relevant for this base packet.

		protected PSOBBPatchPacketPayloadServer(ushort operationCode)
		{
			OperationCode = operationCode;
		}
	}

	/// <summary>
	/// The default/unknown packet that is deserialized when an unknown
	/// or unimplemented opcode is encountered.
	/// </summary>
	[WireDataContract]
	public partial class UnknownPatchPacket : PSOBBPatchPacketPayloadServer
	{
		//We don't know what the packet is so we can't put any information here
		/// <summary>
		/// The entire unknown deserialized bytes for login packets.
		/// </summary>
		[ReadToEnd]
		[WireMember(1)]
		public byte[] UnknownBytes { get; internal set; }

		//Serializer ctor
		public UnknownPatchPacket()
			: base(0)
		{

		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"Unknown OpCode: #{OperationCode:X} Type: {base.ToString()} Size: {UnknownBytes.Length + 2}";
		}
	}

	/// <summary>
	/// The base type for PSOBB patching packet payloads that the client sends. This isn't for ship payloads The child
	/// type based on a 2 byte opcode <see cref="ushort"/> that comes over the network.
	/// </summary>
	[WireDataContract(PrimitiveSizeType.UInt16)]
	public abstract partial class PSOBBPatchPacketPayloadClient
	{
		//Nothing, only the 2 byte Type is relevant for this base packet.
		[WireMember(1)]
		public short OperationCode { get; internal set; }

		protected PSOBBPatchPacketPayloadClient()
		{

		}
	}

	//Syl struct: https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.h#L62
	/// <summary>
	/// The login request packet for the patching server.
	/// </summary>
	[WireDataContract]
	[WireDataContractBaseLink((int)0x07)]
	public sealed partial class PatchingLoginRequestPayload : PSOBBPatchPacketPayloadClient
	{
		/// <summary>
		/// Username to authenticate with the patchserver.
		/// </summary>
		[KnownSize(16)]
		[WireMember(2)]
		public string UserName { get; internal set; }

		/// <summary>
		/// Password to authenticate with the patchserver.
		/// </summary>
		[KnownSize(16)]
		[WireMember(3)]
		public string Password { get; internal set; }

		/// <summary>
		/// Padding (?)
		/// </summary>
		[KnownSize(64)]
		[WireMember(1)]
		internal byte[] Padding2 { get; set; } = new byte[64];

		/// <summary>
		/// Padding (?)
		/// </summary>
		[KnownSize(12)]
		[WireMember(4)]
		internal byte[] Padding { get; set; } = new byte[12];

		//serializer ctor

		public PatchingLoginRequestPayload(string userName, string password)
		{
			if(string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
			if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

			//TODO: Verify length, I have a headache right now.

			UserName = userName;
			Password = password;
		}

		//serializer ctor
		public PatchingLoginRequestPayload()
		{

		}
	}

	[WireDataContract]
	[WireDataContractBaseLink(0x99)]
	public sealed partial class PatchingWelcomePayload : PSOBBPatchPacketPayloadServer
	{
		/// <summary>
		/// Copyright message sent down from the patch server.
		/// Always the same message.
		/// </summary>
		[DontTerminate]
		[KnownSize(44)]
		[WireMember(1)]
		public string PatchCopyrightMessage { get; internal set; } //I don't think this is null terminated?

		//TODO: Why?
		[KnownSize(20)]
		[WireMember(2)]
		internal byte[] Padding { get; set; } = new byte[20];

		//TODO: What is this?
		/// <summary>
		/// Server IV (?)
		/// </summary>
		[WireMember(3)]
		public uint ServerVector { get; internal set; }

		/// <summary>
		/// Client IV (?)
		/// </summary>
		[WireMember(4)]
		public uint ClientVector { get; internal set; }

		public PatchingWelcomePayload(string patchCopyrightMessage, uint serverVector, uint clientVector)
			: this()
		{
			if(string.IsNullOrWhiteSpace(patchCopyrightMessage)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(patchCopyrightMessage));

			PatchCopyrightMessage = patchCopyrightMessage;
			ServerVector = serverVector;
			ClientVector = clientVector;
		}

		//serializer ctor
		public PatchingWelcomePayload()
			: base(0x99)
		{

		}
	}
}
