using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.Tests
{
	[TestFixture]
	public static class AuthRealmListPacketTest
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

		[Test]
		public static void Test_Can_Deserialize_From_RealWorld_Bytes()
		{
			//arrange
			SerializerService service = new SerializerService();
			service.RegisterType<AuthRealmListResponse>();
			service.Compile();

			//act
			AuthRealmListResponse response = service.Deserialize<AuthRealmListResponse>(realworldBytes);

			//assert
			Assert.True(response.Realms.Count() == 1);
			Assert.True(response.Realms.First().isLocked == false);
			Assert.True(response.Realms.First().Information.Flags == RealmFlags.None);
			Assert.AreEqual(response.Realms.First().Information.RealmString, "Trinity");
			Assert.AreEqual(response.Realms.First().Information.RealmAddress.RealmAddress.Value.ToString(), "127.0.0.1");
		}

		/// <summary>
		/// Response payload contains the realm list.
		/// Response to the request <see cref="AuthRealmListRequest"/>.
		/// </summary>
		[WireDataContract]
		public class AuthRealmListResponse
		{
			/// <summary>
			/// The size of the payload.
			/// </summary>
			[WireMember(1)]
			private ushort payloadSize { get; set; }

			//Unknown field. Trinitycore always sends 0.
			//I think EmberEmu said it's expected as 0 in the client? Can't recall
			[WireMember(2)]
			private uint unknownOne { get; set; }

			/// <summary>
			/// Realm information.
			/// </summary>
			[SendSize(SendSizeAttribute.SizeType.UShort)] //in 2.x and 3.x this is ushort but in 1.12.1 it's a uint32
			[WireMember(3)]
			private RealmInfo[] realms { get; set; }

			/// <summary>
			/// Collection of realm's.
			/// </summary>
			public IEnumerable<RealmInfo> Realms { get { return realms; } }

			//2.x and 3.x clients send byte 16 and 0
			//1.12.1 clients send 0 and 2.
			//EmberEmu has no information on what it is.
			[WireMember(4)]
			private short unknownTwo { get; set; }

			public AuthRealmListResponse()
			{

			}
		}

		[WireDataContract]
		public class DefaultRealmInformation : IRealmInformation
		{
			//TODO: Should we hide this an expose the information in a more OOP way?
			/// <summary>
			/// Packed information about the realm.
			/// </summary>
			[WireMember(1)]
			public RealmFlags Flags { get; private set; }

			/// <summary>
			/// The string the realm should display on the realmlist tab.
			/// This might not be only the name. It could include build information.
			/// </summary>
			[WireMember(2)]
			public string RealmString { get; private set; }

			/// <summary>
			/// Endpoint information for the realm.
			/// </summary>
			[WireMember(3)]
			public RealmEndpoint RealmAddress { get; private set; }

			//Maybe wrap this into something? Query it for realm pop info? I don't know
			//TOOD: Research Mangos and Ember to find out why this is a float.
			//Odd that this is a float.
			[WireMember(4)]
			public float PopulationLevel { get; private set; }

			/// <summary>
			/// Indicates how many character's the account of the client has
			/// on this realm.
			/// </summary>
			[WireMember(5)]
			public byte CharacterCount { get; private set; }

			//TODO: Ok, which time zone maps to which byte?
			[WireMember(6)]
			public byte RealmTimeZone { get; private set; }

			//2.x and 3.x clients expect the realm index.
			//1.12.1 just expect a byte-sized 0
			/// <summary>
			/// Indicates the ID of the realm.
			/// </summary>
			[WireMember(7)]
			public byte RealmId { get; private set; }

			//TODO: If we ever make a server then we should create a real ctor

			public DefaultRealmInformation()
			{

			}
		}

		[WireDataContract]
		public class RealmInfo
		{
			[WireMember(1)]
			public byte RealmType { get; private set; }

			// <summary>
			/// Indidicates if the realm is open.
			/// (Only in 2.x and 3.x according to Trinitycore)
			/// </summary>
			[WireMember(2)]
			public bool isLocked { get; private set; }

			[WireMember(3)]
			public IRealmInformation Information { get; private set; }

			public RealmInfo()
			{

			}
		}

		[WireDataContract]
		public class RealmCompleteInformation : IRealmInformation
		{
			//So, the idea here is to avoid code duplicate we can decorate around the default
			//The beauty of this is it can still deserialize like this. The flags information will
			//still be able to be read when deserializing the default info. This was an unexpected beauty
			//of this implementation
			[WireMember(1)]
			public DefaultRealmInformation DefaultInformation { get; private set; }

			/// <summary>
			/// Contains the build and version information for the realm.
			/// </summary>
			[WireMember(2)]
			public RealmBuildInformation BuildInfo { get; private set; }

			#region Auto Implemented Decoration

			public RealmFlags Flags
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).Flags;
				}
			}

			public string RealmString
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).RealmString;
				}
			}

			public RealmEndpoint RealmAddress
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).RealmAddress;
				}
			}

			public float PopulationLevel
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).PopulationLevel;
				}
			}

			public byte CharacterCount
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).CharacterCount;
				}
			}

			public byte RealmTimeZone
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).RealmTimeZone;
				}
			}

			public byte RealmId
			{
				get
				{
					return ((IRealmInformation)DefaultInformation).RealmId;
				}
			}

			#endregion
		}

		[WireDataContract]
		public class RealmBuildInformation
		{
			[WireMember(1)]
			public ExpansionType Expansion { get; private set; }

			[WireMember(2)]
			public byte MajorVersion { get; private set; }

			[WireMember(3)]
			public byte MinorVersion { get; private set; }

			//TODO: If we ever make a server add a real ctor. Right now only the server sends this

			public RealmBuildInformation()
			{

			}
		}

		/// <summary>
		/// Enumeration of all Expansions for WoW
		/// </summary>
		public enum ExpansionType : byte //uint8 version1;
		{
			/// <summary>
			/// No Expansion
			/// </summary>
			None = 0,

			/// <summary>
			/// 1.x.x (It is helpful to consider Vanilla an expansion upon the base engine)
			/// </summary>
			Vanilla = 1,

			/// <summary>
			/// 2.x.x
			/// </summary>
			TheBurningCrusade = 2,

			/// <summary>
			/// 3.x.x
			/// </summary>
			WrathOfTheLichKing = 3,

			/// <summary>
			/// 4.x.x
			/// </summary>
			Cataclysm = 4,

			/// <summary>
			/// 5.x.x
			/// </summary>
			MistsOfPandaria = 5,

			/// <summary>
			/// 6.x.x
			/// </summary>
			WarlordsOfDraenor = 6,

			/// <summary>
			/// 7.x.x
			/// </summary>
			Legion = 7
		}

		//Based on: https://github.com/EmberEmu/Ember/blob/spark-new/src/libs/shared/shared/Realm.h
		/// <summary>
		/// Contains flag based information about a realm.
		/// </summary>
		public enum RealmFlags : byte
		{
			None = 0x00,
			Invalid = 0x01,
			Offline = 0x02,
			SpecifyBuild = 0x04,
			Unknown1 = 0x08,
			Unknown2 = 0x10,
			Recommended = 0x20, // can set manually or allow client to do so by setting the population to 600.0f
			NewPlayers = 0x40, // can set manually or allow client to do so by setting the population to 200.0f
			Full = 0x80  // can set manually or allow client to do so by setting the population to 400.0f
		}

		/// <summary>
		/// Base contract of information a realm definition contains.
		/// </summary>
		[DefaultNoFlags(typeof(DefaultRealmInformation))]
		[WireDataContractBaseTypeByFlags((int)RealmFlags.SpecifyBuild, typeof(RealmCompleteInformation))]
		[WireDataContract(WireDataContractAttribute.KeyType.Byte, false)] //AuthServer sents byte flags that can be used to determine type information
		public interface IRealmInformation
		{
			/// <summary>
			/// Packed information about the realm.
			/// </summary>
			RealmFlags Flags { get; }

			/// <summary>
			/// The string the realm should display on the realmlist tab.
			/// This might not be only the name. It could include build information.
			/// </summary>
			string RealmString { get; }

			/// <summary>
			/// Endpoint information for the realm.
			/// </summary>
			RealmEndpoint RealmAddress { get; }

			//TOOD: Research Mangos and Ember to find out why this is a float.
			//Odd that this is a float.
			float PopulationLevel { get; }

			/// <summary>
			/// Indicates how many character's the account of the client has
			/// on this realm.
			/// </summary>
			byte CharacterCount { get; }

			//TODO: Ok, which time zone maps to which byte?
			byte RealmTimeZone { get; }

			//2.x and 3.x clients expect the realm index.
			//1.12.1 just expect a byte-sized 0
			/// <summary>
			/// Indicates the ID of the realm.
			/// </summary>
			byte RealmId { get; }

			//Optional data may also be sent but implementers of this interface should deal with it.
		}

		[WireDataContract]
		public class RealmEndpoint
		{
			//We use lazy because we may have like 100 realms and don't need to initialize ALL of their
			//IPAddress and ports. Just waste of CPU cycles.

			//They send IP as a string that contains both IP and the port
			[WireMember(1)]
			private string RealmEndpointInformation { get; set; }

			/// <summary>
			/// <see cref="IPAddress"/> for the realm.
			/// </summary>
			public Lazy<IPAddress> RealmAddress { get; }

			/// <summary>
			/// Port for the realm.
			/// (Usually 8085)
			/// </summary>
			public Lazy<int> Port { get; }

			//TODO: If we make a server make a ctor for this

			public RealmEndpoint()
			{
				RealmAddress = new Lazy<IPAddress>(BuildRealmIP, true);
				Port = new Lazy<int>(BuildRealmPort, true);
			}

			//TODO: Cache split realm info
			private IPAddress BuildRealmIP()
			{
				IPAddress address = null;

				if (IPAddress.TryParse(RealmEndpointInformation.Split(':').First(), out address))
					return address;

				throw new InvalidOperationException($"Failed to generate IPAddress from {RealmEndpointInformation}.");
			}

			private int BuildRealmPort()
			{
				int port = 0;

				if (int.TryParse(RealmEndpointInformation.Split(':').Last(), out port))
					return port;

				throw new InvalidOperationException($"Failed to generate Port from {RealmEndpointInformation}.");
			}
		}
	}
}
