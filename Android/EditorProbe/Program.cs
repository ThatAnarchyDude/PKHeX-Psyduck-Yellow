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
if (args.Length == 2 && args[0] == "--verify-party-export")
{
    var model = new SaveSession(File.ReadAllBytes(args[1]));
    var first = model.GetParty(0); var second = model.GetParty(1);
    if (model.Save.PartyCount != 2 || first.CurrentLevel != 6 || first.HeldItem != 234 || second.Species != 133 || second.CurrentLevel != 5 || second.Move1 != 98)
        throw new Exception("Party/item/move UI changes did not survive export.");
    if (model.Save.GetPartySlotAtIndex(2).Species != 0 || second.Move1_PP != second.GetMovePP(98, 0))
        throw new Exception("Removed slot or move PP is incorrect.");
    Console.WriteLine("PASS: Android UI created and removed party entries, assigned Leftovers, edited moves and exported a valid two-Pokémon party.");
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

    // Party changes must survive real save serialization without changing story state.
    var team = new SaveSession(input) { PsyduckProfile = true };
    var keep = Enumerable.Range(0, team.Save.PartyCount).Select(i => team.GetParty(i).Data.ToArray()).ToArray();
    int countBefore = team.Save.PartyCount;
    int added = team.AddPokemon(133, 5, 1, 3, true);
    var eevee = team.GetParty(added);
    Check(added == countBefore && team.Save.PartyCount == countBefore + 1, "Adding Pokémon did not update party count.");
    Check(eevee.Species == 133 && eevee.Gender == 1 && (int)eevee.Nature == 3 && team.IsShiny(eevee), "Created identity does not match requested properties.");
    Check(EntityGender.GetFromPIDAndRatio(eevee.PID, eevee.PersonalInfo.Gender) == eevee.Gender, "Created PID/gender mismatch.");
    Check(eevee.Move1 != 0 && eevee.Stat_HPCurrent == eevee.Stat_HPMax && eevee.OriginalTrainerName == team.Save.OT, "Created Pokémon lacks usable moves/stats/OT.");
    int[] newIVs = [eevee.IV_HP,eevee.IV_ATK,eevee.IV_DEF,eevee.IV_SPA,eevee.IV_SPD,eevee.IV_SPE];
    team.EditParty(added, 5, eevee.CurrentFriendship, newIVs, eevee.Ability, 234);
    Check(team.GetParty(added).HeldItem == 234 && team.GetParty(added).PID == eevee.PID, "Held-item edit changed identity.");
    team.EditMoves(added, [98,0,33,0]);
    Check(team.GetParty(added).Move2 == 33 && team.GetParty(added).Move3 == 0 && team.GetParty(added).Move1_PP == team.GetParty(added).GetMovePP(98,0), "Moves were not compacted or PP reset.");
    var partyReload = new SaveSession(team.Export());
    Check(partyReload.Save.PartyCount == countBefore + 1 && partyReload.GetParty(added).HeldItem == 234 && partyReload.GetParty(added).Move1 == 98, "Added Pokémon, item or moves lost on reload.");
    for(int i=0;i<countBefore;i++) Check(team.GetParty(i).Data.SequenceEqual(keep[i]), "Adding a party member changed an existing Pokémon.");
    Check(team.Save.General.Slice(0xDE4,1100).SequenceEqual(progress) && team.Save.General.Slice(0x12B8,0x338).SequenceEqual(dex), "Party edit changed story/Pokédex.");
    var shifted = team.GetParty(1).Data.ToArray();
    team.RemovePokemon(0);
    Check(team.GetParty(0).Data.SequenceEqual(shifted) && team.Save.GetPartySlotAtIndex(team.Save.PartyCount).Species == 0, "Removing a member did not compact or clear the party.");
    while(team.Save.PartyCount < 6) team.AddPokemon(54, 5);
    Check(new SaveSession(team.Export()).Save.PartyCount == 6, "Full six-member party lost on reload.");
    var beforeRejected = team.Export();
    try { team.AddPokemon(54,5); throw new Exception("Seventh Pokémon accepted."); } catch(InvalidOperationException) {checks++;}
    Check(team.Export().SequenceEqual(beforeRejected), "Rejected seventh member changed the save.");
    while(team.Save.PartyCount > 1) team.RemovePokemon(team.Save.PartyCount-1);
    try {team.RemovePokemon(0);throw new Exception("Final party member removed.");}catch(InvalidOperationException){checks++;}
    try {team.AddPokemon(493,5,1);throw new Exception("Genderless species accepted as female.");}catch(ArgumentException){checks++;}
    try {team.AddPokemon(494,5);throw new Exception("Unsupported species accepted.");}catch(ArgumentException){checks++;}
    try {team.EditMoves(0,[98,98,0,0]);throw new Exception("Duplicate moves accepted.");}catch(ArgumentException){checks++;}
    var lead=team.GetParty(0);int[] leadIVs=[lead.IV_HP,lead.IV_ATK,lead.IV_DEF,lead.IV_SPA,lead.IV_SPD,lead.IV_SPE];
    try {team.EditParty(0,lead.CurrentLevel,lead.CurrentFriendship,leadIVs,lead.Ability,450);throw new Exception("Key item assigned as held item.");}catch(ArgumentException){checks++;}
    team.EditParty(0,lead.CurrentLevel,lead.CurrentFriendship,leadIVs,lead.Ability,0);
    Check(team.GetParty(0).HeldItem==0,"Held item could not be cleared.");
}
try { new SaveSession(new byte[42]); throw new Exception("Malformed file accepted."); }
catch (InvalidDataException) { checks++; }
Console.WriteLine($"PASS: {checks} assertions; .sav/.dsv exports, identity, story flags, Pokédex, abilities, shiny boundaries, validation.");
