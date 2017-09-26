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
			serializer.RegisterType<PSOBBPatchPacketPayloadServer>();
		}

		[Test]
		public void Test_Can_Serialize_Payload()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<PSOBBPatchPacketPayloadServer>();
			byte[] bytes = Enumerable.Repeat((byte)55, 200).ToArray();

			//act
			serializer.Compile();
			UnknownPatchPacket payload = serializer.Deserialize<PSOBBPatchPacketPayloadServer>(bytes)
				as UnknownPatchPacket;

			Assert.True(payload.UnknownBytes.Length == (200 - 2));

			for(int i = 0; i < bytes.Length - 2; i++)
				Assert.AreEqual(bytes[i], payload.UnknownBytes[i]);
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
