using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static partial class PsyduckYellowExpanded
{
    /// <summary>Starter Eevee copy; legality treats it as the base Eevee.</summary>
    public const ushort StarterEevee = 1561;
    /// <summary>Mega-capable Eevee copy; legality treats it as the base Eevee.</summary>
    public const ushort MegaEevee = 1562;

    /// <summary>
    /// Campaign Pokémon only use Kanto encounters, so Pokémon whose species, moves, ability and item fit the retail Generation 4 catalogs
    /// are checked with HeartGold's rules plus the Psyduck Yellow starter rules. Returns false when the Pokémon uses anything beyond that catalog.
    /// </summary>
    public static bool TryGetRetailView(PK4 pk, out PK4 view)
    {
        view = pk;
        var species = pk.Species is StarterEevee or MegaEevee ? (ushort)Species.Eevee : pk.Species;
        if (species is 0 or > Legal.MaxSpeciesID_4)
            return false;
        if (pk.Move1 > Legal.MaxMoveID_4 || pk.Move2 > Legal.MaxMoveID_4 || pk.Move3 > Legal.MaxMoveID_4 || pk.Move4 > Legal.MaxMoveID_4)
            return false;
        if (pk.Ability > Legal.MaxAbilityID_4 || pk.HeldItem > Legal.MaxItemID_4_HGSS)
            return false;

        var clone = new PK4(pk.Data.ToArray()) { IsPsyduckYellow0029 = true, IsExpandedEngine = false };
        // Expanded-engine extras share bytes with retail fields: ability bit 8 sits in EXP bit 31, and flags occupy the shiny-leaf byte.
        var exp = ReadUInt32LittleEndian(clone.Data[0x10..]) & 0x1FFFFFu;
        WriteUInt32LittleEndian(clone.Data[0x10..], exp);
        clone.Data[0x41] &= 0x3F;
        clone.Species = species;
        // The expanded game writes species names in mixed case; retail Generation 4 stores them in upper case.
        var expectedName = SpeciesName.GetSpeciesNameGeneration(species, clone.Language, 4);
        if (!clone.IsNicknamed && string.Equals(clone.Nickname, expectedName, StringComparison.OrdinalIgnoreCase))
            clone.Nickname = expectedName;
        if (clone.Version == GameVersion.PY)
            clone.Version = GameVersion.HG;
        view = clone;
        return true;
    }
}
