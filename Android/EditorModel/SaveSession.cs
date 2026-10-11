// SPDX-License-Identifier: GPL-3.0-or-later
using PKHeX.Core;
namespace PsyduckSaveEditor;

public sealed class SaveSession
{
    public static readonly int[] HeldItems = new[] { InventoryType.Items, InventoryType.Medicine, InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems, InventoryType.TMHMs }
        .SelectMany(type => ItemStorage4HGSS.Instance.GetItems(type).ToArray().Select(x => (int)x))
        .Where(x => x < 420 || x > 427).Append(0).Distinct().Order().ToArray();
    public SAV4HGSS Save { get; }
    public bool PsyduckProfile { get; set; }
    public SaveSession(byte[] bytes, string? name = null)
    {
        if (bytes.Length is not (524288 or 524410))
            throw new InvalidDataException("Open a normal 512 KB .sav or DeSmuME .dsv save. Emulator save states and ROM files are not supported.");
        Save = SaveUtil.GetSaveFile(bytes.ToArray(), name) as SAV4HGSS
            ?? throw new InvalidDataException("This file is not a HeartGold/SoulSilver save.");
        if (!Save.ChecksumsValid) throw new InvalidDataException("The save checksum is invalid. Save normally in-game and try that file.");
    }
    public PKM GetParty(int slot)
    {
        if ((uint)slot >= Save.PartyCount) throw new ArgumentOutOfRangeException(nameof(slot));
        return Save.GetPartySlotAtIndex(slot).Clone();
    }
    public void EditParty(int slot, int level, int friendship, int[] ivs, int ability, int? heldItem = null)
    {
        var pk = GetParty(slot);
        if (pk.IsEgg) throw new InvalidOperationException("Egg editing is not supported in this first version.");
        if (level is < 1 or > 100 || friendship is < 0 or > 255 || ivs.Length != 6 || ivs.Any(x => x is < 0 or > 31))
            throw new ArgumentException("Level must be 1–100, friendship 0–255, and each IV 0–31.");
        int[] before = [pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE];
        bool statsChanged = level != pk.CurrentLevel || !before.SequenceEqual(ivs);
        if (level != pk.CurrentLevel) pk.CurrentLevel = (byte)level;
        pk.CurrentFriendship = (byte)friendship;
        if (ability != pk.Ability && !AbilityChoices(pk).Contains(ability))
            throw new ArgumentException("This ability is not available for the selected species and profile.");
        if (heldItem is int item && item != pk.HeldItem && !HeldItems.Contains(item))
            throw new ArgumentException("Choose a supported HeartGold held item. Key items, HMs and mail cannot be assigned here.");
        pk.Ability = ability;
        if (heldItem is int nextItem) pk.HeldItem = nextItem;
        pk.IV_HP = ivs[0]; pk.IV_ATK = ivs[1]; pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3]; pk.IV_SPD = ivs[4]; pk.IV_SPE = ivs[5];
        if (statsChanged) pk.ResetPartyStats(); // Recompute party stats and heal after a stat edit.
        Save.SetPartySlotAtIndex(pk, slot, EntityImportSettings.None);
    }
    public int AddPokemon(ushort species, int level, byte? gender = null, int nature = 0, bool shiny = false)
    {
        if (Save.PartyCount >= 6) throw new InvalidOperationException("Your party already has six Pokémon. Remove one before adding another.");
        if (species is < 1 or > 493 || level is < 1 or > 100 || nature is < 0 or > 24)
            throw new ArgumentException("Choose species 1–493, level 1–100 and a valid nature.");
        var info = PersonalTable.HGSS[species];
        byte selectedGender = gender ?? (info.Gender == 255 ? (byte)2 : info.Gender == 254 ? (byte)1 : (byte)0);
        bool possible = info.Gender switch { 255 => selectedGender == 2, 254 => selectedGender == 1, 0 => selectedGender == 0, _ => selectedGender < 2 };
        if (!possible) throw new ArgumentException("That gender is not available for this species.");
        var pk = new PK4 { Species = species, Version = GameVersion.HG, Language = Save.Language,
            ID32 = Save.ID32, OriginalTrainerName = Save.OT, OriginalTrainerGender = Save.Gender,
            Gender = selectedGender, CurrentLevel = (byte)level, MetLevel = (byte)level,
            MetLocation = 0, MetDate = DateOnly.FromDateTime(DateTime.Today), Ball = 4,
            CurrentFriendship = info.BaseFriendship };
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(species, pk.Language, 4);
        bool matched = false;
        for (int attempt = 0; attempt < 100000; attempt++)
        {
            uint pid = (uint)Random.Shared.NextInt64(0, (long)uint.MaxValue + 1);
            if (shiny) pid = (pid & 0xFFFF) | (((Save.ID32 & 0xFFFF) ^ (Save.ID32 >> 16) ^ (pid & 0xFFFF)) << 16);
            pk.PID = pid;
            if ((int)pk.Nature == nature && EntityGender.GetFromPIDAndRatio(pid, info.Gender) == selectedGender && IsShiny(pk) == shiny)
            { matched = true; break; }
        }
        if (!matched) throw new InvalidOperationException("Could not generate that combination. Please try again.");
        pk.Ability = info.GetAbilityAtIndex(pk.PIDAbility);
        pk.IV_HP = Random.Shared.Next(32); pk.IV_ATK = Random.Shared.Next(32); pk.IV_DEF = Random.Shared.Next(32);
        pk.IV_SPA = Random.Shared.Next(32); pk.IV_SPD = Random.Shared.Next(32); pk.IV_SPE = Random.Shared.Next(32);
        Span<ushort> moves = stackalloc ushort[4]; moves.Clear();
        LearnSource4HGSS.Instance.GetLearnset(species, 0).SetEncounterMoves((byte)level, moves);
        if (moves[0] == 0) moves[0] = 33;
        pk.SetMoves(moves); pk.ResetPartyStats(); pk.RefreshChecksum();
        int slot = Save.PartyCount;
        Save.SetPartySlotAtIndex(pk, slot, EntityImportSettings.None);
        return slot;
    }
    public void RemovePokemon(int slot)
    {
        _ = GetParty(slot);
        if (Save.PartyCount <= 1) throw new InvalidOperationException("Keep at least one Pokémon in a started game's party.");
        Save.DeletePartySlot(slot);
    }
    public void EditMoves(int slot, ushort[] moves)
    {
        var pk = GetParty(slot);
        if (pk.IsEgg) throw new InvalidOperationException("Egg editing is not supported.");
        if (moves.Length != 4 || moves.Any(x => x > 467) || moves.All(x => x == 0) || moves.Where(x => x != 0).Distinct().Count() != moves.Count(x => x != 0))
            throw new ArgumentException("Choose up to four different Generation IV moves, with at least one nonempty move.");
        var packed = moves.Where(x => x != 0).Concat(Enumerable.Repeat((ushort)0, 4)).Take(4).ToArray();
        pk.Move1_PPUps = pk.Move2_PPUps = pk.Move3_PPUps = pk.Move4_PPUps = 0;
        pk.SetMoves(packed);
        Save.SetPartySlotAtIndex(pk, slot, EntityImportSettings.None);
    }
    public int[] AbilityChoices(PKM pk)
    {
        var values = new List<int> { pk.Ability, pk.PersonalInfo.GetAbilityAtIndex(0), pk.PersonalInfo.GetAbilityAtIndex(1) };
        if (PsyduckProfile && HiddenAbility(pk.Species) != 0) values.Add(HiddenAbility(pk.Species));
        return values.Where(x => x > 0).Distinct().ToArray();
    }
    public int[] HeldItemChoices(PKM pk) => HeldItems.Append(pk.HeldItem).Distinct().Order().ToArray();
    public bool IsShiny(PKM pk) => pk.ShinyXor < (PsyduckProfile ? 16 : 8);
    public byte[] Export()
    {
        var bytes = Save.Write().ToArray();
        var reopened = SaveUtil.GetSaveFile(bytes) as SAV4HGSS;
        if (reopened is null || !reopened.ChecksumsValid) throw new InvalidDataException("Export verification failed.");
        return bytes;
    }
    public static int HiddenAbility(ushort species) => species switch
    {
        54 or 55 => 33, 133 => 107, 134 => 93, 135 => 95, 136 => 62,
        197 => 39, 470 => 34, 471 => 115, 175 or 176 or 468 => 105, _ => 0
    };
}
