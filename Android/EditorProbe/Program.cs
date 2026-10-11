// SPDX-License-Identifier: GPL-3.0-or-later
using PKHeX.Core;
using PsyduckSaveEditor;

if (args.Length == 2 && args[0] == "--create-fixture")
{
    byte[] container = new byte[524288];
    foreach (int offset in new[] {0, 0x40000})
    {
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(container.AsSpan(offset + SAV4HGSS.GeneralSize - 12), SAV4HGSS.GeneralSize);
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(container.AsSpan(offset + SAV4HGSS.GeneralSize - 8), SAV4.MAGIC_JAPAN_INTL);
    }
    var save = new SAV4HGSS(container) { Version = GameVersion.HG, OT = "TEST", Language = 2, ID32 = 123456 };
    var pk = new PK4 { Species = 54, Version = GameVersion.HG, Language = 2, ID32 = save.ID32,
        PID = 12345678, Gender = 0, Nickname = "PSYDUCK", IsNicknamed = true,
        OriginalTrainerName = "TEST", CurrentLevel = 5, CurrentFriendship = 40 };
    pk.Ability = pk.PersonalInfo.GetAbilityAtIndex(0);
    pk.ResetPartyStats();
    save.PartyData = new[] { pk };
    File.WriteAllBytes(args[1], save.Write().ToArray());
    Console.WriteLine("Synthetic UI fixture created; party: " + new SaveSession(File.ReadAllBytes(args[1])).Save.PartyCount);
    return;
}
if (args.Length == 2 && args[0] == "--verify-ui-export")
{
    var save = new SaveSession(File.ReadAllBytes(args[1]));
    if (save.GetParty(0).CurrentLevel != 6 || save.GetParty(0).Nickname != "PSYDUCK")
        throw new Exception("Android UI edits did not survive export.");
    Console.WriteLine("PASS: Android UI exported a valid save with the requested level-6 party member.");
    return;
}
if (args.Length != 1) throw new ArgumentException("Provide a private HGSS .sav/.dsv fixture path.");
var source = File.ReadAllBytes(args[0]);
int checks = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    checks++;
}
foreach (byte[] input in new[] { source, source[..524288] })
{
    var model = new SaveSession(input, input.Length == 524288 ? "fixture.sav" : "fixture.dsv") { PsyduckProfile = true };
    Check(model.Save.PartyCount > 0, "Fixture needs a party member.");
    var progress = model.Save.General.Slice(0xDE4, 1100).ToArray();
    var dex = model.Save.General.Slice(0x12B8, 0x338).ToArray();
    var original = model.GetParty(0);
    var identity = (original.PID, original.Species, original.Gender, original.Nickname, original.ShinyXor);
    var otherParty = Enumerable.Range(1, model.Save.PartyCount - 1).Select(i => model.GetParty(i).Data.ToArray()).ToArray();
    int[] unchangedIVs = [original.IV_HP, original.IV_ATK, original.IV_DEF, original.IV_SPA, original.IV_SPD, original.IV_SPE];
    model.EditParty(0, original.CurrentLevel, original.CurrentFriendship, unchangedIVs, original.Ability);
    Check(model.GetParty(0).Data.SequenceEqual(original.Data), "A no-change apply altered the Pokémon.");
    model.EditParty(0, 50, 255, [31, 31, 31, 31, 31, 31], original.Ability);
    var edited = model.GetParty(0);
    Check(edited.CurrentLevel == 50 && edited.CurrentFriendship == 255, "Level/friendship edit failed.");
    Check(new[] {edited.IV_HP, edited.IV_ATK, edited.IV_DEF, edited.IV_SPA, edited.IV_SPD, edited.IV_SPE}.All(x => x == 31), "IV edit failed.");
    Check(identity == (edited.PID, edited.Species, edited.Gender, edited.Nickname, edited.ShinyXor), "Identity changed.");
    var output = model.Export();
    Check(output.Length == input.Length, "Container size changed.");
    if (input.Length > 524288) Check(output.AsSpan(524288).SequenceEqual(input.AsSpan(524288)), "DSV footer changed.");
    var reopened = new SaveSession(output) { PsyduckProfile = true };
    Check(reopened.Save.ChecksumsValid, "Checksums invalid after export.");
    Check(reopened.GetParty(0).CurrentLevel == 50, "Edit lost on reload.");
    Check(reopened.Save.General.Slice(0xDE4, 1100).SequenceEqual(progress), "Story progress changed.");
    Check(reopened.Save.General.Slice(0x12B8, 0x338).SequenceEqual(dex), "Pokédex changed.");
    for (int i = 1; i < reopened.Save.PartyCount; i++) Check(reopened.GetParty(i).Data.SequenceEqual(otherParty[i-1]), "Another party member changed.");
    int hidden = SaveSession.HiddenAbility(edited.Species);
    if (hidden != 0)
    {
        model.EditParty(0, 50, 255, [31,31,31,31,31,31], hidden);
        var ha = new SaveSession(model.Export());
        Check(ha.GetParty(0).Ability == hidden, "Custom Hidden Ability lost during export.");
        Check(ha.AbilityChoices(ha.GetParty(0)).Contains(hidden), "Existing Hidden Ability unavailable in normal profile.");
    }
    var pk = model.GetParty(0);
    foreach (ushort xor in new ushort[] {0,7,8,15,16,65535})
    {
        pk.PID = pk.ID32 ^ xor;
        model.PsyduckProfile = true; Check(model.IsShiny(pk) == (xor < 16), "Project shiny boundary wrong.");
        model.PsyduckProfile = false; Check(model.IsShiny(pk) == (xor < 8), "HGSS shiny boundary wrong.");
    }
    try { model.EditParty(0, 101, 255, [31,31,31,31,31,31], original.Ability); throw new Exception("Invalid level accepted."); }
    catch (ArgumentException) { checks++; }
}
try { new SaveSession(new byte[42]); throw new Exception("Malformed file accepted."); }
catch (InvalidDataException) { checks++; }
Console.WriteLine($"PASS: {checks} assertions; .sav/.dsv exports, identity, story flags, Pokédex, abilities, shiny boundaries, validation.");
