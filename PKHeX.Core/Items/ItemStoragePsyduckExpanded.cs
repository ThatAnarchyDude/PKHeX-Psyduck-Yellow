using System;
using System.Collections.Generic;
namespace PKHeX.Core;

/// <summary>Item IDs and pocket assignment extracted from the released 0038 engine.</summary>
public sealed class ItemStoragePsyduckExpanded : IItemStorage
{
    public static readonly ItemStoragePsyduckExpanded Instance = new();
    private static readonly ushort[] PCItems = CreatePCItems();
    private static ushort[] CreatePCItems()
    {
        var all = new SortedSet<ushort>();
        for (int i = 0; i < 7; i++) foreach (var id in PsyduckYellowExpanded.ItemPockets[i]) all.Add(id);
        all.Add(114); all.Add(121);
        return [.. all];
    }
    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => GetItems(type).Contains((ushort)itemIndex);
    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => PsyduckYellowExpanded.ItemPockets[0],
        InventoryType.Medicine => PsyduckYellowExpanded.ItemPockets[1],
        InventoryType.Balls => PsyduckYellowExpanded.ItemPockets[2],
        InventoryType.TMHMs => PsyduckYellowExpanded.ItemPockets[3],
        InventoryType.Berries => PsyduckYellowExpanded.ItemPockets[4],
        InventoryType.MailItems => PsyduckYellowExpanded.ItemPockets[5],
        InventoryType.BattleItems => PsyduckYellowExpanded.ItemPockets[6],
        InventoryType.KeyItems => PsyduckYellowExpanded.ItemPockets[7],
        InventoryType.PCItems => PCItems,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };
}
