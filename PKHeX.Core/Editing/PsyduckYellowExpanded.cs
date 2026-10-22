using System;
using static System.Buffers.Binary.BinaryPrimitives;
namespace PKHeX.Core;

/// <summary>Verified prototype 0038 hg-engine layout, separate from retail HGSS.</summary>
public static partial class PsyduckYellowExpanded
{
    public const string Name = "Psyduck Yellow Expanded Engine";
    public const int GeneralSize = 0xFFA0;
    public const int StorageSize = 0x1E4FC;
    public const int StorageStart = 0x10000;
    public const int PartitionSize = 0x40000;
    private static ReadOnlySpan<int> LegacyOffsets => [0, 96, 144, 1604, 3556, 4660, 4792, 5628, 6112, 8292, 9032, 14156, 14192, 16296, 17420, 17688, 19260, 19712, 20084, 21092, 26768, 26788, 26896, 27148, 30168, 33916, 36160, 40252, 46016, 47692, 47736, 47748, 47816, 47828, 47840, 49468, 51004, 55764, 58712, 58844, 59156];
    private static ReadOnlySpan<int> ExpandedOffsets => [0, 96, 144, 1604, 4052, 5156, 5288, 7084, 7568, 9748, 11456, 16580, 16616, 18720, 19844, 20112, 21684, 22136, 22508, 23516, 29192, 29212, 29320, 29572, 32592, 36340, 38584, 42676, 48440, 50116, 50160, 50172, 50240, 50252, 50264, 51892, 53428, 58188, 61136, 61268, 61580];
    public static int GeneralOffset(int offset)
    {
        var old = LegacyOffsets;
        for (int i = old.Length - 1; i >= 0; i--)
            if (offset >= old[i]) return ExpandedOffsets[i] + offset - old[i];
        throw new ArgumentOutOfRangeException(nameof(offset));
    }
    public static bool IsExpandedSave(ReadOnlySpan<byte> data)
        => data.Length == 0x80000 && (HasFooter(data, 0, GeneralSize, 0) || HasFooter(data, PartitionSize, GeneralSize, 0));
    private static bool HasFooter(ReadOnlySpan<byte> data, int offset, int size, ushort slot)
    {
        var footer = data.Slice(offset + size - 16, 16);
        return ReadUInt32LittleEndian(footer[4..]) == size &&
               ReadUInt32LittleEndian(footer[8..]) == SAV4.MAGIC_JAPAN_INTL &&
               ReadUInt16LittleEndian(footer[12..]) == slot;
    }
    private static bool IsValid(ReadOnlySpan<byte> data, int offset, int size, ushort slot)
        => HasFooter(data, offset, size, slot) && Checksums.CRC16_CCITT(data.Slice(offset, size - 16)) == ReadUInt16LittleEndian(data.Slice(offset + size - 2));
    public static int GetActiveBlock(ReadOnlySpan<byte> data, int start, int size)
    {
        ushort slot = (ushort)(start == 0 ? 0 : 1);
        bool first = IsValid(data, start, size, slot);
        bool second = IsValid(data, start + PartitionSize, size, slot);
        if (first != second) return first ? 0 : 1;
        var a = ReadUInt32LittleEndian(data.Slice(start + size - 16));
        var b = ReadUInt32LittleEndian(data.Slice(start + PartitionSize + size - 16));
        if (a == uint.MaxValue) return 1;
        if (b == uint.MaxValue) return 0;
        return unchecked((int)(b - a)) > 0 ? 1 : 0;
    }
}
