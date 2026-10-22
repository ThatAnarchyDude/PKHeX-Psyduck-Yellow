using System;
namespace PKHeX.Core;

/// <summary>Opt-in, non-serialized rules for the released Psyduck Yellow 0029 ROM.</summary>
public static partial class PsyduckYellow0029
{
    public const string Name = "Psyduck Yellow prototype 0029";
    public const GameVersion ProfileVersion = GameVersion.PY;
    public static readonly PersonalTable4 Personal = CreatePersonalTable();
    private static PersonalTable4 CreatePersonalTable()
    {
        var original = PersonalTable.HGSS;
        var bytes = new byte[original.Count * PersonalInfo4.SIZE];
        for (int i = 0; i < original.Count; i++)
            original[i].Write().CopyTo(bytes, i * PersonalInfo4.SIZE);
        var table = new PersonalTable4(bytes);
        table[55].Type2 = (byte)MoveType.Psychic; // Verified 0029 Golduck: Water/Psychic.
        return table;
    }
    public static int GetHiddenAbility(ushort species) => species switch
    {
        54 or 55 => 33, 133 => 107, 134 => 93, 135 => 95, 136 => 62,
        197 => 39, 470 => 34, 471 => 115, 175 or 176 or 468 => 105, _ => 0,
    };
    public static bool IsEnabled(PKM pk) => pk is PK4 { IsPsyduckYellow0029: true, IsExpandedEngine: false };
    public static string[] GetItemNames(string[] original)
    {
        var names = (string[])original.Clone();
        names[114] = "Shiny Charm";
        names[121] = "Friendship Charm";
        names[115] = "TM34 Bide";
        names[116] = "TM12 Water Gun";
        names[117] = "TM01 Mega Punch";
        names[118] = "TM04 Whirlwind";
        names[119] = "TM11 BubbleBeam";
        names[120] = "TM45 Thunder Wave";
        names[123] = "TM19 Seismic Toss";
        return names;
    }
    public static readonly EncounterStatic4[] Starters =
    [
        CreateStarter(54), CreateStarter(133), CreateStarter(175),
    ];
    private static EncounterStatic4 CreateStarter(ushort species) => new(GameVersion.HG)
    {
        Species = species, Level = 5, Location = 138, FixedBall = Ball.Poke,
        Gender = species switch { 54 => 0, 133 => 1, _ => FixedGenderUtil.GenderRandom },
        GroundTile = GroundTileAllowed.Building | GroundTileAllowed.Max_Pt, IsPsyduckYellowStarter = true, // gifted indoors; older builds wrote "Unspecific"
    };
}
