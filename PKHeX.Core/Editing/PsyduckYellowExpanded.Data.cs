using System;
using System.Collections.Generic;
namespace PKHeX.Core;
public static partial class PsyduckYellowExpanded
{
    private static readonly Lazy<PersonalTable4> PersonalData = new(CreatePersonalTable);
    public static PersonalTable4 Personal => PersonalData.Value;
    public static ushort MaxSpecies => (ushort)(SpeciesNames.Length - 1);
    public static ushort MaxMove => (ushort)(MoveNames.Length - 1);
    public static ushort MaxAbility => (ushort)(AbilityNames.Length - 1);
    public static ushort MaxItem => (ushort)(ItemNames.Length - 1);
    private static readonly Lazy<ushort[]> HeldItemData = new(GetHeldItems);
    public static ReadOnlySpan<ushort> HeldItems => HeldItemData.Value;
    private static ushort[] GetHeldItems()
    {
        var result = new SortedSet<ushort>();
        for (int i = 0; i < 7; i++) foreach (var id in ItemPockets[i]) result.Add(id);
        return [.. result];
    }
    private static PersonalTable4 CreatePersonalTable()
    {
        var data = Util.GetBinaryResource("personal_psyduck_expanded");
        var table = new PersonalTable4(data, (ushort)(data.Length / PersonalInfo4.SIZE - 1), FormMap);
        for (int i = 0; i < table.Count; i++)
        {
            var p = table[i]; p.EngineAbility1 = Ability1[i]; p.EngineAbility2 = Ability2[i]; p.EngineHiddenAbility = HiddenAbility[i];
        }
        return table;
    }
    public static int GetFormCount(ushort species)
    {
        int count = 1;
        for (int f = 1; f <= 31; f++) { int at = species * 32 + f - 1; if ((uint)at < FormMap.Length && FormMap[at] is > 0 and var index && index < SpeciesNames.Length) count = f + 1; }
        return count;
    }
    public static string[] GetFormNames(ushort species)
    {
        var names = new string[GetFormCount(species)]; names[0] = "Base";
        for (int f = 1; f < names.Length; f++) { var index = FormMap[species * 32 + f - 1]; names[f] = index > 0 && index < SpeciesNames.Length ? $"{f}: {SpeciesNames[index]}" : $"Form {f}"; }
        return names;
    }
    /// <summary>Species of the project without a national sprite: their pictures are b_py{index} / a_py{index} (see CustomSpriteBase).</summary>
    public const ushort CustomSpriteBase = 20000;
    public static readonly string[] CustomSpriteNames = ["Brawleon", "Venomeon", "Duneon", "Zephyreon", "Chryseon", "Strateon", "Wraitheon", "Wyrmeon", "Ferreon", "Stellareon"];

    public static ushort GetDisplaySpecies(ushort species)
    {
        if (species <= 493) return species;
        if (species >= SpeciesNames.Length) return 0;
        var custom = Array.FindIndex(CustomSpriteNames, n => string.Equals(n, SpeciesNames[species], StringComparison.OrdinalIgnoreCase));
        if (custom >= 0) return (ushort)(CustomSpriteBase + custom);
        var national = GameInfo.GetStrings("en").specieslist;
        for (ushort i = 1; i < national.Length; i++)
            if (string.Equals(national[i], SpeciesNames[species], StringComparison.OrdinalIgnoreCase)) return i;
        return 0; // Unknown engine forms use the neutral sprite rather than an unrelated Pokemon.
    }
}

