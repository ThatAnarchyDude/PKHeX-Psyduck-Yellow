using System.Reflection;
using PKHeX.Core;
using PKHeX.WinForms;
using PKHeX.WinForms.Controls;
internal static class Probe
{
    static void Check(bool v, string s) { if (!v) throw new Exception(s); }
    static T Field<T>(object o, string name) => (T)o.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!.GetValue(o)!;
    [STAThread] static void Main(string[] args)
    {
        try { Run(args); } catch (Exception ex) { Console.Error.WriteLine(ex); Environment.ExitCode = 1; }
    }
    static void Run(string[] args)
    {
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Main).Assembly.GetType("PKHeX.WinForms.Program")!.TypeHandle);
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Console.WriteLine("PKHeX startup initialized; creating full form.");
        using var main = new Main();
        Console.WriteLine("Full form created.");
        var save = new SAV4HGSS { Version = GameVersion.HG, OT = "PLAYER", Language = 2 };
        GameInfo.FilteredSources = new FilteredGameDataSource(save, GameInfo.Sources, false);
        var reset = typeof(Main).GetMethod("ResetSAVPKMEditors", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!;
        reset.Invoke(main, [save]);
        var menu = Field<ToolStripMenuItem>(main, "Menu_PsyduckYellow");
        menu.Checked = true;
        Check(save.IsPsyduckYellow0029, "Built-in profile menu failed");
        var editor = Field<PKMEditor>(main, "PKME_Tabs");
        var ability = Field<ComboBox>(editor, "CB_Ability");
        foreach (ushort species in new ushort[] { 54, 55, 133, 134, 135, 136, 197, 470, 471, 175, 176, 468 })
        {
            var starter = PsyduckYellow0029.Starters[species is 54 or 55 ? 0 : species is 175 or 176 or 468 ? 2 : 1].ConvertToPKM(save);
            starter.Species = species;
            starter.Ability = PsyduckYellow0029.GetHiddenAbility(species);
            var low = starter.PID & 0xFFFF;
            starter.PID = low | ((low ^ starter.TID16 ^ starter.SID16 ^ 12) << 16);
            starter.HealPP();
            editor.PopulateFields(starter);
            Check(ability.SelectedIndex == 2, $"HA not selected for {species}");
            var pid = editor.Entity.PID;
            var edited = editor.CurrentPKM;
            Check(edited.Ability == starter.Ability && edited.PID == pid, $"Normal editor rewrote HA/PID for {species}");
            Check(edited.IsShiny, "Native editor failed project shiny display");
            Console.WriteLine($"Native editor species {species}: HA, PID and shiny preserved.");
        }
        using var inventory = new SAV_Inventory(save);
        var grids = Field<Dictionary<InventoryType, DataGridView>>(inventory, "ControlGrids");
        Check(grids.ContainsKey(InventoryType.PCItems) && grids[InventoryType.PCItems].Rows.Count == 10, "PC inventory tab/rows missing");
        var names = Field<string[]>(inventory, "itemlist");
        Check(names[114] == "Shiny Charm" && names[121] == "Friendship Charm", "Inventory names failed");
        Check(names[115] == "TM34 Bide" && names[123] == "TM19 Seismic Toss", "Custom TM name failed");
        Check(save.Personal[55].Type2 == (byte)MoveType.Psychic, "Native profile Golduck typing missing");
        using var retailInventory = new SAV_Inventory(new SAV4HGSS());
        Check(Field<string[]>(retailInventory, "itemlist")[114] != "Shiny Charm", "Names leaked into retail");
        main.CreateControl();
        if (args.Length > 0)
        {
            using var image = new System.Drawing.Bitmap(main.Width, main.Height);
            main.DrawToBitmap(image, main.ClientRectangle);
            image.Save(args[0]);
        }
        Console.WriteLine("Full PKHeX form, built-in profile, all 12 supported HAs, shiny display, inventory and retail isolation: PASS");
    }
}
