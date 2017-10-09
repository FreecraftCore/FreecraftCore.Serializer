using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;

namespace FreecraftCore.Register.PerfTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//arrange
			Stopwatch watch = new Stopwatch();
			SerializerService serializer = new SerializerService();

			NewMethod(watch, serializer);

			Console.WriteLine($"Register Time: {watch.Elapsed}");
			Console.ReadKey();
		}

		private static void NewMethod(Stopwatch watch, SerializerService serializer)
		{
			watch.Start();
			serializer.RegisterType<PlayerCharacterDataModel>();
			serializer.Compile();
			watch.Stop();
		}
	}

	[WireDataContract]
	public sealed class PlayerCharacterDataModel
	{
		/// <summary>
		/// The progress for the character.
		/// </summary>
		[WireMember(1)]
		public CharacterProgress Progress { get; }

		//TODO: Is this just the guild card as a string?
		/// <summary>
		/// The guild card.
		/// </summary>
		[KnownSize(16)]
		[WireMember(2)]
		public string GuildCard { get; }

		//TODO: What is this?
		[KnownSize(2)]
		[WireMember(3)]
		private byte[] unk3 { get; }

		/// <summary>
		/// The special character data such as name color
		/// or NPC skin.
		/// </summary>
		[WireMember(4)]
		public CharacterSpecialCustomInfo Special { get; }

		/// <summary>
		/// Section ID of the character.
		/// </summary>
		[WireMember(5)]
		public SectionId SectionId { get; }

		/// <summary>
		/// The class/race for the character.
		/// </summary>
		[WireMember(6)]
		public CharacterClassRace ClassRace { get; }

		//TODO: What is this?
		/// <summary>
		/// Character version data.
		/// </summary>
		[WireMember(7)]
		public CharacterVersionData VersionData { get; }

		/// <summary>
		/// Character customization information/data
		/// (Ex. Hair, Costume)
		/// </summary>
		[WireMember(8)]
		public CharacterCustomizationInfo CustomizationInfo { get; }

		/// <summary>
		/// The name of the character.
		/// </summary>
		[Encoding(EncodingType.UTF16)]
		[KnownSize(16)]
		[WireMember(9)]
		public string CharacterName { get; }

		/// <summary>
		/// The amount of time the character has played.
		/// </summary>
		[WireMember(10)]
		public uint PlayedTime { get; }

		//Serializer ctor
		public PlayerCharacterDataModel()
		{

		}
	}

	/// <summary>
	/// PSO doesn't have races and classes. Just a combination of both.
	/// Unlike WoW where you can have a Dwarf Warlock there is only something akin to a DWLock.
	/// </summary>
	public enum CharacterClassRace : byte
	{
		HUmar = 0,

		HUnewearl = 1,

		HUcast = 2,

		HUcaseal = 3,

		RAmar = 4,

		RAmarl = 5,

		RAcast = 6,

		RAcaseal = 7,

		FOmar = 8,

		FOmarl = 9,

		FOnewm = 10,

		FOnewearl = 11
	}

	[WireDataContract]
	public sealed class CharacterCustomizationInfo
	{
		/// <summary>
		/// Id for the character's costume.
		/// </summary>
		[WireMember(1)]
		public ushort CostumeId { get; }

		/// <summary>
		/// Id for the character's skin.
		/// </summary>
		[WireMember(2)]
		public ushort SkinId { get; }

		/// <summary>
		/// Id for the character's face.
		/// </summary>
		[WireMember(3)]
		public ushort FaceId { get; }

		/// <summary>
		/// Id for the character's head.
		/// </summary>
		[WireMember(4)]
		public ushort HeadId { get; }

		/// <summary>
		/// Id for the character's hair.
		/// </summary>
		[WireMember(5)]
		public ushort HairId { get; }

		/// <summary>
		/// The hair color.
		/// </summary>
		[WireMember(6)]
		public HairColorInfo HairColor { get; }

		/// <summary>
		/// The proportions foe the character.
		/// </summary>
		[WireMember(7)]
		public Vector2<float> Proportions { get; }

		//Serializer ctor
		private CharacterCustomizationInfo()
		{

		}
	}

	/// <summary>
	/// Enumeration of all character model types.
	/// (Ex. Regular, Sonic, Tails, Flowen)
	/// </summary>
	public enum CharacterModelType : byte
	{
		/// <summary>
		/// The regular character will be shown.
		/// </summary>
		Regular = 0

		//TODO: Fill in when other NPC ids are known.
	}

	/// <summary>
	/// The model for progress of a character.
	/// </summary>
	[WireDataContract]
	public sealed class CharacterProgress
	{
		/// <summary>
		/// Experience earned by the character.
		/// </summary>
		[WireMember(1)]
		public uint Experience { get; }

		/// <summary>
		/// Level of the character.
		/// </summary>
		[WireMember(2)]
		public uint Level { get; }

		//Serializer ctor
		private CharacterProgress()
		{

		}
	}

	/// <summary>
	/// Represents the information about a character for special configuration
	/// such as name coloring or NPC skins.
	/// </summary>
	[WireDataContract]
	public sealed class CharacterSpecialCustomInfo
	{
		//TODO: Why is this a uint?
		/// <summary>
		/// The color of the name.
		/// </summary>
		[WireMember(1)]
		public uint NameColor { get; }

		/// <summary>
		/// Model type.
		/// (Ex. Regular, Rico, Sonic, Tails)
		/// </summary>
		[WireMember(2)]
		public CharacterModelType ModelType { get; }

		[KnownSize(15)]
		[WireMember(3)]
		private byte[] unused { get; } = new byte[15];

		//TODO: How does this work?
		/// <summary>
		/// The checksum for the name color.
		/// </summary>
		[KnownSize(4)]
		[WireMember(4)]
		public byte[] ColoredNameChecksum { get; }

		//Serializer ctor
		private CharacterSpecialCustomInfo()
		{

		}
	}

	/// <summary>
	/// Currently unknown version data for the character.
	/// </summary>
	[WireDataContract]
	public sealed class CharacterVersionData
	{
		//TODO: What is this?
		[WireMember(1)]
		private byte V2Flags { get; }

		//TODO: What is this?
		[WireMember(2)]
		private byte Version { get; }

		//TODO: What is this?
		[WireMember(3)]
		private uint V1Flags { get; }

		//Serializer ctor.
		private CharacterVersionData()
		{

		}
	}

	/// <summary>
	/// The hair color.
	/// </summary>
	[WireDataContract]
	public sealed class HairColorInfo
	{
		//TODO: Why is the color 2 bytes?
		/// <summary>
		/// The red amount.
		/// </summary>
		[WireMember(1)]
		public ushort RedChannel { get; }

		/// <summary>
		/// The green amount.
		/// </summary>
		[WireMember(2)]
		public ushort GreenChannel { get; }

		/// <summary>
		/// The blue amount.
		/// </summary>
		[WireMember(3)]
		public ushort BlueChannel { get; }

		//Serializer ctor
		private HairColorInfo()
		{

		}
	}

	/// <summary>
	/// Enumeration of all section IDs.
	/// See: http://phantasystar.wikia.com/wiki/Section_ID
	/// </summary>
	public enum SectionId : byte
	{
		Viridia = 0,

		Greenill = 1,

		Skyly = 2,

		Bluefull = 3,

		Purplenum = 4,

		Pinkal = 5,

		Redria = 6,

		Oran = 7,

		Yellowboze = 8,

		Whitill = 9
	}

	/// <summary>
	/// Generic 2-dimensional vector.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[WireDataContract]
	public sealed class Vector2<T>
	{
		/// <summary>
		/// X value.
		/// </summary>
		[WireMember(1)]
		public T X { get; }

		/// <summary>
		/// Y value.
		/// </summary>
		[WireMember(2)]
		public T Y { get; }

		/// <inheritdoc />
		public Vector2(T x, T y)
		{
			X = x;
			Y = y;
		}

		public Vector2()
		{

		}
	}
}
