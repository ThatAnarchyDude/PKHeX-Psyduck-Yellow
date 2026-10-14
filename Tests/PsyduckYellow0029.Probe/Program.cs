using PKHeX.Core;
using System.Text.Json;
static void Require(bool check, string message) { if (!check) throw new Exception(message); }
var cases = new List<object>();
Require((byte)GameVersion.PY == 254 && !GameVersion.PY.IsValidSavedVersion() && GameVersion.PY.Generation == 4, "Application-only PY identifier invalid");
var pyMon = new PK4 { Version = GameVersion.PY };
Require(pyMon.ApplicationVersion == GameVersion.PY && pyMon.Version == GameVersion.HG && pyMon.Data[0x5F] == 7, "PY leaked into serialized Pokemon origin");
var pySave = new SAV4HGSS { Version = GameVersion.PY };
Require(pySave.ApplicationVersion == GameVersion.PY && pySave.Version == GameVersion.HG, "PY leaked into save ROM code");
Require(new PK4 { Species = 55, IsPsyduckYellow0029 = true }.PersonalInfo.Type2 == (byte)MoveType.Psychic, "Project Golduck typing missing");
Require(new PK4 { Species = 55 }.PersonalInfo.Type2 == (byte)MoveType.Water, "Golduck typing leaked into retail");
Require(PsyduckYellow0029.CanLearnCustomTM(54, 69), "Psyduck Seismic Toss compatibility missing");
Require(!PsyduckYellow0029.CanLearnCustomTM(129, 69), "Impossible Seismic Toss compatibility accepted");
var trainer = new SAV4HGSS { Version = GameVersion.HG, OT = "TEST", TID16 = 12345, SID16 = 54321, Language = 2 };
foreach (var encounter in PsyduckYellow0029.Starters)
{
    var pk = encounter.ConvertToPKM(trainer);
    pk.IsPsyduckYellow0029 = true;
    pk.Ability = PsyduckYellow0029.GetHiddenAbility(pk.Species);
    if (pk.Species == 175) pk.Move1 = (ushort)Move.Tackle;
    pk.HealPP();
    pk.RefreshChecksum();
    var analysis = new LegalityAnalysis(pk);
    Console.WriteLine($"{pk.Species} custom: {analysis.Valid}\n{analysis.Report(true)}");
    Require(analysis.Valid, $"Starter {pk.Species} rejected");
    foreach (var slot in new int[] { 0, 1 })
    {
        var ordinary = encounter.ConvertToPKM(trainer, EncounterCriteria.Unrestricted with
        { Ability = slot == 0 ? AbilityPermission.OnlyFirst : AbilityPermission.OnlySecond });
        Require(new LegalityAnalysis(ordinary).Valid, "Permitted ordinary starter ability rejected");
    }
    var evolved = pk.Clone();
    evolved.Species = pk.Species switch { 54 => 55, 133 => 197, _ => 176 };
    evolved.IsNicknamed = true;
    evolved.Ability = PsyduckYellow0029.GetHiddenAbility(evolved.Species);
    Require(new LegalityAnalysis(evolved).Valid, $"Stone-evolved starter {evolved.Species} rejected");
    var perfect = pk.Clone();
    perfect.IV_HP = perfect.IV_ATK = perfect.IV_DEF = perfect.IV_SPE = perfect.IV_SPA = perfect.IV_SPD = 31;
    Require(new LegalityAnalysis(perfect).Valid, "Permitted perfect-IV starter rejected");
    var retail = pk.Clone(); retail.IsPsyduckYellow0029 = false;
    Require(!new LegalityAnalysis(retail).Valid, "Custom HA leaked into retail HGSS");
    var bad = pk.Clone(); bad.MetLevel = 4;
    Require(!new LegalityAnalysis(bad).Valid, "Wrong starter met level accepted");
    bad = pk.Clone(); bad.MetLocation = 1;
    Require(!new LegalityAnalysis(bad).Valid, "Wrong starter location accepted");
    bad = pk.Clone(); bad.Ability = (int)Ability.WonderGuard;
    Require(!new LegalityAnalysis(bad).Valid, "Impossible starter ability accepted");
    bad = pk.Clone(); bad.Move1 = 459;
    Require(!new LegalityAnalysis(bad).Valid, "Impossible starter move accepted");
    bad = pk.Clone(); bad.Ball = (byte)Ball.Master;
    Require(!new LegalityAnalysis(bad).Valid, "Wrong gift ball accepted");
    foreach (uint xor in new uint[] { 0, 7, 8, 15, 16 })
    {
        var shiny = pk.Clone();
        var low = (uint)(shiny.PID & 0xFFFF);
        shiny.PID = low | ((low ^ shiny.TID16 ^ shiny.SID16 ^ xor) << 16);
        Require(shiny.IsShiny == (xor < 16), "Project shiny threshold incorrect");
        Require(shiny.Clone().IsShiny == shiny.IsShiny, "Clone lost profile");
        shiny.IsPsyduckYellow0029 = false;
        Require(shiny.IsShiny == (xor < 8), "Retail shiny threshold changed");
    }
    cases.Add(new { pk.Species, starterLegal = analysis.Valid, retailRejected = true });
}
Require(ItemStorage4HGSS.PsyduckYellow.GetItems(InventoryType.KeyItems).Contains((ushort)114), "Shiny Charm missing");
Require(!ItemStorage4HGSS.Instance.GetItems(InventoryType.KeyItems).Contains((ushort)114), "Charm leaked into retail");
if (args.Length != 0)
{
    var bytes = File.ReadAllBytes(args[0]);
    var save = SaveUtil.GetSaveFile(bytes, args[0]) as SAV4HGSS ?? throw new Exception("Save not recognized");
    Require(save.ChecksumsValid, "Source save checksum invalid");
    save.IsPsyduckYellow0029 = true;
    var clone = (SAV4HGSS)save.Clone();
    Require(clone.IsPsyduckYellow0029 && clone.BlankPKM.IsPsyduckYellow0029, "Save clone lost profile");
    var vars = save.General.Slice(0xDE4, 1100).ToArray();
    var written = clone.Write().ToArray();
    if (args.Length > 1)
    {
        if (File.Exists(args[1])) throw new Exception("Native export output already exists");
        Directory.CreateDirectory(Path.GetDirectoryName(args[1])!);
        File.WriteAllBytes(args[1], written);
    }
    var reopened = (SAV4HGSS)SaveUtil.GetSaveFile(written)!;
    Require(reopened.ChecksumsValid && written.Length == bytes.Length, "Roundtrip checksum/footer failed");
    Require(vars.AsSpan().SequenceEqual(reopened.General.Slice(0xDE4, 1100)), "Quest and PC storage changed");
    for (int i=0;i<save.PartyCount;i++)
    {
        var mon = save.GetPartySlotAtIndex(i);
        Require(mon is PK4 { IsPsyduckYellow0029: true }, "Party profile missing");
        Require(mon.Data.SequenceEqual(reopened.GetPartySlotAtIndex(i).Data), "Party bytes changed");
        Console.WriteLine($"Saved party: species={mon.Species} met={mon.MetLocation} level={mon.MetLevel} ability={mon.Ability}");
    }
    Console.WriteLine("Save roundtrip, party bytes, quest flags, PC and footer preserved.");
}
Console.WriteLine(JsonSerializer.Serialize(cases));
