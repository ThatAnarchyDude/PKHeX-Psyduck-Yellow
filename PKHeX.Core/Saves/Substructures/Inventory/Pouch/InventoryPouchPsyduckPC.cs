using System;
using static System.Buffers.Binary.BinaryPrimitives;
namespace PKHeX.Core;

/// <summary>Released 0029 player-PC stacks stored in script variables, separate from the Bag.</summary>
public sealed class InventoryPouchPsyduckPC(IItemStorage info, int offset = 0xDE4 - 0x644 + (0x4152 - 0x4000) * 2)
    : InventoryPouch(InventoryType.PCItems, info, 999, offset, 10)
{
    private InventoryItem[] items = [];
    public override InventoryItem[] Items => items;
    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };
    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
            items[i] = new InventoryItem { Index = ReadUInt16LittleEndian(data[(Offset + i * 4)..]), Count = ReadUInt16LittleEndian(data[(Offset + i * 4 + 2)..]) };
    }
    public override void SetPouch(Span<byte> data)
    {
        if (items.Length != PouchDataSize) throw new InvalidOperationException("PC slot count changed");
        for (int i = 0; i < items.Length; i++)
        {
            WriteUInt16LittleEndian(data[(Offset + i * 4)..], (ushort)items[i].Index);
            WriteUInt16LittleEndian(data[(Offset + i * 4 + 2)..], (ushort)items[i].Count);
        }
        // Prevent the first PC login from replacing editor-created stacks with starter supplies.
        if (Count != 0) WriteUInt16LittleEndian(data[(Offset + 40)..], 1);
    }
}
