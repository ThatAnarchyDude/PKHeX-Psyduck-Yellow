// SPDX-License-Identifier: GPL-3.0-or-later
using PKHeX.Core;
namespace PsyduckSaveEditor;

public sealed class SaveSession
{
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
    public void EditParty(int slot, int level, int friendship, int[] ivs, int ability)
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
        pk.Ability = ability;
        pk.IV_HP = ivs[0]; pk.IV_ATK = ivs[1]; pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3]; pk.IV_SPD = ivs[4]; pk.IV_SPE = ivs[5];
        if (statsChanged) pk.ResetPartyStats(); // Recompute party stats and heal after a stat edit.
        Save.SetPartySlotAtIndex(pk, slot, EntityImportSettings.None);
    }
    public int[] AbilityChoices(PKM pk)
    {
        var values = new List<int> { pk.Ability, pk.PersonalInfo.GetAbilityAtIndex(0), pk.PersonalInfo.GetAbilityAtIndex(1) };
        if (PsyduckProfile && HiddenAbility(pk.Species) != 0) values.Add(HiddenAbility(pk.Species));
        return values.Where(x => x > 0).Distinct().ToArray();
    }
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
