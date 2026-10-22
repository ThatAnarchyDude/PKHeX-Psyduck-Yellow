using System;
using System.Windows.Forms;
using PKHeX.Core;
namespace PKHeX.WinForms;

public partial class Main
{
    private readonly ToolStripMenuItem Menu_PsyduckYellow = new("Psyduck Yellow prototype 0029") { CheckOnClick = true };
    private void InitializePsyduckYellowProfile()
    {
        Menu_Tools.DropDownItems.Add(new ToolStripSeparator());
        Menu_Tools.DropDownItems.Add(Menu_PsyduckYellow);
        Menu_Tools.DropDownOpening += (_, _) =>
        {
            Menu_PsyduckYellow.Enabled = C_SAV.SAV is SAV4HGSS { IsExpandedEngine: false };
            Menu_PsyduckYellow.Text = C_SAV.SAV is SAV4HGSS { IsExpandedEngine: true } expanded ? expanded.ProjectProfileName : "Psyduck Yellow prototype 0029";
            Menu_PsyduckYellow.Checked = C_SAV.SAV is SAV4HGSS { IsPsyduckYellow0029: true };
        };
        Menu_PsyduckYellow.CheckedChanged += (_, _) =>
        {
            if (C_SAV.SAV is not SAV4HGSS save || save.IsExpandedEngine || save.IsPsyduckYellow0029 == Menu_PsyduckYellow.Checked)
                return;
            // Preserve pending edits before changing the profile; enabling must retain raw HA values.
            var pk = PKME_Tabs.PrepareForPsyduckYellowProfile(Menu_PsyduckYellow.Checked);
            save.IsPsyduckYellow0029 = Menu_PsyduckYellow.Checked;
            if (pk is PK4 pk4)
                pk4.IsPsyduckYellow0029 = save.IsPsyduckYellow0029;
            PKME_Tabs.PopulateFields(pk, focus: false);
            C_SAV.ReloadSlots();
            Text = GetProgramTitle(save);
        };
    }
}
