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
			serializer.RegisterType(typeof(PSOBBPatchPacketPayloadServer));
		}

		[Test]
		public void Test_Can_Serialize_Payload()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType(typeof(PSOBBPatchPacketPayloadServer));
			byte[] bytes = Enumerable.Repeat((byte)55, 200).ToArray();

			//act
			serializer.Compile();
			UnknownPatchPacket payload = serializer.Deserialize<PSOBBPatchPacketPayloadServer>(bytes)
				as UnknownPatchPacket;

			Assert.True(payload.UnknownBytes.Length == (200 - 2));

			for(int i = 0; i < bytes.Length - 2; i++)
				Assert.AreEqual(bytes[i], payload.UnknownBytes[i]);
		}

		[Test]
		public void Test_Can_Register_PatchingUpOneDir()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType(typeof(PatchingInformationPayload));
		}

		[Test]
		public void Test_Can_Link_PatchingUpOneDir()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.Link<PatchingInformationPayload, PSOBBPatchPacketPayloadServer>();
		}
	}

	//0x0C 0x00 0x11 0x00
	//PatchingByteLength{4} PatchFileCount{4}
	//Tethella implementation: https://github.com/justnoxx/psobb-tethealla/blob/master/patch_server/patch_server.c#L578
	//Sylverant implementation: https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.c#L237 and structure https://github.com/Sylverant/patch_server/blob/master/src/patch_packets.h#L106
	[WireDataContract]
	[WireDataContractBaseLink(0x05)]
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
	[WireDataContract(WireDataContractAttribute.KeyType.UShort, InformationHandlingFlags.DontConsumeRead, true)]
	public abstract class PSOBBPatchPacketPayloadServer
	{
		//We really only add this because sometimes we'll get a packet we don't know about and we'll want to log about it.
		/// <summary>
		/// The operation code of the packet.
		/// </summary>
		[WireMember(1)]
		protected short OperationCode { get; }

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
}
