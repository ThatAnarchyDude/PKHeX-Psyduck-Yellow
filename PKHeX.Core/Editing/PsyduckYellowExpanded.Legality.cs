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
    public const ushort Novareonite = 3147;
    /// <summary>Regular (non-starter) Novareon species slot.</summary>
    public const ushort Novareon = 500;

    /// <summary>First and last of the starter Eevee line's species: for each Eeveelution a starter species followed by its Mega copy; the last pair is the Novareon stand-in.</summary>
    public const ushort StarterLineFirst = 1565;
    public const ushort StarterLineLast = 1582;
    private const ushort NovareonFirst = 1581;
    /// <summary>Ordinary internal species behind each starter-line pair (Vaporeon, Jolteon, Flareon, Espeon, Umbreon, Leafeon, Glaceon, Sylveon, then Umbreon for Novareon).</summary>
    private static readonly ushort[] LineBases = [134, 135, 136, 196, 197, 470, 471, 750, 197];

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

    /// <summary>Move types in PKHeX's numbering (Normal..Steel 0-8, Fire 9 .. Dark 16, Fairy 17), as used by the type icons and names.</summary>
    private static readonly Lazy<byte[]> MoveTypeData = new(() => Array.ConvertAll(MoveTypesEngine, ToDisplayType));
    public static byte[] MoveTypes => MoveTypeData.Value;

    /// <summary>Converts a game-stored type (Fairy in slot 9, Fire at 10 .. Dark at 17) to PKHeX's numbering.</summary>
    public static byte ToDisplayType(byte engineType) => engineType switch
    {
        9 => 17,
        > 9 and <= 17 => (byte)(engineType - 1),
        _ => engineType,
    };

    /// <summary>
    /// Fills <paramref name="moves"/> with the last four level-up moves the game teaches the Pokémon's species by its current level.
    /// Used when legality cannot suggest moves (the Pokémon is outside the retail catalogs or matches no known encounter).
    /// </summary>
    public static bool TryGetSuggestedMoves(PK4 pk, Span<ushort> moves)
    {
        var learnsets = Learnsets;
        if (pk.Species >= learnsets.Length)
            return false;
        moves.Clear();
        learnsets[pk.Species].SetEncounterMoves((byte)Math.Clamp((int)pk.CurrentLevel, 1, 100), moves);
        return moves[0] != 0;
    }

    /// <summary>Species names with the project's starter/Mega copies labelled, and unused placeholder slots hidden.</summary>
    public static string[] GetSpeciesDisplayNames()
    {
        var names = (string[])SpeciesNames.Clone();
        names[StarterEevee] = "Eevee (Starter)";
        names[MegaEevee] = "Eevee (Mega)";
        names[StarterPsyduck] = "Psyduck (Starter)";
        names[MegaPsyduck] = "Psyduck (Mega)";
        for (int i = 0; i < StarterLineLast - StarterLineFirst + 1; i++)
        {
            var species = StarterLineFirst + i;
            var name = species >= NovareonFirst ? "Novareon" : names[LineBases[i / 2]];
            names[species] = name + (i % 2 == 0 ? " (Starter)" : " (Mega)");
        }
        return names;
    }

    /// <summary>
    /// Campaign Pokémon only use Kanto encounters, so Pokémon whose species, moves, ability and item fit the retail Generation 4 catalogs
    /// are checked with HeartGold's rules plus the Psyduck Yellow starter rules. Returns false when the Pokémon uses anything beyond that catalog.
    /// </summary>
    /// <summary>The ordinary species that legality (and the encounter search) uses for a starter/Mega copy or the regular Novareon.</summary>
    public static ushort GetRetailSpecies(ushort species) => species switch
    {
        StarterEevee or MegaEevee => (ushort)Species.Eevee,
        StarterPsyduck or MegaPsyduck => (ushort)Species.Psyduck,
        >= StarterLineFirst and <= StarterLineLast => LineBases[(species - StarterLineFirst) / 2],
        Novareon => (ushort)Species.Umbreon, // the regular Novareon (Eevee + Sandwich), stat-wise an Umbreon stand-in
        _ => species,
    };

    /// <summary>True for the starter Eevee/Psyduck, their Eeveelutions and Mega copies: these exist only as the starter gift line, identified by their own species.</summary>
    public static bool IsStarterSpecies(ushort species) => species is StarterEevee or MegaEevee or StarterPsyduck or MegaPsyduck or (>= StarterLineFirst and <= StarterLineLast);

    public static bool TryGetRetailView(PK4 pk, out PK4 view)
    {
        view = pk;
        var species = GetRetailSpecies(pk.Species);
        if (species is 0 or > Legal.MaxSpeciesID_4)
            return false;
        if (pk.Move1 > Legal.MaxMoveID_4 || pk.Move2 > Legal.MaxMoveID_4 || pk.Move3 > Legal.MaxMoveID_4 || pk.Move4 > Legal.MaxMoveID_4)
            return false;
        // Psyduckite belongs to the starter Psyduck only; Eonite to the Eevee family (starter copies included) and the starter line's Eeveelutions;
        // Novareonite to the starter Novareon stand-in only.
        var megaStone = (pk.HeldItem == Psyduckite && pk.Species is StarterPsyduck or MegaPsyduck)
                     || (pk.HeldItem == Eonite && pk.Species is >= StarterLineFirst and < NovareonFirst)
                     || (pk.HeldItem == Novareonite && pk.Species is >= NovareonFirst and <= StarterLineLast)
                     || (pk.HeldItem == Eonite && (pk.Species is StarterEevee or MegaEevee || species is (ushort)Species.Eevee or (ushort)Species.Vaporeon or (ushort)Species.Jolteon or (ushort)Species.Flareon or (ushort)Species.Espeon or (ushort)Species.Umbreon or (ushort)Species.Leafeon or (ushort)Species.Glaceon));
        var isStone = pk.HeldItem is Eonite or Psyduckite or Novareonite;
        if (pk.Ability > Legal.MaxAbilityID_4 || (pk.HeldItem > Legal.MaxItemID_4_HGSS && !isStone))
            return false;

        var clone = new PK4(pk.Data.ToArray()) { IsPsyduckYellow0029 = true, IsExpandedEngine = false, IsExpandedLegalityView = true, ExpandedSpecies = pk.Species };
        // Expanded-engine extras share bytes with retail fields: ability bit 8 sits in EXP bit 31, and flags occupy the shiny-leaf byte.
        var exp = ReadUInt32LittleEndian(clone.Data[0x10..]) & 0x1FFFFFu;
        WriteUInt32LittleEndian(clone.Data[0x10..], exp);
        clone.Data[0x41] &= 0x3F;
        clone.Species = species;
        if (isStone) // no retail equivalent: the right holder is unaffected, anyone else gets an item that can't be held (Bicycle)
            clone.HeldItem = megaStone ? (ushort)0 : (ushort)450;
        // The expanded game writes species names in mixed case; retail Generation 4 stores them in upper case.
        var expectedName = SpeciesName.GetSpeciesNameGeneration(species, clone.Language, 4);
        var ownName = pk.Species < SpeciesNames.Length ? SpeciesNames[pk.Species] : expectedName; // e.g. "Novareon" on its Umbreon stand-in
        if (!clone.IsNicknamed && (string.Equals(clone.Nickname, expectedName, StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(clone.Nickname, ownName, StringComparison.OrdinalIgnoreCase)))
            clone.Nickname = expectedName;
        if (clone.Version == GameVersion.PY)
            clone.Version = GameVersion.HG;
        view = clone;
        return true;
    }
}
