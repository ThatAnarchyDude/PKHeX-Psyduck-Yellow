using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.HG"/> and <see cref="GameVersion.SS"/>
/// </summary>
public sealed class ItemStorage4HGSS : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4HGSS Instance = new();
    public static readonly ItemStorage4HGSS PsyduckYellow = new(true);
    private readonly bool ProjectProfile;
    private static readonly ushort[] ProjectKeys = [.. KeyHGSS, 114, 121];
    private static readonly ushort[] ProjectGeneral = GetProjectGeneral();
    private static readonly ushort[] ProjectMachines = [.. Machine, 115, 116, 117, 118, 119, 120, 123];
    public ItemStorage4HGSS(bool projectProfile = false) => ProjectProfile = projectProfile;
    private static ushort[] GetProjectGeneral()
    {
        var items = new System.Collections.Generic.List<ushort>();
        foreach (var item in GeneralPt)
            if (item is not (>= 114 and <= 121) and not 123) items.Add(item);
        return items.ToArray();
    }

    private static readonly ushort[] ProjectPCItems = GetProjectPCItems();
    private static ushort[] GetProjectPCItems()
    {
        var all = new System.Collections.Generic.SortedSet<ushort>();
        foreach (var type in new[] { InventoryType.Items, InventoryType.Medicine, InventoryType.Balls, InventoryType.TMHMs, InventoryType.Berries, InventoryType.MailItems, InventoryType.BattleItems })
            foreach (var item in PsyduckYellow.GetItems(type)) all.Add(item);
        all.Add(114); all.Add(121);
        var result = new ushort[all.Count]; all.CopyTo(result); return result;
    }

    public static ReadOnlySpan<ushort> KeyHGSS =>
    [
                            434, 435,      437,
                            444, 445, 446, 447,
        450,                          456,
                            464, 465, 466,      468, 469,
        470, 471, 472, 473, 474, 475, 476, 477, 478, 479,
        480, 481, 482, 483, 484,
             501, 502, 503, 504,
                  532, 533, 534, 535, 536,
    ];

    public static ReadOnlySpan<ushort> BallsHGSS =>
    [
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        492, 493, 494, 495, 496, 497, 498, 499, 500, // Apricorn Balls
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => !Unreleased.Contains((ushort)itemIndex);

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.PCItems when ProjectProfile => ProjectPCItems,
        InventoryType.Items => ProjectProfile ? ProjectGeneral : GeneralPt,
        InventoryType.KeyItems => ProjectProfile ? ProjectKeys : KeyHGSS,
        InventoryType.TMHMs => ProjectProfile ? ProjectMachines : Machine,
        InventoryType.MailItems => Mail,
        InventoryType.Medicine => Medicine,
        InventoryType.Berries => Berry,
        InventoryType.Balls => BallsHGSS,
        InventoryType.BattleItems => Battle,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
