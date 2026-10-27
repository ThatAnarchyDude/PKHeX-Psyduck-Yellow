using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Brings Pokémon from Generation 5 and later into Psyduck Yellow saves. The game numbers species, moves, abilities and items
/// differently from the standard order, so every ID is converted by name.
/// </summary>
/// <remarks>Only active while a Psyduck Yellow save is loaded; every other game keeps PKHeX's normal rules.</remarks>
public static class PsyduckYellowImport
{
    /// <summary>Set while a Psyduck Yellow save is open.</summary>
    public static bool Enabled { get; set; }

    /// <summary>Keep the origin game the Pokémon came from (true) or stamp it as Psyduck Yellow (false).</summary>
    public static bool KeepOriginGame { get; set; } = true;

    /// <summary>Fallback when the game has no equivalent location: "Mystery Zone".</summary>
    public const ushort FallbackLocation = 0;

    // Names in the game's lists that differ from the standard spelling (standard species ID -> game species ID).
    private static readonly Dictionary<int, int> SpeciesOverrides = new()
    {
        [662] = 712, [740] = 790, [806] = 856, [822] = 872, [823] = 873, [847] = 897, [851] = 901, [855] = 905,
        [874] = 924, [902] = 952, [908] = 958, [931] = 981, [941] = 991, [947] = 997, [982] = 1032, [986] = 1036,
        [987] = 1037, [988] = 1038, [989] = 1039, [993] = 1043, [1005] = 1055, [1006] = 1056, [1009] = 1059,
        [1012] = 1062, [1016] = 1066, [1020] = 1070, [1022] = 1072,
    };

    private static readonly Lazy<Maps> Cache = new(() => new Maps());

    private sealed class Maps
    {
        public readonly int[] Species, Moves, Abilities, Items;

        public Maps()
        {
            var s = GameInfo.GetStrings("en");
            Species = Build(s.specieslist, PsyduckYellowExpanded.SpeciesNames, 1025);
            foreach (var (nat, eng) in SpeciesOverrides)
                Species[nat] = eng;
            Moves = Build(s.movelist, PsyduckYellowExpanded.MoveNames, 919);
            Abilities = Build(s.abilitylist, PsyduckYellowExpanded.AbilityNames, 310);
            Items = Build(s.itemlist, PsyduckYellowExpanded.ItemNames, 2400);
            // Retail Generation 4 IDs are the game's own for the shared range.
            for (int i = 1; i <= 493; i++) Species[i] = i;
            for (int i = 1; i <= 467; i++) Moves[i] = i;
        }

        private static int[] Build(string[] standard, string[] game, int max)
        {
            var result = new int[Math.Min(max, standard.Length - 1) + 1];
            var byName = new Dictionary<string, int>();
            for (int i = 0; i < game.Length; i++)
            {
                var key = Normalize(game[i]);
                if (key.Length != 0)
                    byName.TryAdd(key, i); // first entry = the base form
            }
            for (int i = 1; i < result.Length; i++)
            {
                var key = Normalize(standard[i]);
                if (key.Length != 0 && byName.TryGetValue(key, out var id))
                    result[i] = id;
            }
            return result;
        }

        private static string Normalize(string s)
        {
            Span<char> buf = stackalloc char[s.Length];
            int n = 0;
            foreach (var c in s)
            {
                if (char.IsLetterOrDigit(c))
                    buf[n++] = char.ToLowerInvariant(c);
            }
            return new string(buf[..n]);
        }
    }

    private static int Map(int[] table, int id) => (uint)id < table.Length ? table[id] : 0;

    /// <summary>Converts a Pokémon from a later generation into the game's Pokémon format. Returns null if the species has no equivalent or it is an Egg.</summary>
    public static PK4? TryConvert(PKM src)
    {
        if (src.IsEgg)
            return null;
        var maps = Cache.Value;
        var species = Map(maps.Species, src.Species);
        if (species == 0)
            return null;

        var pk = new PK4 { IsPsyduckYellow0029 = true, IsExpandedEngine = true };
        pk.Species = (ushort)species;
        pk.PID = src.PID;
        pk.TID16 = src.TID16;
        pk.SID16 = src.SID16;
        pk.HeldItem = Map(maps.Items, src.HeldItem);
        pk.CurrentLevel = Math.Clamp(src.CurrentLevel, (byte)1, (byte)100);

        var ability = src.Ability;
        pk.Ability = ability <= 304 ? ability : Map(maps.Abilities, ability);
        if (pk.Ability == 0)
            pk.Ability = Map(maps.Abilities, ability);
        pk.AbilityNumber = src.AbilityNumber is 4 ? 4 : 1;
        pk.StatAlignment = src.StatAlignment;
        pk.Gender = src.Gender;
        pk.OriginalTrainerFriendship = src.OriginalTrainerFriendship;
        pk.Language = src.Language is >= 1 and <= 7 ? src.Language : (int)LanguageID.English;
        pk.OriginalTrainerGender = src.OriginalTrainerGender;
        pk.OriginalTrainerName = src.OriginalTrainerName;
        pk.IsNicknamed = src.IsNicknamed;
        pk.Nickname = src.IsNicknamed ? src.Nickname : PsyduckYellowExpanded.SpeciesNames[species];

        pk.IV_HP = src.IV_HP; pk.IV_ATK = src.IV_ATK; pk.IV_DEF = src.IV_DEF;
        pk.IV_SPE = src.IV_SPE; pk.IV_SPA = src.IV_SPA; pk.IV_SPD = src.IV_SPD;
        pk.EV_HP = Math.Min(src.EV_HP, 255); pk.EV_ATK = Math.Min(src.EV_ATK, 255); pk.EV_DEF = Math.Min(src.EV_DEF, 255);
        pk.EV_SPE = Math.Min(src.EV_SPE, 255); pk.EV_SPA = Math.Min(src.EV_SPA, 255); pk.EV_SPD = Math.Min(src.EV_SPD, 255);
        if (src is IHyperTrain ht)
        {
            pk.HT_HP = ht.HT_HP; pk.HT_ATK = ht.HT_ATK; pk.HT_DEF = ht.HT_DEF;
            pk.HT_SPE = ht.HT_SPE; pk.HT_SPA = ht.HT_SPA; pk.HT_SPD = ht.HT_SPD;
        }

        // Moves with no equivalent (Z-Moves, G-Max Moves) are dropped.
        Span<ushort> moves = stackalloc ushort[4];
        Span<byte> ups = stackalloc byte[4];
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            var move = Map(maps.Moves, src.GetMove(i));
            if (move == 0)
                continue;
            moves[count] = (ushort)move;
            ups[count++] = GetPPUps(src, i);
        }
        for (int i = 0; i < 4; i++)
        {
            pk.SetMove(i, i < count ? moves[i] : (ushort)0);
            SetPPUps(pk, i, i < count ? ups[i] : (byte)0);
        }

        pk.Ball = src.Ball is >= 1 and <= 24 ? src.Ball : (byte)Core.Ball.Poke;
        pk.Version = KeepOriginGame && src.Version != GameVersion.Invalid ? src.Version : GameVersion.PY;
        pk.MetLocation = FallbackLocation;
        pk.MetLevel = Math.Clamp(src.MetLevel == 0 ? src.CurrentLevel : src.MetLevel, (byte)1, (byte)100);
        pk.MetDate = DateOnly.FromDateTime(DateTime.Today);
        pk.IsPsyduckYellow0029 = true;
        pk.IsExpandedEngine = true;
        pk.HealPP();
        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    private static void SetPPUps(PKM pk, int index, byte value)
    {
        switch (index)
        {
            case 0: pk.Move1_PPUps = value; break;
            case 1: pk.Move2_PPUps = value; break;
            case 2: pk.Move3_PPUps = value; break;
            default: pk.Move4_PPUps = value; break;
        }
    }

    private static byte GetPPUps(PKM src, int index) => index switch { 0 => (byte)src.Move1_PPUps, 1 => (byte)src.Move2_PPUps, 2 => (byte)src.Move3_PPUps, _ => (byte)src.Move4_PPUps };
}
