// SPDX-License-Identifier: GPL-3.0-or-later
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PKHeX.Core;

namespace PsyduckSaveEditor;

[Activity(Label = "Psyduck Save Editor", MainLauncher = true, Exported = true)]
public sealed class MainActivity : Activity
{
    private SaveSession? session;
    private LinearLayout root = null!;
    private TextView status = null!;
    private string name = "save.sav";
    private int slot;
    private EditText level = null!, friendship = null!;
    private EditText[] ivs = [];
    private Spinner ability = null!;
    private AutoCompleteTextView heldItem = null!;
    private int[] abilities = [];
    private bool rendering;
    private string startupStage = "Activity created";
    private string DiagnosticPath => Path.Combine(FilesDir!.AbsolutePath, "startup-diagnostics.txt");
    private string Diagnostics(string? error = null) =>
        $"Psyduck Save Editor 0.1.2\nAndroid API: {(int)Build.VERSION.SdkInt}\n" +
        $"Manufacturer: {Build.Manufacturer}\nModel: {Build.Model}\n" +
        $"Supported ABIs: {string.Join(", ", Build.SupportedAbis ?? [])}\n" +
        $"Process architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}\n" +
        $"Stage: {startupStage}\n{error ?? "No managed startup error captured."}\n";
    private string SessionPath => Path.Combine(FilesDir!.AbsolutePath, "session.bin");
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        try
        {
            startupStage = "Restoring editor session";
            if (state?.GetBoolean("loaded") == true && File.Exists(SessionPath))
            {
                try { session = new(File.ReadAllBytes(SessionPath)); session.PsyduckProfile = state.GetBoolean("profile"); name = state.GetString("name") ?? name; }
                catch { session = null; }
            }
            startupStage = "Building home screen";
            Render();
            startupStage = "Home screen ready";
            File.WriteAllText(DiagnosticPath, Diagnostics());
        }
        catch (Exception e)
        {
            ShowStartupFailure(e);
        }
    }
    private void ShareDiagnostics()
    {
        var report = File.Exists(DiagnosticPath) ? File.ReadAllText(DiagnosticPath) : Diagnostics();
        var intent = new Intent(Intent.ActionSend);
        intent.SetType("text/plain"); intent.PutExtra(Intent.ExtraText, report);
        StartActivity(Intent.CreateChooser(intent, "Share startup diagnostics"));
    }
    private void ShowStartupFailure(Exception error)
    {
        var report = Diagnostics(error.ToString());
        Android.Util.Log.Error("PsyduckSaveEditor", report);
        try { File.WriteAllText(DiagnosticPath, report); } catch { }
        var scroll = new ScrollView(this);
        root = new LinearLayout(this) { Orientation = Orientation.Vertical };
        root.SetPadding(24, 48, 24, 24); scroll.AddView(root); SetContentView(scroll);
        Label("Startup could not finish", 22);
        Label("No game save was changed. Share the report below so we can investigate.");
        Label(report, 13);
        Button("Share startup diagnostics", (_, _) => Guard(ShareDiagnostics));
    }
    protected override void OnSaveInstanceState(Bundle state)
    {
        try
        {
            if (session is not null)
            {
                File.WriteAllBytes(SessionPath, session.Export());
                state.PutBoolean("loaded", true); state.PutBoolean("profile", session.PsyduckProfile); state.PutString("name", name);
            }
        }
        catch (Exception e) { Android.Util.Log.Error("PsyduckSaveEditor", "Session persistence failed: " + e.Message); }
        base.OnSaveInstanceState(state);
    }
    private void Render()
    {
        rendering = true;
        var scroll = new ScrollView(this);
        if (OperatingSystem.IsAndroidVersionAtLeast(30)) scroll.SetOnApplyWindowInsetsListener(new InsetsListener());
        root = new LinearLayout(this) { Orientation = Orientation.Vertical };
        int pad = (int)(16 * Resources!.DisplayMetrics!.Density);
        root.SetPadding(pad, pad, pad, pad); scroll.AddView(root); SetContentView(scroll);
        scroll.Post(() =>
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
                Window?.InsetsController?.SetSystemBarsAppearance(8 | 16, 8 | 16); // Light-theme system bars.
        });
        Label("Psyduck Save Editor", 24);
        Label("Unofficial Android prototype • PKHeX.Core • GPLv3", 13);
        Label("Open an emulator's in-game .sav or .dsv save. ROMs and emulator save states cannot be edited. Export makes a separate copy.");
        Button("Open save", (_, _) => Open());
        Button("Source code and instructions", (_, _) => Guard(() => StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/tree/master/Android")))));
        Button("Share startup diagnostics", (_, _) => Guard(ShareDiagnostics));
        status = Label("");
        if (session is null) { rendering = false; return; }
        Label($"Trainer: {session.Save.OT}   •   Party: {session.Save.PartyCount}\n{name}");
        Label($"Party slots used: {session.Save.PartyCount}/6");
        Button("Add Pokémon", (_, _) => Guard(() => { if (session.Save.PartyCount > 0) Apply(); AddPokemonDialog(); }));
        var profile = new Switch(this) { Text = "Psyduck Yellow custom rules", Checked = session.PsyduckProfile };
        root.AddView(profile);
        profile.CheckedChange += (_, e) => { if (!rendering) { Guard(() => { if (session.Save.PartyCount > 0) Apply(); session.PsyduckProfile = e.IsChecked; }); Render(); } };
        Label("Enable this only for Psyduck Yellow. Existing custom abilities and story flags are preserved in either profile.", 13);
        if (session.Save.PartyCount > 0)
        {
            slot = Math.Min(slot, session.Save.PartyCount - 1);
            var party = new Spinner(this);
            party.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem,
                Enumerable.Range(0, session.Save.PartyCount).Select(i => $"{i + 1}: {session.GetParty(i).Nickname} (Lv. {session.GetParty(i).CurrentLevel})").ToArray());
            root.AddView(party); party.SetSelection(slot);
            party.ItemSelected += (_, e) => { if (!rendering && e.Position != slot) { Guard(() => { Apply(); slot = e.Position; }); Render(); } };
            var pk = session.GetParty(slot);
            Button("Remove selected Pokémon", (_, _) => new AlertDialog.Builder(this)!.SetTitle("Remove from party?")!
                .SetMessage("This removes the selected Pokémon from the exported save, without moving it to a PC box. Keep your original backup.")!
                .SetNegativeButton("Cancel", (_, _) => { })!.SetPositiveButton("Remove", (_, _) => Guard(() =>
                {
                    session.RemovePokemon(slot); slot = Math.Max(0, slot - 1); Persist(); Render();
                }))!.Show());
            Label($"{GameInfo.Strings.Species[pk.Species]} • {(pk.Gender == 0 ? "Male" : pk.Gender == 1 ? "Female" : "Genderless")}\nShiny: {(session.IsShiny(pk) ? "Yes" : "No")} • Nature: {pk.Nature}");
            Label("Identity, gender, shiny value, story progress and Pokédex remain unchanged. Changing level or IVs recomputes stats and heals this Pokémon.", 13);
            level = Number("Level (1–100)", pk.CurrentLevel);
            friendship = Number("Friendship (0–255)", pk.CurrentFriendship);
            string[] labels = ["HP", "Attack", "Defense", "Sp. Attack", "Sp. Defense", "Speed"];
            int[] values = [pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE];
            ivs = labels.Select((label, i) => Number($"{label} IV (0–31)", values[i])).ToArray();
            Label("Ability"); abilities = session.AbilityChoices(pk);
            ability = new Spinner(this);
            ability.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem,
                abilities.Select(a => GameInfo.Strings.Ability[a]).ToArray());
            root.AddView(ability); ability.SetSelection(Array.IndexOf(abilities, pk.Ability));
            Label("Held item (name or ID; None removes it)");
            heldItem = ChoiceField(root, session.HeldItemChoices(pk), ItemNames, pk.HeldItem);
            Button("Edit moves", (_, _) => Guard(() => { Apply(); EditMovesDialog(); }));
            Button("Apply party edits", (_, _) => Guard(() => { Apply(); Render(); status.Text = "Edits applied. Export a copy to use them in your emulator."; }));
        }
        Button("Export edited copy", (_, _) => Guard(() =>
        {
            if (session.Save.PartyCount > 0) Apply();
            var intent = new Intent(Intent.ActionCreateDocument);
            intent.AddCategory(Intent.CategoryOpenable); intent.SetType("application/octet-stream");
            intent.PutExtra(Intent.ExtraTitle, Path.GetFileNameWithoutExtension(name) + "-edited" + Path.GetExtension(name));
            StartActivityForResult(intent, 2);
        }));
        Label("Keep your original save backup. Close your emulator before replacing its save, then launch through Continue. Created Pokémon are for customized play/testing; official encounter legality is not guaranteed. Boxes, bags and legality tools come later.", 13);
        rendering = false;
    }
    private void Apply()
    {
        if (session is null) return;
        int Parse(EditText field) => int.TryParse(field.Text, out var value) ? value : throw new ArgumentException("Enter a whole number in each field.");
        session.EditParty(slot, Parse(level), Parse(friendship), ivs.Select(Parse).ToArray(), abilities[ability.SelectedItemPosition],
            ParseChoice(heldItem, session.HeldItemChoices(session.GetParty(slot)), ItemNames));
        Persist();
    }
    private void Persist() => File.WriteAllBytes(SessionPath, session!.Export());
    private static string[] ItemNames => GameInfo.Strings.GetItemStrings(EntityContext.Gen4, GameVersion.HG);
    private static string ChoiceName(int id, IReadOnlyList<string> names) => id == 0 ? "None" : id < names.Count ? names[id] : "Existing custom item";
    private AutoCompleteTextView ChoiceField(LinearLayout container, int[] ids, IReadOnlyList<string> names, int selected)
    {
        var view = new AutoCompleteTextView(this) { Threshold = 1, Text = $"{selected}: {ChoiceName(selected, names)}" };
        view.SetSingleLine(true);
        view.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line,
            ids.Select(id => $"{id}: {ChoiceName(id, names)}").ToArray());
        container.AddView(view); return view;
    }
    private static int ParseChoice(AutoCompleteTextView field, int[] ids, IReadOnlyList<string> names)
    {
        var text = (field.Text ?? "").Trim();
        if (int.TryParse(text.Split(':')[0], out int id) && ids.Contains(id)) return id;
        foreach (int option in ids) if (string.Equals(text, ChoiceName(option, names), StringComparison.OrdinalIgnoreCase)) return option;
        throw new ArgumentException("Select a suggestion or enter a supported name or ID.");
    }
    private void AddPokemonDialog()
    {
        if (session!.Save.PartyCount >= 6) throw new InvalidOperationException("Your party already has six Pokémon.");
        var layout = new LinearLayout(this) { Orientation = Orientation.Vertical }; layout.SetPadding(24, 8, 24, 8);
        layout.AddView(new TextView(this) { Text = "Species (name or National Dex ID)" });
        int[] speciesIds = Enumerable.Range(1, 493).ToArray();
        var species = ChoiceField(layout, speciesIds, GameInfo.Strings.Species, 54);
        layout.AddView(new TextView(this) { Text = "Starting level (1–100)" });
        var startLevel = new EditText(this) { Text = "5", InputType = Android.Text.InputTypes.ClassNumber }; layout.AddView(startLevel);
        var gender = new Spinner(this);
        gender.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, new[] { "Species default gender", "Male", "Female", "Genderless" }); layout.AddView(gender);
        var nature = new Spinner(this);
        nature.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, GameInfo.Strings.Natures.Take(25).ToArray()); layout.AddView(nature);
        var shiny = new CheckBox(this) { Text = "Shiny" }; layout.AddView(shiny);
        layout.AddView(new TextView(this) { Text = "Random IVs and normal HeartGold level-up moves are generated. You can edit them afterward. Story and Pokédex progress are unchanged.", TextSize = 13 });
        var scroll = new ScrollView(this); scroll.AddView(layout);
        var dialog = new AlertDialog.Builder(this)!.SetTitle("Add Pokémon")!.SetView(scroll)!.SetNegativeButton("Cancel", (_, _) => { })!.SetPositiveButton("Add", (_, _) => { })!.Create()!;
        dialog.Show();
        dialog.GetButton((int)DialogButtonType.Positive)!.Click += (_, _) => Guard(() =>
        {
            if (!int.TryParse(startLevel.Text, out int value)) throw new ArgumentException("Enter a whole-number level.");
            slot = session.AddPokemon((ushort)ParseChoice(species, speciesIds, GameInfo.Strings.Species), value,
                gender.SelectedItemPosition == 0 ? null : (byte)(gender.SelectedItemPosition - 1), nature.SelectedItemPosition, shiny.Checked);
            Persist(); dialog.Dismiss(); Render(); status.Text = "Pokémon added. Export a copy to use the new party.";
        });
    }
    private void EditMovesDialog()
    {
        var pk = session!.GetParty(slot); int[] moveIds = Enumerable.Range(0, 468).ToArray();
        var layout = new LinearLayout(this) { Orientation = Orientation.Vertical }; layout.SetPadding(24, 8, 24, 8);
        ushort[] current = [pk.Move1, pk.Move2, pk.Move3, pk.Move4];
        var fields = new AutoCompleteTextView[4];
        for (int i = 0; i < fields.Length; i++)
        {
            layout.AddView(new TextView(this) { Text = $"Move {i + 1} (name or ID; None clears it)" });
            fields[i] = ChoiceField(layout, moveIds, GameInfo.Strings.Move, current[i]);
        }
        layout.AddView(new TextView(this) { Text = "Move PP is reset to its normal maximum. Move compatibility and official legality are not checked.", TextSize = 13 });
        var dialog = new AlertDialog.Builder(this)!.SetTitle("Edit moves")!.SetView(layout)!.SetNegativeButton("Cancel", (_, _) => { })!.SetPositiveButton("Apply moves", (_, _) => { })!.Create()!;
        dialog.Show();
        dialog.GetButton((int)DialogButtonType.Positive)!.Click += (_, _) => Guard(() =>
        {
            session.EditMoves(slot, fields.Select(f => (ushort)ParseChoice(f, moveIds, GameInfo.Strings.Move)).ToArray());
            Persist(); dialog.Dismiss(); Render(); status.Text = "Moves updated. Export a copy to use them.";
        });
    }
    private void Open()
    {
        var intent = new Intent(Intent.ActionOpenDocument);
        intent.AddCategory(Intent.CategoryOpenable); intent.SetType("*/*"); StartActivityForResult(intent, 1);
    }
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (resultCode != Result.Ok || data?.Data is null) return;
        Guard(() =>
        {
            if (requestCode == 1)
            {
                using var input = ContentResolver!.OpenInputStream(data.Data) ?? throw new IOException("Could not read this file.");
                using var memory = new MemoryStream();
                byte[] buffer = new byte[8192]; int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memory.Write(buffer, 0, count);
                    if (memory.Length > 524410) throw new InvalidDataException("This file is too large to be a normal HGSS save.");
                }
                string displayName = "save.sav";
                using var cursor = ContentResolver.Query(data.Data, null, null, null, null);
                if (cursor?.MoveToFirst() == true)
                {
                    int index = cursor.GetColumnIndex("_display_name");
                    if (index >= 0) displayName = cursor.GetString(index) ?? displayName;
                }
                var next = new SaveSession(memory.ToArray(), displayName);
                if (!displayName.EndsWith(".sav", StringComparison.OrdinalIgnoreCase) && !displayName.EndsWith(".dsv", StringComparison.OrdinalIgnoreCase))
                    displayName = memory.Length == 524410 ? "save.dsv" : "save.sav";
                File.WriteAllBytes(Path.Combine(FilesDir!.AbsolutePath, "original-backup.bin"), memory.ToArray());
                session = next; name = displayName; slot = 0;
                File.WriteAllBytes(SessionPath, next.Export()); Render(); status.Text = "Save opened. Original file untouched.";
            }
            else if (requestCode == 2 && session is not null)
            {
                using var output = ContentResolver!.OpenOutputStream(data.Data, "w") ?? throw new IOException("Could not write this file.");
                var bytes = session.Export(); output.Write(bytes, 0, bytes.Length); output.Flush();
                status.Text = "Exported a verified copy. Your original save is unchanged.";
            }
        });
    }
    private TextView Label(string text, int size = 16)
    {
        var view = new TextView(this) { Text = text, TextSize = size }; view.SetPadding(0, 8, 0, 8); root.AddView(view); return view;
    }
    private EditText Number(string label, int value)
    {
        Label(label); var view = new EditText(this) { Text = value.ToString(), InputType = Android.Text.InputTypes.ClassNumber };
        view.SetSingleLine(true); root.AddView(view); return view;
    }
    private void Button(string text, EventHandler handler)
    {
        var button = new Button(this) { Text = text }; button.Click += handler; root.AddView(button);
    }
    [System.Runtime.Versioning.SupportedOSPlatform("android30.0")]
    private sealed class InsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        public WindowInsets OnApplyWindowInsets(View view, WindowInsets insets)
        {
            var bars = insets.GetInsets(WindowInsets.Type.SystemBars() | WindowInsets.Type.Ime());
            view.SetPadding(bars.Left, bars.Top, bars.Right, bars.Bottom);
            return insets;
        }
    }
    private void Guard(Action action)
    {
        try { action(); }
        catch (Exception e) { new AlertDialog.Builder(this)!.SetTitle("Could not complete edit")!.SetMessage(e.Message)!.SetPositiveButton("OK", (_, _) => { })!.Show(); }
    }
}
