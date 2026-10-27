using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 4 <see cref="PKM"/> format. </summary>
public sealed class PK4 : G4PKM, IHyperTrain
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x42, 0x43, // Unused
        0x5E, // Unused
        0x63, 0x64, 0x65, 0x66, 0x67, // unused ribbon bits
    ];

    public override int SIZE_PARTY => PokeCrypto.SIZE_4PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
    public override EntityContext Context => EntityContext.Gen4;
    public override PersonalInfo4 PersonalInfo => (IsExpandedEngine ? PsyduckYellowExpanded.Personal : IsPsyduckYellow0029 ? PsyduckYellow0029.Personal : PersonalTable.HGSS).GetFormEntry(Species, Form);

    public PK4() : base(PokeCrypto.SIZE_4PARTY) { }
    public PK4(Memory<byte> data) : base(DecryptParty(data)) { }

    private static Memory<byte> DecryptParty(Memory<byte> data)
    {
        PokeCrypto.DecryptIfEncrypted45(data.Span);
        if (data.Length >= PokeCrypto.SIZE_4PARTY)
            return data;

        var result = new byte[PokeCrypto.SIZE_4PARTY];
        data.Span.CopyTo(result);
        return result;
    }

    // Profile metadata is intentionally never stored in the Pokemon/save bytes.
    public bool IsPsyduckYellow0029 { get; set; }
    public bool IsExpandedEngine { get; set; }
    /// <summary>Set on the retail-shaped copy used for legality of expanded-engine Pokémon (not stored).</summary>
    public bool IsExpandedLegalityView { get; set; }
    /// <summary>The Pokémon's own internal species on the retail-shaped legality copy (whose <see cref="PKM.Species"/> is mapped to the ordinary species); 0 otherwise.</summary>
    public ushort ExpandedSpecies { get; set; }
    public override int GetBasePP(ushort move) => IsExpandedEngine && move < PsyduckYellowExpanded.MovePP.Length ? PsyduckYellowExpanded.MovePP[move] : base.GetBasePP(move);
    public override bool IsShiny => IsPsyduckYellow0029 ? ShinyXor < 16 : base.IsShiny;
    public override PK4 Clone() => new(Data.ToArray()) { IsPsyduckYellow0029 = IsPsyduckYellow0029, IsExpandedEngine = IsExpandedEngine, IsExpandedLegalityView = IsExpandedLegalityView, ExpandedSpecies = ExpandedSpecies };

    // Structure
    public override uint PID { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public override ushort Sanity { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public override ushort Checksum { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }

    #region Block A
    public override ushort Species { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], (ushort)value); }
    public override uint ID32 { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data[0x0E..]); set => WriteUInt16LittleEndian(Data[0x0E..], value); }
    public override uint EXP { get => ReadUInt32LittleEndian(Data[0x10..]) & (IsExpandedEngine ? 0x1FFFFFu : uint.MaxValue); set => WriteUInt32LittleEndian(Data[0x10..], IsExpandedEngine ? (ReadUInt32LittleEndian(Data[0x10..]) & 0xFFE00000u) | (value & 0x1FFFFFu) : value); }
    public override byte OriginalTrainerFriendship { get => Data[0x14]; set => Data[0x14] = value; }
    public override int Ability { get => Data[0x15] | (IsExpandedEngine ? (int)(ReadUInt32LittleEndian(Data[0x10..]) >> 31) << 8 : 0); set { Data[0x15] = (byte)value; if (IsExpandedEngine) WriteUInt32LittleEndian(Data[0x10..], (ReadUInt32LittleEndian(Data[0x10..]) & 0x7FFFFFFFu) | (((uint)value & 0x100u) << 23)); } }
    public override byte MarkingValue { get => Data[0x16]; set => Data[0x16] = value; }
    public override int Language { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public override int EV_HP { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public override int EV_ATK { get => Data[0x19]; set => Data[0x19] = (byte)value; }
    public override int EV_DEF { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
    public override int EV_SPE { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public override int EV_SPA { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public override int EV_SPD { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
    public override byte ContestCool   { get => Data[0x1E]; set => Data[0x1E] = value; }
    public override byte ContestBeauty { get => Data[0x1F]; set => Data[0x1F] = value; }
    public override byte ContestCute   { get => Data[0x20]; set => Data[0x20] = value; }
    public override byte ContestSmart  { get => Data[0x21]; set => Data[0x21] = value; }
    public override byte ContestTough  { get => Data[0x22]; set => Data[0x22] = value; }
    public override byte ContestSheen  { get => Data[0x23]; set => Data[0x23] = value; }

    private byte RIB0 { get => Data[0x24]; set => Data[0x24] = value; } // Sinnoh 1
    private byte RIB1 { get => Data[0x25]; set => Data[0x25] = value; } // Sinnoh 2
    private byte RIB2 { get => Data[0x26]; set => Data[0x26] = value; } // Unova 1
    private byte RIB3 { get => Data[0x27]; set => Data[0x27] = value; } // Unova 2
    public override bool RibbonChampionSinnoh    { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonAbility           { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonAbilityGreat      { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonAbilityDouble     { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonAbilityMulti      { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonAbilityPair       { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonAbilityWorld      { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonAlert             { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonShock             { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonDowncast          { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonCareless          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonRelax             { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonSnooze            { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonSmile             { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonGorgeous          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonRoyal             { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonGorgeousRoyal     { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)((RIB2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonFootprint         { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)((RIB2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonRecord            { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)((RIB2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonEvent             { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)((RIB2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonLegend            { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)((RIB2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonChampionWorld     { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)((RIB2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonBirthday          { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)((RIB2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonSpecial           { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)((RIB2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonSouvenir          { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)((RIB3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonWishing           { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)((RIB3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonClassic           { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)((RIB3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonPremier           { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)((RIB3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RIB3_4 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)((RIB3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public override bool RIB3_5 { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)((RIB3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public override bool RIB3_6 { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)((RIB3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public override bool RIB3_7 { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)((RIB3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused

    public override int RibbonCount => BitOperations.PopCount(ReadUInt32LittleEndian(Data[0x24..]) & 0b00001111_11111111__11111111_11111111)
                                     + BitOperations.PopCount(ReadUInt32LittleEndian(Data[0x3C..]))
                                     + BitOperations.PopCount(ReadUInt32LittleEndian(Data[0x60..]) & 0b00000000_00001111__11111111_11111111);
    #endregion

    #region Block B
    public override ushort Move1 { get => ReadUInt16LittleEndian(Data[0x28..]); set => WriteUInt16LittleEndian(Data[0x28..], value); }
    public override ushort Move2 { get => ReadUInt16LittleEndian(Data[0x2A..]); set => WriteUInt16LittleEndian(Data[0x2A..], value); }
    public override ushort Move3 { get => ReadUInt16LittleEndian(Data[0x2C..]); set => WriteUInt16LittleEndian(Data[0x2C..], value); }
    public override ushort Move4 { get => ReadUInt16LittleEndian(Data[0x2E..]); set => WriteUInt16LittleEndian(Data[0x2E..], value); }
    public override int Move1_PP { get => Data[0x30]; set => Data[0x30] = (byte)value; }
    public override int Move2_PP { get => Data[0x31]; set => Data[0x31] = (byte)value; }
    public override int Move3_PP { get => Data[0x32]; set => Data[0x32] = (byte)value; }
    public override int Move4_PP { get => Data[0x33]; set => Data[0x33] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x34]; set => Data[0x34] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x35]; set => Data[0x35] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x36]; set => Data[0x36] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x37]; set => Data[0x37] = (byte)value; }
    public override uint IV32 { get => ReadUInt32LittleEndian(Data[0x38..]); set => WriteUInt32LittleEndian(Data[0x38..], value); }
    public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

    private byte RIB4 { get => Data[0x3C]; set => Data[0x3C] = value; } // Hoenn 1a
    private byte RIB5 { get => Data[0x3D]; set => Data[0x3D] = value; } // Hoenn 1b
    private byte RIB6 { get => Data[0x3E]; set => Data[0x3E] = value; } // Hoenn 2a
    private byte RIB7 { get => Data[0x3F]; set => Data[0x3F] = value; } // Hoenn 2b
    public override bool RibbonG3Cool            { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)((RIB4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG3CoolSuper       { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)((RIB4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG3CoolHyper       { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)((RIB4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG3CoolMaster      { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)((RIB4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonG3Beauty          { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)((RIB4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonG3BeautySuper     { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)((RIB4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonG3BeautyHyper     { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)((RIB4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonG3BeautyMaster    { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)((RIB4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonG3Cute            { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)((RIB5 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG3CuteSuper       { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)((RIB5 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG3CuteHyper       { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)((RIB5 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG3CuteMaster      { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)((RIB5 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonG3Smart           { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)((RIB5 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonG3SmartSuper      { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)((RIB5 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonG3SmartHyper      { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)((RIB5 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonG3SmartMaster     { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)((RIB5 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonG3Tough           { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)((RIB6 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG3ToughSuper      { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)((RIB6 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG3ToughHyper      { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)((RIB6 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG3ToughMaster     { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)((RIB6 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonChampionG3        { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)((RIB6 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonWinning           { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)((RIB6 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonVictory           { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)((RIB6 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonArtist            { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)((RIB6 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonEffort            { get => (RIB7 & (1 << 0)) == 1 << 0; set => RIB7 = (byte)((RIB7 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonChampionBattle    { get => (RIB7 & (1 << 1)) == 1 << 1; set => RIB7 = (byte)((RIB7 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonChampionRegional  { get => (RIB7 & (1 << 2)) == 1 << 2; set => RIB7 = (byte)((RIB7 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonChampionNational  { get => (RIB7 & (1 << 3)) == 1 << 3; set => RIB7 = (byte)((RIB7 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonCountry           { get => (RIB7 & (1 << 4)) == 1 << 4; set => RIB7 = (byte)((RIB7 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonNational          { get => (RIB7 & (1 << 5)) == 1 << 5; set => RIB7 = (byte)((RIB7 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonEarth             { get => (RIB7 & (1 << 6)) == 1 << 6; set => RIB7 = (byte)((RIB7 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonWorld             { get => (RIB7 & (1 << 7)) == 1 << 7; set => RIB7 = (byte)((RIB7 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

    public override bool FatefulEncounter { get => (Data[0x40] & 1) == 1; set => Data[0x40] = (byte)((Data[0x40] & ~0x01) | (value ? 1 : 0)); }
    public override byte Gender { get => (byte)((Data[0x40] >> 1) & 0x3); set => Data[0x40] = (byte)((Data[0x40] & ~0x06) | (value << 1)); }
    public override byte Form { get => (byte)(Data[0x40] >> 3); set => Data[0x40] = (byte)((Data[0x40] & 0x07) | (value << 3)); }
    public override int ShinyLeaf { get => IsExpandedEngine ? Data[0x41] & 0x3F : Data[0x41]; set => Data[0x41] = IsExpandedEngine ? (byte)((Data[0x41] & 0xC0) | (value & 0x3F)) : (byte)value; }
    public override int AbilityNumber
    {
        get => !IsExpandedEngine ? base.AbilityNumber : (Data[0x41] & 0x40) != 0 ? 4 : 1 << ((int)(PID & 1) ^ (Data[0x42] & 1));
        set
        {
            if (!IsExpandedEngine) return;
            Data[0x41] = (byte)((Data[0x41] & ~0x40) | (value == 4 ? 0x40 : 0));
            if (value is 1 or 2) Data[0x42] = (byte)((Data[0x42] & ~1) | ((value == 2 ? 1 : 0) ^ (int)(PID & 1)));
        }
    }
    public override Nature StatAlignment
    {
        get { int mint = (Data[0x42] >> 1) & 31; return IsExpandedEngine && mint is >= 1 and <= 25 ? (Nature)(mint - 1) : Nature; }
        set { if (IsExpandedEngine) Data[0x42] = (byte)((Data[0x42] & ~0x3E) | (((int)value + 1) << 1)); }
    }
    private bool GetEngineIV(int bit) => IsExpandedEngine && (ReadUInt16LittleEndian(Data[0x42..]) & (1 << bit)) != 0;
    private void SetEngineIV(int bit, bool value)
    {
        if (!IsExpandedEngine) return;
        ushort flags = ReadUInt16LittleEndian(Data[0x42..]);
        WriteUInt16LittleEndian(Data[0x42..], (ushort)((flags & ~(1 << bit)) | (value ? 1 << bit : 0)));
    }
    public bool HT_HP { get => GetEngineIV(6); set => SetEngineIV(6, value); }
    public bool HT_ATK { get => GetEngineIV(7); set => SetEngineIV(7, value); }
    public bool HT_DEF { get => GetEngineIV(8); set => SetEngineIV(8, value); }
    public bool HT_SPE { get => GetEngineIV(9); set => SetEngineIV(9, value); }
    public bool HT_SPA { get => GetEngineIV(10); set => SetEngineIV(10, value); }
    public bool HT_SPD { get => GetEngineIV(11); set => SetEngineIV(11, value); }
    public byte HyperTrainFlags
    {
        get => (byte)((HT_HP ? 1 : 0) | (HT_ATK ? 2 : 0) | (HT_DEF ? 4 : 0) | (HT_SPA ? 8 : 0) | (HT_SPD ? 16 : 0) | (HT_SPE ? 32 : 0));
        set { HT_HP = (value & 1) != 0; HT_ATK = (value & 2) != 0; HT_DEF = (value & 4) != 0; HT_SPA = (value & 8) != 0; HT_SPD = (value & 16) != 0; HT_SPE = (value & 32) != 0; }
    }
    // 0x42-0x43 Unused
    public override ushort EggLocationExtended
    {
        get => ReadUInt16LittleEndian(Data[0x44..]);
        set => WriteUInt16LittleEndian(Data[0x44..], value);
    }

    public override ushort MetLocationExtended
    {
        get => ReadUInt16LittleEndian(Data[0x46..]);
        set => WriteUInt16LittleEndian(Data[0x46..], value);
    }

    #endregion

    #region Block C
    public override string Nickname
    {
        get => StringConverter4.GetString(NicknameTrash);
        set
        {
            var language = Language;
            CheckKoreanNidoranDPPt(value, ref language);
            StringConverter4.SetString(NicknameTrash, value, 10, language, StringConverterOption.None);
        }
    }

    // 0x5E unused
    public GameVersion ApplicationVersion => IsPsyduckYellow0029 ? GameVersion.PY : Version;
    public override GameVersion Version
    {
        get => (GameVersion)Data[0x5F];
        set
        {
            if (value == GameVersion.PY)
                IsPsyduckYellow0029 = true; // Psyduck Yellow is a valid origin game; stored as-is
            Data[0x5F] = (byte)value;
        }
    }
    private byte RIB8 { get => Data[0x60]; set => Data[0x60] = value; } // Sinnoh 3
    private byte RIB9 { get => Data[0x61]; set => Data[0x61] = value; } // Sinnoh 4
    private byte RIBA { get => Data[0x62]; set => Data[0x62] = value; } // Sinnoh 5
    private byte RIBB { get => Data[0x63]; set => Data[0x63] = value; } // Sinnoh 6
    public override bool RibbonG4Cool            { get => (RIB8 & (1 << 0)) == 1 << 0; set => RIB8 = (byte)((RIB8 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG4CoolGreat       { get => (RIB8 & (1 << 1)) == 1 << 1; set => RIB8 = (byte)((RIB8 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG4CoolUltra       { get => (RIB8 & (1 << 2)) == 1 << 2; set => RIB8 = (byte)((RIB8 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG4CoolMaster      { get => (RIB8 & (1 << 3)) == 1 << 3; set => RIB8 = (byte)((RIB8 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonG4Beauty          { get => (RIB8 & (1 << 4)) == 1 << 4; set => RIB8 = (byte)((RIB8 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonG4BeautyGreat     { get => (RIB8 & (1 << 5)) == 1 << 5; set => RIB8 = (byte)((RIB8 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonG4BeautyUltra     { get => (RIB8 & (1 << 6)) == 1 << 6; set => RIB8 = (byte)((RIB8 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonG4BeautyMaster    { get => (RIB8 & (1 << 7)) == 1 << 7; set => RIB8 = (byte)((RIB8 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonG4Cute            { get => (RIB9 & (1 << 0)) == 1 << 0; set => RIB9 = (byte)((RIB9 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG4CuteGreat       { get => (RIB9 & (1 << 1)) == 1 << 1; set => RIB9 = (byte)((RIB9 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG4CuteUltra       { get => (RIB9 & (1 << 2)) == 1 << 2; set => RIB9 = (byte)((RIB9 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG4CuteMaster      { get => (RIB9 & (1 << 3)) == 1 << 3; set => RIB9 = (byte)((RIB9 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RibbonG4Smart           { get => (RIB9 & (1 << 4)) == 1 << 4; set => RIB9 = (byte)((RIB9 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public override bool RibbonG4SmartGreat      { get => (RIB9 & (1 << 5)) == 1 << 5; set => RIB9 = (byte)((RIB9 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public override bool RibbonG4SmartUltra      { get => (RIB9 & (1 << 6)) == 1 << 6; set => RIB9 = (byte)((RIB9 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public override bool RibbonG4SmartMaster     { get => (RIB9 & (1 << 7)) == 1 << 7; set => RIB9 = (byte)((RIB9 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public override bool RibbonG4Tough           { get => (RIBA & (1 << 0)) == 1 << 0; set => RIBA = (byte)((RIBA & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public override bool RibbonG4ToughGreat      { get => (RIBA & (1 << 1)) == 1 << 1; set => RIBA = (byte)((RIBA & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public override bool RibbonG4ToughUltra      { get => (RIBA & (1 << 2)) == 1 << 2; set => RIBA = (byte)((RIBA & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public override bool RibbonG4ToughMaster     { get => (RIBA & (1 << 3)) == 1 << 3; set => RIBA = (byte)((RIBA & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public override bool RIBA_4 { get => (RIBA & (1 << 4)) == 1 << 4; set => RIBA = (byte)((RIBA & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public override bool RIBA_5 { get => (RIBA & (1 << 5)) == 1 << 5; set => RIBA = (byte)((RIBA & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public override bool RIBA_6 { get => (RIBA & (1 << 6)) == 1 << 6; set => RIBA = (byte)((RIBA & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public override bool RIBA_7 { get => (RIBA & (1 << 7)) == 1 << 7; set => RIBA = (byte)((RIBA & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
    public override bool RIBB_0 { get => (RIBB & (1 << 0)) == 1 << 0; set => RIBB = (byte)((RIBB & ~(1 << 0)) | (value ? 1 << 0 : 0)); } // Unused
    public override bool RIBB_1 { get => (RIBB & (1 << 1)) == 1 << 1; set => RIBB = (byte)((RIBB & ~(1 << 1)) | (value ? 1 << 1 : 0)); } // Unused
    public override bool RIBB_2 { get => (RIBB & (1 << 2)) == 1 << 2; set => RIBB = (byte)((RIBB & ~(1 << 2)) | (value ? 1 << 2 : 0)); } // Unused
    public override bool RIBB_3 { get => (RIBB & (1 << 3)) == 1 << 3; set => RIBB = (byte)((RIBB & ~(1 << 3)) | (value ? 1 << 3 : 0)); } // Unused
    public override bool RIBB_4 { get => (RIBB & (1 << 4)) == 1 << 4; set => RIBB = (byte)((RIBB & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public override bool RIBB_5 { get => (RIBB & (1 << 5)) == 1 << 5; set => RIBB = (byte)((RIBB & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public override bool RIBB_6 { get => (RIBB & (1 << 6)) == 1 << 6; set => RIBB = (byte)((RIBB & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public override bool RIBB_7 { get => (RIBB & (1 << 7)) == 1 << 7; set => RIBB = (byte)((RIBB & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
    // 0x64-0x67 Unused
    #endregion

    #region Block D
    public override string OriginalTrainerName
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, 7, Language, StringConverterOption.None);
    }

    public override byte EggYear { get => Data[0x78]; set => Data[0x78] = value; }
    public override byte EggMonth { get => Data[0x79]; set => Data[0x79] = value; }
    public override byte EggDay { get => Data[0x7A]; set => Data[0x7A] = value; }
    public override byte MetYear { get => Data[0x7B]; set => Data[0x7B] = value; }
    public override byte MetMonth { get => Data[0x7C]; set => Data[0x7C] = value; }
    public override byte MetDay { get => Data[0x7D]; set => Data[0x7D] = value; }

    public override ushort EggLocationDP
    {
        get => ReadUInt16LittleEndian(Data[0x7E..]);
        set => WriteUInt16LittleEndian(Data[0x7E..], value);
    }
    public override ushort MetLocationDP
    {
        get => ReadUInt16LittleEndian(Data[0x80..]);
        set => WriteUInt16LittleEndian(Data[0x80..], value);
    }

    public override byte PokerusState { get => Data[0x82]; set => Data[0x82] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    public override byte BallDPPt { get => Data[0x83]; set => Data[0x83] = value; }
    public override byte MetLevel { get => (byte)(Data[0x84] & ~0x80); set => Data[0x84] = (byte)((Data[0x84] & 0x80) | value); }
    public override byte OriginalTrainerGender { get => (byte)(Data[0x84] >> 7); set => Data[0x84] = (byte)((Data[0x84] & ~0x80) | (value << 7)); }
    public override GroundTileType GroundTile { get => (GroundTileType)Data[0x85]; set => Data[0x85] = (byte)value; }
    public override byte BallHGSS { get => Data[0x86]; set => Data[0x86] = value; }
    public override sbyte WalkingMood { get => (sbyte)Data[0x87]; set => Data[0x87] = (byte)value; }
    #endregion

    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data[0x88..]); set => WriteInt32LittleEndian(Data[0x88..], value); }
    public override byte Stat_Level { get => Data[0x8C]; set => Data[0x8C] = value; }
    public byte BallCapsuleIndex { get => Data[0x8D]; set => Data[0x8D] = value; }
    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data[0x8E..]); set => WriteUInt16LittleEndian(Data[0x8E..], (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data[0x90..]); set => WriteUInt16LittleEndian(Data[0x90..], (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data[0x92..]); set => WriteUInt16LittleEndian(Data[0x92..], (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data[0x94..]); set => WriteUInt16LittleEndian(Data[0x94..], (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data[0x96..]); set => WriteUInt16LittleEndian(Data[0x96..], (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data[0x98..]); set => WriteUInt16LittleEndian(Data[0x98..], (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data[0x9A..]); set => WriteUInt16LittleEndian(Data[0x9A..], (ushort)value); }

    public Span<byte> HeldMail => Data.Slice(0x9C, 0x38);
    public Span<byte> Seals => Data.Slice(0xD4, 0x18);

    #endregion

    // Methods

    public BK4 ConvertToBK4()
    {
        BK4 bk4 = ConvertTo<BK4>();

        StripPtHGSSContent(bk4);
        bk4.RefreshChecksum();
        return bk4;
    }

    public RK4 ConvertToRK4()
    {
        var rk4 = new RK4 { OwnershipType = RanchOwnershipType.Hayley };
        var stored = Data[..PokeCrypto.SIZE_4STORED];
        stored.CopyTo(rk4.Data);

        rk4.RefreshChecksum();
        return rk4;
    }

    public PK5 ConvertToPK5()
    {
        var pk5 = new PK5();
        var stored = Data[..PokeCrypto.SIZE_4STORED];
        stored.CopyTo(pk5.Data);
        // Double Check Location Data to see if we were originally a PK5, no need to adjust.
        if (Version <= GameVersion.CXD && MetLocationDP > 0x4000)
            return pk5;

        pk5.JunkByte = 0;
        pk5.OriginalTrainerFriendship = 70;
        if (!EntityConverter.RetainMetDateTransfer45)
            EncounterDate.GetDateNDS(); // Apply new met date
        pk5.HeldMail.Clear();

        // Arceus Type Changing -- Plate forcibly removed.
        if (pk5.Species == (int)Core.Species.Arceus)
        {
            pk5.Form = 0;
            pk5.HeldItem = 0;
        }
        else if (!Legal.HeldItems_BW.Contains((ushort)HeldItem))
        {
            pk5.HeldItem = 0; // if valid, it's already copied
        }

        // Fix PP
        pk5.HealPP();

        // Disassociate Nature and PID, pk4 getter does PID%25
        pk5.Nature = Nature;

        // Delete Platinum/HGSS Met Location Data
        pk5.Data.Slice(0x44, 4).Clear();

        // Met / Crown Data Detection
        pk5.MetLocation = PK5.GetTransferMetLocation4(pk5);

        // Egg Location is not modified; when clearing Pt/HGSS egg data, the location will revert to Faraway Place
        // pk5.EggLocation = EggLocation;

        // Delete HG/S Data
        pk5.Data.Slice(0x86, 2).Clear();
        pk5.Ball = Ball;

        // Transfer Nickname and OT Name, update encoding
        TransferTrash(NicknameTrash, pk5.NicknameTrash, Language);
        TransferTrash(OriginalTrainerTrash, pk5.OriginalTrainerTrash, Language);

        // Fix Level
        pk5.MetLevel = pk5.CurrentLevel;

        // Remove HM moves; Defog should be kept if both are learned.
        // If it has Defog, remove Whirlpool.
        bool hasDefog = HasMove((int) Move.Defog);
        var banned = PersonalInfo4.GetPreferredTransferHMs(hasDefog);
        if (banned.Contains(Move1)) pk5.Move1 = 0;
        if (banned.Contains(Move2)) pk5.Move2 = 0;
        if (banned.Contains(Move3)) pk5.Move3 = 0;
        if (banned.Contains(Move4)) pk5.Move4 = 0;
        pk5.FixMoves();

        // D/P(not Pt)/HG/SS created Shedinja forget to set Gender to Genderless.
        if (pk5.Species is (int)Core.Species.Shedinja)
            pk5.Gender = 2; // Genderless

        pk5.RefreshChecksum();
        return pk5;
    }

    public static void TransferTrash(ReadOnlySpan<byte> src, Span<byte> dest, int language)
    {
        Span<char> temp = stackalloc char[13];
        var len = StringConverter4.LoadString(src, temp);
        temp = temp[..len];
        StringConverter345.TransferGlyphs45(temp);
        StringConverter5.SetString(dest, temp, len, language);
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter4.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter4.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter4.SetString(destBuffer, value, maxLength, Language, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data, StringConverter4.Terminator);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data, StringConverter4.Terminator);
    public override int GetBytesPerChar() => 2;
}

