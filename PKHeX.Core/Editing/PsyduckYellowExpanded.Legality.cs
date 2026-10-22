using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static partial class PsyduckYellowExpanded
{
    /// <summary>Starter Eevee copy; legality treats it as the base Eevee.</summary>
    public const ushort StarterEevee = 1561;
    /// <summary>Mega-capable Eevee copy; legality treats it as the base Eevee.</summary>
    public const ushort MegaEevee = 1562;
    /// <summary>Starter Psyduck copy (never evolves); legality treats it as the base Psyduck.</summary>
    public const ushort StarterPsyduck = 1563;
    /// <summary>Mega-capable Psyduck copy; legality treats it as the base Psyduck.</summary>
    public const ushort MegaPsyduck = 1564;
    public const ushort Eonite = 3145;
    public const ushort Psyduckite = 3146;

    private static readonly Lazy<Learnset[]> LearnsetData = new(LoadLearnsets);
    /// <summary>Level-up learnsets read from the expanded game's ROM, indexed by internal species.</summary>
    public static Learnset[] Learnsets => LearnsetData.Value;

    private static Learnset[] LoadLearnsets()
    {
        // u16 species count; per species: u8 n, n * u16 move, n * u8 level
        var data = Util.GetBinaryResource("learnset_psyduck_expanded");
        var count = ReadUInt16LittleEndian(data);
        var result = new Learnset[count];
        int pos = 2;
        for (int i = 0; i < count; i++)
        {
            int n = data[pos++];
            var moves = new ushort[n];
            for (int m = 0; m < n; m++, pos += 2)
                moves[m] = ReadUInt16LittleEndian(data.AsSpan(pos));
            var levels = data.AsSpan(pos, n).ToArray();
            pos += n;
            result[i] = new Learnset(moves, levels);
        }
        return result;
    }

    /// <summary>Species names with the project's starter/Mega copies labelled, and unused placeholder slots hidden.</summary>
    public static string[] GetSpeciesDisplayNames()
    {
        var names = (string[])SpeciesNames.Clone();
        names[StarterEevee] = "Eevee (Starter)";
        names[MegaEevee] = "Eevee (Mega)";
        names[StarterPsyduck] = "Psyduck (Starter)";
        names[MegaPsyduck] = "Psyduck (Mega)";
        return names;
    }

    /// <summary>
    /// Campaign Pokémon only use Kanto encounters, so Pokémon whose species, moves, ability and item fit the retail Generation 4 catalogs
    /// are checked with HeartGold's rules plus the Psyduck Yellow starter rules. Returns false when the Pokémon uses anything beyond that catalog.
    /// </summary>
    public static bool TryGetRetailView(PK4 pk, out PK4 view)
    {
        view = pk;
        var species = pk.Species switch
        {
            StarterEevee or MegaEevee => (ushort)Species.Eevee,
            StarterPsyduck or MegaPsyduck => (ushort)Species.Psyduck,
            _ => pk.Species,
        };
        if (species is 0 or > Legal.MaxSpeciesID_4)
            return false;
        if (pk.Move1 > Legal.MaxMoveID_4 || pk.Move2 > Legal.MaxMoveID_4 || pk.Move3 > Legal.MaxMoveID_4 || pk.Move4 > Legal.MaxMoveID_4)
            return false;
        // Psyduckite belongs to the starter Psyduck only; Eonite to the Eevee family (starter copies included).
        var megaStone = (pk.HeldItem == Psyduckite && pk.Species is StarterPsyduck or MegaPsyduck)
                     || (pk.HeldItem == Eonite && (pk.Species is StarterEevee or MegaEevee || species is (ushort)Species.Eevee or (ushort)Species.Vaporeon or (ushort)Species.Jolteon or (ushort)Species.Flareon or (ushort)Species.Espeon or (ushort)Species.Umbreon or (ushort)Species.Leafeon or (ushort)Species.Glaceon));
        var isStone = pk.HeldItem is Eonite or Psyduckite;
        if (pk.Ability > Legal.MaxAbilityID_4 || (pk.HeldItem > Legal.MaxItemID_4_HGSS && !isStone))
            return false;

        var clone = new PK4(pk.Data.ToArray()) { IsPsyduckYellow0029 = true, IsExpandedEngine = false, IsExpandedLegalityView = true };
        // Expanded-engine extras share bytes with retail fields: ability bit 8 sits in EXP bit 31, and flags occupy the shiny-leaf byte.
        var exp = ReadUInt32LittleEndian(clone.Data[0x10..]) & 0x1FFFFFu;
        WriteUInt32LittleEndian(clone.Data[0x10..], exp);
        clone.Data[0x41] &= 0x3F;
        clone.Species = species;
        if (isStone) // no retail equivalent: the right holder is unaffected, anyone else gets an item that can't be held (Bicycle)
            clone.HeldItem = megaStone ? (ushort)0 : (ushort)450;
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
