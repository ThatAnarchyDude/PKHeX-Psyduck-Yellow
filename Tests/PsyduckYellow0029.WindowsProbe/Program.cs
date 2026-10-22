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
        Check(main.FormBorderStyle == FormBorderStyle.Sizable && main.MaximizeBox, "Resizable/maximize controls disabled");
        var contentSplit = Field<SplitContainer>(main, "splitContainer1");
        var headerSplit = Field<SplitContainer>(main, "splitContainer2");
        var initialClient = main.ClientSize;
        var initialEditor = Field<PKMEditor>(main, "PKME_Tabs").Size;
        int headerHeight = headerSplit.Panel1.Height;
        var baseFontSize = Field<PKMEditor>(main, "PKME_Tabs").Font.Size;
        var baseIconSize = Field<PictureBox>(main, "PB_Legal").Size;
        main.ClientSize = new System.Drawing.Size(initialClient.Width + 300, initialClient.Height + 180);
        main.PerformLayout();
        Check(Field<PKMEditor>(main, "PKME_Tabs").Height > initialEditor.Height && Field<float>(main, "WholeWindowScale") > 1, "Manual resizing did not enlarge the full interface");
        Check(headerSplit.Panel1.Height > headerHeight, "Manual resizing did not enlarge the menu");
        main.ClientSize = new System.Drawing.Size((int)Math.Round(initialClient.Width * 1.5), (int)Math.Round(initialClient.Height * 1.5));
        var nativeFont = baseFontSize;
        var nativeIcon = baseIconSize;
        var applyZoom = typeof(Main).GetMethod("ApplyWholeWindowZoom", BindingFlags.NonPublic | BindingFlags.Instance)!;
        applyZoom.Invoke(main, [1.5f]);
        Check(Math.Abs(Field<PKMEditor>(main, "PKME_Tabs").Font.Size / nativeFont - 1.5f) < 0.01 && Field<PictureBox>(main, "PB_Legal").Width == (int)Math.Round(nativeIcon.Width * 1.5), "Exact 150-percent text/image scale failed");
        applyZoom.Invoke(main, [1f]);
        main.ClientSize = initialClient;
        main.Opacity = 0;
        main.ShowInTaskbar = false;
        main.Show();
        float originalFont = Field<PKMEditor>(main, "PKME_Tabs").Font.Size;
        var originalImageSize = Field<PictureBox>(main, "PB_Legal").Size;
        main.WindowState = FormWindowState.Maximized;
        Application.DoEvents();
        var workingArea = Screen.FromControl(main).WorkingArea;
        var clientOnScreen = main.RectangleToScreen(main.ClientRectangle);
        Check(main.WindowState == FormWindowState.Maximized && workingArea.Contains(clientOnScreen), "Maximized content covered the taskbar/work-area boundary");
        float zoom = Field<float>(main, "WholeWindowScale");
        Check(zoom > 1 && Math.Abs(Field<PKMEditor>(main, "PKME_Tabs").Font.Size / originalFont - zoom) < 0.03, "Text did not scale with maximization");
        Check(Math.Abs((float)Field<PictureBox>(main, "PB_Legal").Width / originalImageSize.Width - zoom) < 0.08, "Images did not scale with maximization");
        Check(headerSplit.Panel1.Height > headerHeight, "Menu failed to enlarge with maximization");
        if (args.Length > 0)
        {
            using var scaledImage = new System.Drawing.Bitmap(headerSplit.Width, headerSplit.Height);
            headerSplit.DrawToBitmap(scaledImage, headerSplit.ClientRectangle);
            scaledImage.Save(args[0]);
        }
        main.WindowState = FormWindowState.Normal;
        Application.DoEvents();
        main.Hide();
        Check(main.ClientSize == initialClient, "Restore did not retain the normal window size");
        Check(Math.Abs(Field<PKMEditor>(main, "PKME_Tabs").Font.Size - originalFont) < 0.01 && Field<PictureBox>(main, "PB_Legal").Size == originalImageSize, "Restore retained enlarged text/images");
        Console.WriteLine("Whole-interface maximize scale, text, images, menu, working-area boundary and restore: PASS");
        var save = new SAV4HGSS { Version = GameVersion.HG, OT = "PLAYER", Language = 2 };
        GameInfo.FilteredSources = new FilteredGameDataSource(save, GameInfo.Sources, false);
        var reset = typeof(Main).GetMethod("ResetSAVPKMEditors", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!;
        Console.WriteLine("Resetting save editor...");
        reset.Invoke(main, [save]);
        Console.WriteLine("Save editor reset completed.");
        var menu = Field<ToolStripMenuItem>(main, "Menu_PsyduckYellow");
        menu.Checked = true;
        Check(save.IsPsyduckYellow0029, "Built-in profile menu failed");
        var editor = Field<PKMEditor>(main, "PKME_Tabs");
        var ability = Field<ComboBox>(editor, "CB_Ability");
        main.Show();
        main.WindowState = FormWindowState.Maximized;
        Application.DoEvents();
        Console.WriteLine("Resetting save editor...");
        reset.Invoke(main, [save]);
        Console.WriteLine("Save editor reset completed.");
        Check(Field<float>(main, "WholeWindowScale") > 1, "Loading a save lost maximized scaling");
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
        if (args.Length > 0)
        {
            using var loadedImage = new System.Drawing.Bitmap(headerSplit.Width, headerSplit.Height);
            headerSplit.DrawToBitmap(loadedImage, headerSplit.ClientRectangle);
            loadedImage.Save(args[0]);
        }
        main.WindowState = FormWindowState.Normal;
        Application.DoEvents();
        main.Hide();
        Check(Math.Abs(editor.Font.Size - originalFont) < 0.01, "Save-load/restore introduced font drift");
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
        if (args.Length > 1)
        {
            using var image = new System.Drawing.Bitmap(main.Width, main.Height);
            main.DrawToBitmap(image, main.ClientRectangle);
            image.Save(args[0]);
        }
        Console.WriteLine("Full PKHeX form, built-in profile, all 12 supported HAs, shiny display, inventory and retail isolation: PASS");
    }
}
