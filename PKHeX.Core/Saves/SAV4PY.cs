using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> format for Psyduck Yellow (<see cref="GameVersion.PY"/>), the expanded hg-engine build of HeartGold.
/// </summary>
/// <remarks>
/// The save keeps HeartGold's ROM code byte so stored Pokémon origin data stays valid for the game; <see cref="GameVersion.PY"/> is never written into the save.
/// </remarks>
public sealed class SAV4PY : SAV4HGSS
{
    public SAV4PY() : base(expandedBlank: true) => ROMCode = (byte)GameVersion.HG;
    public SAV4PY(Memory<byte> data) : base(data, true) { }

    /// <summary>Reports <see cref="GameVersion.PY"/>; the ROM code stored in the save is left as the game wrote it (HeartGold).</summary>
    public override GameVersion Version
    {
        get => GameVersion.PY;
        set { if (value is not GameVersion.PY) base.Version = value; }
    }

    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4PY(Data.ToArray()) : new SAV4PY();
}
