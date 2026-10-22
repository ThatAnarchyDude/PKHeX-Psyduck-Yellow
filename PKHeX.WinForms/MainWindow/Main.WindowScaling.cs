using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace PKHeX.WinForms;

public partial class Main
{
    private Panel? ZoomViewport;
    private Size ZoomBaseSize;
    private List<ZoomControlState>? ZoomStates;
    private readonly Dictionary<(Font Font, float Scale), Font> ZoomFonts = [];
    private bool ApplyingWindowZoom;
    private float WholeWindowScale = 1;

    private void InitializeWholeWindowScaling()
    {
        ZoomBaseSize = ClientSize;
        ZoomViewport = new Panel { Dock = DockStyle.Fill, Name = "WholeWindowViewport" };
        Controls.Remove(splitContainer2);
        ZoomViewport.Controls.Add(splitContainer2);
        Controls.Add(ZoomViewport);
        Resize += (_, _) => UpdateWholeWindowScale();
        Disposed += (_, _) => { foreach (var font in ZoomFonts.Values) font.Dispose(); };
    }

    private void UpdateWholeWindowScale()
    {
        if (ZoomViewport is null || ApplyingWindowZoom) return;
        if (WindowState == FormWindowState.Minimized) return;
        float scale = Math.Min((float)ZoomViewport.ClientSize.Width / ZoomBaseSize.Width,
                               (float)ZoomViewport.ClientSize.Height / ZoomBaseSize.Height);
        ApplyWholeWindowZoom(Math.Max(1, scale));
    }

    private void RestoreZoomForLayoutChange()
    {
        if (ZoomStates is null) return;
        ApplyWholeWindowZoom(1);
        ZoomStates = null;
        foreach (var font in ZoomFonts.Values) font.Dispose();
        ZoomFonts.Clear();
    }

    private Font ZoomFont(Font original, float scale)
    {
        if (scale == 1) return original;
        var key = (original, scale);
        if (!ZoomFonts.TryGetValue(key, out var font))
            ZoomFonts[key] = font = new Font(original.FontFamily, original.Size * scale, original.Style, original.Unit);
        return font;
    }

    private void ApplyWholeWindowZoom(float scale)
    {
        if (ZoomViewport is null || ApplyingWindowZoom) return;
        ApplyingWindowZoom = true;
        try
        {
            splitContainer2.Dock = DockStyle.None;
            if (ZoomStates is null)
            {
                splitContainer2.Size = ZoomBaseSize;
                splitContainer2.PerformLayout();
                ZoomStates = CaptureZoomStates(splitContainer2).ToList();
            }
            foreach (var state in ZoomStates) state.Control.SuspendLayout();
            foreach (var state in ZoomStates)
            {
                var c = state.Control;
                if (c.IsDisposed) continue;
                if (c is ContainerControl container) container.AutoScaleMode = AutoScaleMode.None;
                c.AutoSize = false;
                c.MinimumSize = Size.Empty;
                c.MaximumSize = Size.Empty;
                c.Font = ZoomFont(state.Font, scale);
                c.Margin = ScalePadding(state.Margin, scale);
                c.Padding = ScalePadding(state.Padding, scale);
                if (c is not SplitterPanel) c.Bounds = ScaleRectangle(state.Bounds, scale);
                if (c is TableLayoutPanel table)
                {
                    for (int i=0;i<state.Columns.Length;i++)
                        if (table.ColumnStyles[i].SizeType == SizeType.Absolute) table.ColumnStyles[i].Width = state.Columns[i] * scale;
                    for (int i=0;i<state.Rows.Length;i++)
                        if (table.RowStyles[i].SizeType == SizeType.Absolute) table.RowStyles[i].Height = state.Rows[i] * scale;
                }
                if (c is SplitContainer split)
                {
                    split.Panel1MinSize = 0; split.Panel2MinSize = 0;
                    split.SplitterWidth = Math.Max(1, (int)Math.Round(state.SplitterWidth * scale));
                    split.SplitterDistance = (int)Math.Round(state.SplitterDistance * scale);
                    split.Panel1MinSize = (int)Math.Round(state.Panel1Minimum * scale);
                    split.Panel2MinSize = (int)Math.Round(state.Panel2Minimum * scale);
                }
                c.BackgroundImageLayout = scale == 1 ? state.BackgroundLayout : ImageLayout.Zoom;
                if (c is PictureBox picture) picture.SizeMode = scale == 1 ? state.PictureMode : PictureBoxSizeMode.Zoom;
                if (c is TabControl tabs)
                {
                    tabs.ItemSize = ScaleSize(state.TabSize, scale);
                    tabs.Padding = new Point((int)Math.Round(state.TabPadding.X * scale), (int)Math.Round(state.TabPadding.Y * scale));
                }
                if (c is ToolStrip strip)
                {
                    strip.ImageScalingSize = ScaleSize(state.ImageSize, scale);
                    foreach (var (item,font,imageSize,margin,padding) in state.MenuFonts)
                    {
                        item.Font = ZoomFont(font, scale);
                        item.Margin = ScalePadding(margin, scale);
                        item.Padding = ScalePadding(padding, scale);
                        if (item is ToolStripDropDownItem drop) drop.DropDown.ImageScalingSize = ScaleSize(imageSize, scale);
                    }
                }
                c.MinimumSize = ScaleSize(state.Minimum, scale);
                c.MaximumSize = ScaleSize(state.Maximum, scale);
                if (scale == 1)
                {
                    c.AutoSize = state.AutoSize;
                    if (c is ContainerControl original) original.AutoScaleMode = state.AutoScaleMode;
                }
            }
            for(int i=ZoomStates.Count-1;i>=0;i--) ZoomStates[i].Control.ResumeLayout(true);
            WholeWindowScale = scale;
            foreach (var key in ZoomFonts.Keys.Where(k => k.Scale != scale).ToArray())
            {
                ZoomFonts[key].Dispose();
                ZoomFonts.Remove(key);
            }
            splitContainer2.Location = new Point(Math.Max(0,(ZoomViewport.ClientSize.Width-splitContainer2.Width)/2),
                                                  Math.Max(0,(ZoomViewport.ClientSize.Height-splitContainer2.Height)/2));
        }
        finally { ApplyingWindowZoom = false; }
    }

    private static IEnumerable<ZoomControlState> CaptureZoomStates(Control control)
    {
        yield return new ZoomControlState(control);
        foreach(Control child in control.Controls)
            foreach(var state in CaptureZoomStates(child)) yield return state;
    }
    private static IEnumerable<(ToolStripItem Item,Font Font,Size ImageSize,Padding Margin,Padding Padding)> CaptureMenuFonts(ToolStripItemCollection items)
    {
        foreach(ToolStripItem item in items)
        {
            yield return (item,item.Font,item is ToolStripDropDownItem imageDrop ? imageDrop.DropDown.ImageScalingSize : Size.Empty,item.Margin,item.Padding);
            if(item is ToolStripDropDownItem drop)
                foreach(var child in CaptureMenuFonts(drop.DropDownItems)) yield return child;
        }
    }
    private static Size ScaleSize(Size size,float scale) => new((int)Math.Round(size.Width*scale),(int)Math.Round(size.Height*scale));
    private static Rectangle ScaleRectangle(Rectangle rect,float scale) => new((int)Math.Round(rect.X*scale),(int)Math.Round(rect.Y*scale),(int)Math.Round(rect.Width*scale),(int)Math.Round(rect.Height*scale));
    private static Padding ScalePadding(Padding p,float scale) => new((int)Math.Round(p.Left*scale),(int)Math.Round(p.Top*scale),(int)Math.Round(p.Right*scale),(int)Math.Round(p.Bottom*scale));
    private sealed class ZoomControlState(Control control)
    {
        public Control Control { get; } = control;
        public Rectangle Bounds { get; } = control.Bounds;
        public Font Font { get; } = control.Font;
        public Padding Margin { get; } = control.Margin;
        public Padding Padding { get; } = control.Padding;
        public Size Minimum { get; } = control.MinimumSize;
        public Size Maximum { get; } = control.MaximumSize;
        public bool AutoSize { get; } = control.AutoSize;
        public AutoScaleMode AutoScaleMode { get; } = control is ContainerControl c ? c.AutoScaleMode : AutoScaleMode.None;
        public float[] Columns { get; } = control is TableLayoutPanel t ? t.ColumnStyles.Cast<ColumnStyle>().Select(s=>s.Width).ToArray() : [];
        public float[] Rows { get; } = control is TableLayoutPanel t ? t.RowStyles.Cast<RowStyle>().Select(s=>s.Height).ToArray() : [];
        public int SplitterDistance { get; } = control is SplitContainer s ? s.SplitterDistance : 0;
        public ImageLayout BackgroundLayout { get; } = control.BackgroundImageLayout;
        public int Panel1Minimum { get; } = control is SplitContainer s ? s.Panel1MinSize : 0;
        public int Panel2Minimum { get; } = control is SplitContainer s ? s.Panel2MinSize : 0;
        public int SplitterWidth { get; } = control is SplitContainer s ? s.SplitterWidth : 1;
        public PictureBoxSizeMode PictureMode { get; } = control is PictureBox p ? p.SizeMode : PictureBoxSizeMode.Normal;
        public Size TabSize { get; } = control is TabControl t ? t.ItemSize : Size.Empty;
        public Point TabPadding { get; } = control is TabControl t ? t.Padding : Point.Empty;
        public Size ImageSize { get; } = control is ToolStrip t ? t.ImageScalingSize : Size.Empty;
        public (ToolStripItem Item,Font Font,Size ImageSize,Padding Margin,Padding Padding)[] MenuFonts { get; } = control is ToolStrip t ? CaptureMenuFonts(t.Items).ToArray() : [];
    }
}