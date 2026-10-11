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
    private int[] abilities = [];
    private bool rendering;
    private string SessionPath => Path.Combine(FilesDir!.AbsolutePath, "session.bin");
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        if (state?.GetBoolean("loaded") == true && File.Exists(SessionPath))
        {
            try { session = new(File.ReadAllBytes(SessionPath)); session.PsyduckProfile = state.GetBoolean("profile"); name = state.GetString("name") ?? name; }
            catch { session = null; }
        }
        Render();
    }
    protected override void OnSaveInstanceState(Bundle state)
    {
        if (session is not null)
        {
            File.WriteAllBytes(SessionPath, session.Export());
            state.PutBoolean("loaded", true); state.PutBoolean("profile", session.PsyduckProfile); state.PutString("name", name);
        }
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
        Button("Source code and instructions", (_, _) => StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/tree/master/Android"))));
        status = Label("");
        if (session is null) { rendering = false; return; }
        Label($"Trainer: {session.Save.OT}   •   Party: {session.Save.PartyCount}\n{name}");
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
        Label("Keep your original save backup. Close your emulator before replacing its save, then launch through the game's Continue option. This first version edits party level, friendship, IVs and abilities; boxes, bags and legality tools come later.", 13);
        rendering = false;
    }
    private void Apply()
    {
        if (session is null) return;
        int Parse(EditText field) => int.TryParse(field.Text, out var value) ? value : throw new ArgumentException("Enter a whole number in each field.");
        session.EditParty(slot, Parse(level), Parse(friendship), ivs.Select(Parse).ToArray(), abilities[ability.SelectedItemPosition]);
        File.WriteAllBytes(SessionPath, session.Export());
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
