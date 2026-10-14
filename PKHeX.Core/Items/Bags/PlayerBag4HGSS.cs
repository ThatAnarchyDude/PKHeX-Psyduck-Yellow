using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag4HGSS : PlayerBag
{
    private const int BaseOffset = 0x644;

    public override IReadOnlyList<InventoryPouch> Pouches { get; } = GetPouches(ItemStorage4HGSS.Instance);
    public override ItemStorage4HGSS Info { get; } = ItemStorage4HGSS.Instance;

    private static InventoryPouch[] GetPouches(ItemStorage4HGSS info) =>
    [
        new InventoryPouch4(0x000, 999, info, Items),
        new InventoryPouch4(0x294, 001, info, KeyItems),
        new InventoryPouch4(0x35C, 099, info, TMHMs),
        new InventoryPouch4(0x4F0, 999, info, MailItems),
        new InventoryPouch4(0x520, 999, info, Medicine),
        new InventoryPouch4(0x5C0, 999, info, Berries),
        new InventoryPouch4(0x6C0, 999, info, Balls),
        new InventoryPouch4(0x720, 999, info, BattleItems),
    ];

    public PlayerBag4HGSS(SAV4HGSS sav)
    {
        Info = sav.IsPsyduckYellow0029 ? ItemStorage4HGSS.PsyduckYellow : ItemStorage4HGSS.Instance;
        Pouches = sav.IsPsyduckYellow0029 ? [.. GetPouches(Info), new InventoryPouchPsyduckPC(Info)] : GetPouches(Info);
        Pouches.LoadAll(sav.General[BaseOffset..]);
    }
    public PlayerBag4HGSS(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4HGSS)sav);
    public void CopyTo(SAV4HGSS sav) => CopyTo(sav.General[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
