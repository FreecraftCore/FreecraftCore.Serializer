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

	}

	/// <summary>
	/// The header for PSOBB packets.
	/// This looks non-standard according to documentation but for serialization purposes it's easier to
	/// write 
	/// </summary>
	[WireDataContract]
	public class PSOBBPacketHeader
	{
		/// <summary>
		/// The size of the packet.
		/// </summary>
		[WireMember(1)]
		public short PacketSize { get; }

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
		private PSOBBPacketHeader()
		{

		}
	}

	//0x0C 0x00 0x11 0x00
	//PatchingByteLength{4} PatchFileCount{4}
	//Tethella implementation: https://github.com/justnoxx/psobb-tethealla/blob/master/patch_server/patch_server.c#L578
	//Sylverant implementation: https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.c#L237 and structure https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.h#L106
	[WireDataContract]
	[WireDataContractBaseLink(0x05, typeof(PSOBBPatchPacketPayloadServer))]
	public sealed class PatchingInformationPayload : PSOBBPatchPacketPayloadServer
	{
		//0x0C 0x00 Size
		//0x11 0x00 Type

		//If there is patching information it will send
		/// <summary>
		/// Indicates the length and size of the patching data.
		/// </summary>
		[WireMember(1)]
		public int PatchingByteLength { get; }

		/// <summary>
		/// Not 100% sure but looks like the number of files that need to be patched.
		/// </summary>
		[WireMember(2)]
		public int PatchFileCount { get; }

		public PatchingInformationPayload(int patchingByteLength, int patchFileCount)
		{
			if(patchingByteLength < 0) throw new ArgumentOutOfRangeException(nameof(patchingByteLength));
			if(patchFileCount < 0) throw new ArgumentOutOfRangeException(nameof(patchFileCount));

			PatchingByteLength = patchingByteLength;
			PatchFileCount = patchFileCount;
		}

		//Serializer ctor
		protected PatchingInformationPayload()
			: base()
		{

		}
	}

	/// <summary>
	/// The base type for PSOBB patching packet payloads that the server sends. This isn't for ship payloads. The child
	/// type based on a 2 byte opcode <see cref="ushort"/> that comes over the network.
	/// </summary>
	[DefaultChild(typeof(UnknownPatchPacket))] //this will be the default deserialized packet when we don't know what it is.
	[WireDataContract(PrimitiveSizeType.UInt16)]
	public abstract class PSOBBPatchPacketPayloadServer
	{
		//We really only add this because sometimes we'll get a packet we don't know about and we'll want to log about it.
		/// <summary>
		/// The operation code of the packet.
		/// </summary>
		[WireMember(1)]
		public short OperationCode { get; }

		//Nothing, only the 2 byte Type is relevant for this base packet.

		protected PSOBBPatchPacketPayloadServer()
		{

		}
	}

	/// <summary>
	/// The default/unknown packet that is deserialized when an unknown
	/// or unimplemented opcode is encountered.
	/// </summary>
	public class UnknownPatchPacket : PSOBBPatchPacketPayloadServer
	{
		//We don't know what the packet is so we can't put any information here
		/// <summary>
		/// The entire unknown deserialized bytes for login packets.
		/// </summary>
		[ReadToEnd]
		[WireMember(1)]
		public byte[] UnknownBytes { get; }

		//Serializer ctor
		private UnknownPatchPacket()
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
	public abstract class PSOBBPatchPacketPayloadClient
	{
		//Nothing, only the 2 byte Type is relevant for this base packet.
		[WireMember(1)]
		public short OperationCode { get; }

		protected PSOBBPatchPacketPayloadClient()
		{

		}
	}

	//Syl struct: https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.h#L62
	/// <summary>
	/// The login request packet for the patching server.
	/// </summary>
	[WireDataContract]
	[WireDataContractBaseLink((int)0x07, typeof(PSOBBPatchPacketPayloadClient))]
	public sealed class PatchingLoginRequestPayload : PSOBBPatchPacketPayloadClient
	{
		/// <summary>
		/// Username to authenticate with the patchserver.
		/// </summary>
		[KnownSize(16)]
		[WireMember(2)]
		public string UserName { get; }

		/// <summary>
		/// Password to authenticate with the patchserver.
		/// </summary>
		[KnownSize(16)]
		[WireMember(3)]
		public string Password { get; }

		/// <summary>
		/// Padding (?)
		/// </summary>
		[KnownSize(64)]
		[WireMember(1)]
		private byte[] Padding2 { get; } = new byte[64];

		/// <summary>
		/// Padding (?)
		/// </summary>
		[KnownSize(12)]
		[WireMember(4)]
		private byte[] Padding { get; } = new byte[12];

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
		private PatchingLoginRequestPayload()
		{

		}
	}

	[WireDataContract]
	[WireDataContractBaseLink(0x99, typeof(PSOBBPatchPacketPayloadServer))]
	public sealed class PatchingWelcomePayload : PSOBBPatchPacketPayloadServer
	{
		/// <summary>
		/// Copyright message sent down from the patch server.
		/// Always the same message.
		/// </summary>
		[DontTerminate]
		[KnownSize(44)]
		[WireMember(1)]
		public string PatchCopyrightMessage { get; } //I don't think this is null terminated?

		//TODO: Why?
		[KnownSize(20)]
		[WireMember(2)]
		private byte[] Padding { get; } = new byte[20];

		//TODO: What is this?
		/// <summary>
		/// Server IV (?)
		/// </summary>
		[WireMember(3)]
		public uint ServerVector { get; }

		/// <summary>
		/// Client IV (?)
		/// </summary>
		[WireMember(4)]
		public uint ClientVector { get; }

		public PatchingWelcomePayload(string patchCopyrightMessage, uint serverVector, uint clientVector)
		{
			if(string.IsNullOrWhiteSpace(patchCopyrightMessage)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(patchCopyrightMessage));

			PatchCopyrightMessage = patchCopyrightMessage;
			ServerVector = serverVector;
			ClientVector = clientVector;
		}

		//serializer ctor
		private PatchingWelcomePayload()
		{

		}
	}
}
