using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace FreecraftCore
{
	[WireDataContract]
	public sealed class InitialSpellDataBlock<TSpellType>
		where TSpellType : struct
	{
		//TODO:W hat is this?
		//TC always sets this to 0.
		/// <summary>
		/// UNK
		/// </summary>
		[WireMember(1)]
		public byte UnkByte { get; internal set; } = 0;

		//TODO: Hide behind readonly collection.
		//TC sends this as a short prefixed array
		/// <summary>
		/// Array of spell ids sent as the inital spells
		/// </summary>
		[SendSize(PrimitiveSizeType.UInt16)]
		[WireMember(2)]
		public InitialSpellData<TSpellType>[] SpellIds { get; internal set; }

		/// <summary>
		/// Spell cooldowns.
		/// </summary>
		[SendSize(PrimitiveSizeType.UInt16)]
		[WireMember(3)]
		public InitialSpellCooldown<TSpellType>[] SpellCooldowns { get; internal set; }

		//TODO: Can cooldown be null?
		/// <inheritdoc />
		public InitialSpellDataBlock([NotNull] InitialSpellData<TSpellType>[] spellIds, InitialSpellCooldown<TSpellType>[] spellCooldowns)
		{
			SpellIds = spellIds ?? throw new ArgumentNullException(nameof(spellIds));
			SpellCooldowns = spellCooldowns;
		}

		public InitialSpellDataBlock([NotNull] InitialSpellData<TSpellType>[] spellIds)
		{
			SpellIds = spellIds ?? throw new ArgumentNullException(nameof(spellIds));
			SpellCooldowns = Array.Empty<InitialSpellCooldown<TSpellType>>();
		}

		public InitialSpellDataBlock()
		{

		}
	}

	[WireDataContract]
	public sealed class InitialSpellData<TSpellIdType>
		where TSpellIdType : struct
	{
		/// <summary>
		/// The spell id of the data.
		/// </summary>
		[WireMember(1)]
		public TSpellIdType SpellId { get; internal set; }

		/// <summary>
		/// UNK data
		/// </summary>
		[WireMember(2)]
		public short UnkShort { get; internal set; }

		/// <inheritdoc />
		public InitialSpellData(TSpellIdType spellId, short unkShort)
		{
			SpellId = spellId;
			UnkShort = unkShort;
		}

		public InitialSpellData()
		{

		}
	}

	[WireDataContract]
	public sealed class InitialSpellCooldown<TSpellIdType>
		where TSpellIdType : struct
	{
		[WireMember(1)]
		public TSpellIdType SpellId { get; internal set; }

		[WireMember(2)]
		public short ItemId { get; internal set; }

		[WireMember(3)]
		public short CategoryId { get; internal set; }

		[WireMember(4)]
		public uint SpellCooldown { get; internal set; }

		[WireMember(5)]
		public uint CategoryCooldown { get; internal set; }

		public bool IsInfiteCooldown => SpellCooldown == 1 && CategoryCooldown == 0x80000000;

		/// <inheritdoc />
		public InitialSpellCooldown(TSpellIdType spellId, short itemId, short categoryId, uint spellCooldown, uint categoryCooldown)
		{
			SpellId = spellId;
			ItemId = itemId;
			CategoryId = categoryId;
			SpellCooldown = spellCooldown;
			CategoryCooldown = categoryCooldown;
		}

		public InitialSpellCooldown()
		{

		}
	}

	[WireDataContract]
	[WireMessageType]
	public sealed partial class SMSG_INITIAL_SPELLS_Payload
	{
		/// <summary>
		/// The initial spell data and cooldown data.
		/// Uses int spell ids in 3.3.5.
		/// </summary>
		[WireMember(1)]
		public InitialSpellDataBlock<int> Data { get; internal set; }

		/// <inheritdoc />
		public SMSG_INITIAL_SPELLS_Payload()
		{

		}
	}
}
