using System;
using System.Windows.Forms;

namespace AutoModPlugins.GUI
{
    partial class SimpleHexEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            B_Update = new Button();
            PG_BlockView = new PropertyGrid();
            CB_AutoRefresh = new CheckBox();
            CB_CopyMethod = new ComboBox();
            RTB_RAM = new HexRichTextBox();
            RT_Label = new Label();
            RT_Timer = new NumericUpDown();
            B_Load = new Button();
            B_Save = new Button();
            ((System.ComponentModel.ISupportInitialize)RT_Timer).BeginInit();
            SuspendLayout();
            // 
            // B_Update
            // 
            B_Update.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            B_Update.Location = new System.Drawing.Point(10, 355);
            B_Update.Margin = new Padding(4, 3, 4, 3);
            B_Update.Name = "B_Update";
            B_Update.Size = new System.Drawing.Size(133, 30);
            B_Update.TabIndex = 1;
            B_Update.Text = "Update";
            B_Update.UseVisualStyleBackColor = true;
            B_Update.Click += Update_Click;
            // 
            // PG_BlockView
            // 
            PG_BlockView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PG_BlockView.BackColor = System.Drawing.SystemColors.Control;
            PG_BlockView.Location = new System.Drawing.Point(10, 12);
            PG_BlockView.Margin = new Padding(4, 3, 4, 3);
            PG_BlockView.Name = "PG_BlockView";
            PG_BlockView.Size = new System.Drawing.Size(133, 150);
            PG_BlockView.TabIndex = 15;
            PG_BlockView.Visible = false;
            // 
            // CB_AutoRefresh
            // 
            CB_AutoRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CB_AutoRefresh.AutoSize = true;
            CB_AutoRefresh.Location = new System.Drawing.Point(381, 363);
            CB_AutoRefresh.Margin = new Padding(4, 3, 4, 3);
            CB_AutoRefresh.Name = "CB_AutoRefresh";
            CB_AutoRefresh.Size = new System.Drawing.Size(65, 19);
            CB_AutoRefresh.TabIndex = 16;
            CB_AutoRefresh.Text = "Refresh";
            CB_AutoRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            CB_AutoRefresh.UseVisualStyleBackColor = true;
            CB_AutoRefresh.CheckedChanged += CB_AutoRefresh_CheckedChanged;
            // 
            // CB_CopyMethod
            // 
            CB_CopyMethod.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CB_CopyMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_CopyMethod.FormattingEnabled = true;
            CB_CopyMethod.Location = new System.Drawing.Point(153, 360);
            CB_CopyMethod.Margin = new Padding(4, 3, 4, 3);
            CB_CopyMethod.Name = "CB_CopyMethod";
            CB_CopyMethod.Size = new System.Drawing.Size(107, 23);
            CB_CopyMethod.TabIndex = 17;
            CB_CopyMethod.SelectedIndexChanged += ChangeCopyMethod;
            // 
            // RTB_RAM
            // 
            RTB_RAM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RTB_RAM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            RTB_RAM.Location = new System.Drawing.Point(10, 12);
            RTB_RAM.Margin = new Padding(4, 3, 4, 3);
            RTB_RAM.Name = "RTB_RAM";
            RTB_RAM.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            RTB_RAM.Size = new System.Drawing.Size(363, 336);
            RTB_RAM.TabIndex = 0;
            RTB_RAM.Text = "";
            // 
            // RT_Label
            // 
            RT_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            RT_Label.AutoSize = true;
            RT_Label.Location = new System.Drawing.Point(264, 365);
            RT_Label.Margin = new Padding(4, 0, 4, 0);
            RT_Label.Name = "RT_Label";
            RT_Label.Size = new System.Drawing.Size(37, 15);
            RT_Label.TabIndex = 19;
            RT_Label.Text = "Timer";
            // 
            // RT_Timer
            // 
            RT_Timer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            RT_Timer.Location = new System.Drawing.Point(304, 360);
            RT_Timer.Margin = new Padding(4, 3, 4, 3);
            RT_Timer.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            RT_Timer.Name = "RT_Timer";
            RT_Timer.Size = new System.Drawing.Size(61, 23);
            RT_Timer.TabIndex = 20;
            RT_Timer.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            RT_Timer.ValueChanged += RT_Timer_ValueChanged;
            // 
            // B_Load
            // 
            B_Load.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            B_Load.Location = new System.Drawing.Point(381, 12);
            B_Load.Margin = new Padding(4, 3, 4, 3);
            B_Load.Name = "B_Load";
            B_Load.Size = new System.Drawing.Size(65, 30);
            B_Load.TabIndex = 21;
            B_Load.Text = "Import";
            B_Load.UseVisualStyleBackColor = true;
            B_Load.Click += B_Load_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(381, 44);
            B_Save.Margin = new Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(65, 30);
            B_Save.TabIndex = 22;
            B_Save.Text = "Export";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // SimpleHexEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(448, 396);
            Controls.Add(B_Save);
            Controls.Add(B_Load);
            Controls.Add(RT_Timer);
            Controls.Add(RT_Label);
            Controls.Add(CB_CopyMethod);
            Controls.Add(CB_AutoRefresh);
            Controls.Add(PG_BlockView);
            Controls.Add(B_Update);
            Controls.Add(RTB_RAM);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(464, 435);
            Name = "SimpleHexEditor";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "RAMEdit";
            FormClosing += SimpleHexEditor_FormClosing;
            ((System.ComponentModel.ISupportInitialize)RT_Timer).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        private HexRichTextBox RTB_RAM;
        private Button B_Update;
        public PropertyGrid PG_BlockView;
        private CheckBox CB_AutoRefresh;
        private ComboBox CB_CopyMethod;
        private Label RT_Label;
        private NumericUpDown RT_Timer;
        private Button B_Load;
        private Button B_Save;
    }
}
