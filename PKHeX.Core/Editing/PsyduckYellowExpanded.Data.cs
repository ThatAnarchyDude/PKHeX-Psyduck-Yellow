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
        var data = Util.GetBinaryResource("personal_psyduck0040");
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
    public static ushort GetDisplaySpecies(ushort species)
    {
        if (species <= 493) return species;
        if (species >= SpeciesNames.Length) return 0;
        var national = GameInfo.GetStrings("en").specieslist;
        for (ushort i = 1; i < national.Length; i++)
            if (string.Equals(national[i], SpeciesNames[species], StringComparison.OrdinalIgnoreCase)) return i;
        return 0; // Unknown engine forms use the neutral sprite rather than an unrelated Pokemon.
    }
}

