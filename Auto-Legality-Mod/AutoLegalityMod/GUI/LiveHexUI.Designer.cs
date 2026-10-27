namespace AutoModPlugins
{
    partial class LiveHeXUI
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
            tabPage1 = new System.Windows.Forms.TabPage();
            groupBox5 = new System.Windows.Forms.GroupBox();
            CB_BlockName = new System.Windows.Forms.ComboBox();
            B_EditBlock = new System.Windows.Forms.Button();
            L_Block = new System.Windows.Forms.Label();
            GB_Boxes = new System.Windows.Forms.GroupBox();
            checkBox2 = new System.Windows.Forms.CheckBox();
            CB_ReadBox = new System.Windows.Forms.CheckBox();
            B_ReadCurrent = new System.Windows.Forms.Button();
            B_WriteCurrent = new System.Windows.Forms.Button();
            B_ReadOffset = new System.Windows.Forms.Button();
            connectionMode = new System.Windows.Forms.ComboBox();
            B_Connect = new System.Windows.Forms.Button();
            TB_IP = new System.Windows.Forms.TextBox();
            TB_Port = new System.Windows.Forms.TextBox();
            L_USBState = new System.Windows.Forms.Label();
            B_Disconnect = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage2 = new System.Windows.Forms.TabPage();
            groupBox4 = new System.Windows.Forms.GroupBox();
            B_ReadPointer = new System.Windows.Forms.Button();
            B_CopyAddress = new System.Windows.Forms.Button();
            B_EditPointer = new System.Windows.Forms.Button();
            TB_Pointer = new HexTextBox();
            L_Pointer = new System.Windows.Forms.Label();
            groupBox6 = new System.Windows.Forms.GroupBox();
            RB_Absolute = new System.Windows.Forms.RadioButton();
            RamSize = new System.Windows.Forms.TextBox();
            RB_Main = new System.Windows.Forms.RadioButton();
            L_ReadRamSize = new System.Windows.Forms.Label();
            RB_Heap = new System.Windows.Forms.RadioButton();
            L_OffsRelative = new System.Windows.Forms.Label();
            groupBox3 = new System.Windows.Forms.GroupBox();
            B_ReadRAM = new System.Windows.Forms.Button();
            RamOffset = new HexTextBox();
            L_ReadRamOffset = new System.Windows.Forms.Label();
            tabPage1.SuspendLayout();
            groupBox5.SuspendLayout();
            GB_Boxes.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage1
            // 
            tabPage1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            tabPage1.Controls.Add(groupBox5);
            tabPage1.Controls.Add(GB_Boxes);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Margin = new System.Windows.Forms.Padding(0);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new System.Drawing.Size(396, 220);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Basic";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(CB_BlockName);
            groupBox5.Controls.Add(B_EditBlock);
            groupBox5.Controls.Add(L_Block);
            groupBox5.Enabled = false;
            groupBox5.Location = new System.Drawing.Point(4, 103);
            groupBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox5.Size = new System.Drawing.Size(385, 91);
            groupBox5.TabIndex = 13;
            groupBox5.TabStop = false;
            groupBox5.Text = "Block Editor";
            // 
            // CB_BlockName
            // 
            CB_BlockName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BlockName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            CB_BlockName.Location = new System.Drawing.Point(53, 22);
            CB_BlockName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_BlockName.Name = "CB_BlockName";
            CB_BlockName.Size = new System.Drawing.Size(224, 22);
            CB_BlockName.Sorted = true;
            CB_BlockName.TabIndex = 22;
            // 
            // B_EditBlock
            // 
            B_EditBlock.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_EditBlock.Location = new System.Drawing.Point(52, 50);
            B_EditBlock.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_EditBlock.Name = "B_EditBlock";
            B_EditBlock.Size = new System.Drawing.Size(84, 28);
            B_EditBlock.TabIndex = 21;
            B_EditBlock.Text = "Edit Block";
            B_EditBlock.UseVisualStyleBackColor = true;
            B_EditBlock.Click += B_EditBlock_Click;
            // 
            // L_Block
            // 
            L_Block.Location = new System.Drawing.Point(8, 22);
            L_Block.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Block.Name = "L_Block";
            L_Block.Size = new System.Drawing.Size(40, 23);
            L_Block.TabIndex = 17;
            L_Block.Text = "Block:";
            L_Block.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GB_Boxes
            // 
            GB_Boxes.Controls.Add(checkBox2);
            GB_Boxes.Controls.Add(CB_ReadBox);
            GB_Boxes.Controls.Add(B_ReadCurrent);
            GB_Boxes.Controls.Add(B_WriteCurrent);
            GB_Boxes.Enabled = false;
            GB_Boxes.ForeColor = System.Drawing.SystemColors.ControlText;
            GB_Boxes.Location = new System.Drawing.Point(4, 6);
            GB_Boxes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Boxes.Name = "GB_Boxes";
            GB_Boxes.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Boxes.Size = new System.Drawing.Size(385, 91);
            GB_Boxes.TabIndex = 8;
            GB_Boxes.TabStop = false;
            GB_Boxes.Text = "Boxes";
            // 
            // checkBox2
            // 
            checkBox2.Checked = true;
            checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox2.Location = new System.Drawing.Point(165, 22);
            checkBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(146, 25);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "Inject In Slot On Set";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // CB_ReadBox
            // 
            CB_ReadBox.Checked = true;
            CB_ReadBox.CheckState = System.Windows.Forms.CheckState.Checked;
            CB_ReadBox.Location = new System.Drawing.Point(15, 22);
            CB_ReadBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_ReadBox.Name = "CB_ReadBox";
            CB_ReadBox.Size = new System.Drawing.Size(146, 25);
            CB_ReadBox.TabIndex = 2;
            CB_ReadBox.Text = "Read On Change Box";
            CB_ReadBox.UseVisualStyleBackColor = true;
            // 
            // B_ReadCurrent
            // 
            B_ReadCurrent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_ReadCurrent.Location = new System.Drawing.Point(11, 53);
            B_ReadCurrent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ReadCurrent.Name = "B_ReadCurrent";
            B_ReadCurrent.Size = new System.Drawing.Size(146, 27);
            B_ReadCurrent.TabIndex = 0;
            B_ReadCurrent.Text = "Read Current Box";
            B_ReadCurrent.UseVisualStyleBackColor = true;
            B_ReadCurrent.Click += B_ReadCurrent_Click;
            // 
            // B_WriteCurrent
            // 
            B_WriteCurrent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_WriteCurrent.Location = new System.Drawing.Point(165, 53);
            B_WriteCurrent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_WriteCurrent.Name = "B_WriteCurrent";
            B_WriteCurrent.Size = new System.Drawing.Size(146, 27);
            B_WriteCurrent.TabIndex = 1;
            B_WriteCurrent.Text = "Write Current Box";
            B_WriteCurrent.UseVisualStyleBackColor = true;
            B_WriteCurrent.Click += B_WriteCurrent_Click;
            // 
            // B_ReadOffset
            // 
            B_ReadOffset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_ReadOffset.Location = new System.Drawing.Point(36, 80);
            B_ReadOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ReadOffset.Name = "B_ReadOffset";
            B_ReadOffset.Size = new System.Drawing.Size(102, 27);
            B_ReadOffset.TabIndex = 13;
            B_ReadOffset.Text = "Read Pokémon";
            B_ReadOffset.UseVisualStyleBackColor = true;
            B_ReadOffset.Click += B_ReadOffset_Click;
            // 
            // connectionMode
            // 
            connectionMode.BackColor = System.Drawing.SystemColors.Window;
            connectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            connectionMode.FormattingEnabled = true;
            connectionMode.Items.AddRange(new object[] { "WiFi", "USB" });
            connectionMode.Location = new System.Drawing.Point(201, 7);
            connectionMode.Name = "connectionMode";
            connectionMode.Size = new System.Drawing.Size(121, 23);
            connectionMode.TabIndex = 16;
            connectionMode.SelectedIndexChanged += ConnectionMode_SelectedIndexChanged;
            // 
            // B_Connect
            // 
            B_Connect.BackColor = System.Drawing.Color.Transparent;
            B_Connect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_Connect.Location = new System.Drawing.Point(329, 6);
            B_Connect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Connect.Name = "B_Connect";
            B_Connect.Size = new System.Drawing.Size(74, 26);
            B_Connect.TabIndex = 7;
            B_Connect.Text = "Connect";
            B_Connect.UseVisualStyleBackColor = false;
            B_Connect.Click += B_Connect_Click;
            // 
            // TB_IP
            // 
            TB_IP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_IP.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_IP.Location = new System.Drawing.Point(14, 8);
            TB_IP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_IP.Name = "TB_IP";
            TB_IP.Size = new System.Drawing.Size(128, 20);
            TB_IP.TabIndex = 3;
            TB_IP.Text = "111.111.111.111";
            // 
            // TB_Port
            // 
            TB_Port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Port.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_Port.Location = new System.Drawing.Point(150, 8);
            TB_Port.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Port.Name = "TB_Port";
            TB_Port.ReadOnly = true;
            TB_Port.Size = new System.Drawing.Size(48, 20);
            TB_Port.TabIndex = 5;
            TB_Port.Text = "6000";
            // 
            // L_USBState
            // 
            L_USBState.AutoSize = true;
            L_USBState.Location = new System.Drawing.Point(33, 9);
            L_USBState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_USBState.Name = "L_USBState";
            L_USBState.Size = new System.Drawing.Size(109, 15);
            L_USBState.TabIndex = 11;
            L_USBState.Text = "USB-Botbase Mode";
            L_USBState.Visible = false;
            // 
            // B_Disconnect
            // 
            B_Disconnect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_Disconnect.Location = new System.Drawing.Point(330, 6);
            B_Disconnect.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            B_Disconnect.Name = "B_Disconnect";
            B_Disconnect.Size = new System.Drawing.Size(74, 26);
            B_Disconnect.TabIndex = 15;
            B_Disconnect.Text = "Disconnect";
            B_Disconnect.UseVisualStyleBackColor = true;
            B_Disconnect.Visible = false;
            B_Disconnect.Click += B_Disconnect_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new System.Drawing.Point(3, 34);
            tabControl1.Margin = new System.Windows.Forms.Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(404, 248);
            tabControl1.TabIndex = 16;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            tabPage2.Controls.Add(groupBox4);
            tabPage2.Controls.Add(groupBox6);
            tabPage2.Controls.Add(groupBox3);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(396, 220);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Advanced";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(B_ReadPointer);
            groupBox4.Controls.Add(B_CopyAddress);
            groupBox4.Controls.Add(B_EditPointer);
            groupBox4.Controls.Add(TB_Pointer);
            groupBox4.Controls.Add(L_Pointer);
            groupBox4.Enabled = false;
            groupBox4.Location = new System.Drawing.Point(8, 127);
            groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox4.Size = new System.Drawing.Size(378, 84);
            groupBox4.TabIndex = 12;
            groupBox4.TabStop = false;
            groupBox4.Text = "Pointer Lookup";
            // 
            // B_ReadPointer
            // 
            B_ReadPointer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_ReadPointer.Location = new System.Drawing.Point(130, 50);
            B_ReadPointer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ReadPointer.Name = "B_ReadPointer";
            B_ReadPointer.Size = new System.Drawing.Size(110, 27);
            B_ReadPointer.TabIndex = 23;
            B_ReadPointer.Text = "Read Pokémon";
            B_ReadPointer.UseVisualStyleBackColor = true;
            B_ReadPointer.Click += B_ReadPointer_Click;
            // 
            // B_CopyAddress
            // 
            B_CopyAddress.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_CopyAddress.Location = new System.Drawing.Point(248, 50);
            B_CopyAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CopyAddress.Name = "B_CopyAddress";
            B_CopyAddress.Size = new System.Drawing.Size(110, 27);
            B_CopyAddress.TabIndex = 22;
            B_CopyAddress.Text = "Copy Address";
            B_CopyAddress.UseVisualStyleBackColor = true;
            B_CopyAddress.Click += B_CopyAddress_Click;
            // 
            // B_EditPointer
            // 
            B_EditPointer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_EditPointer.Location = new System.Drawing.Point(12, 50);
            B_EditPointer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_EditPointer.Name = "B_EditPointer";
            B_EditPointer.Size = new System.Drawing.Size(110, 27);
            B_EditPointer.TabIndex = 21;
            B_EditPointer.Text = "Edit RAM";
            B_EditPointer.UseVisualStyleBackColor = true;
            B_EditPointer.Click += B_EditPointerData_Click;
            // 
            // TB_Pointer
            // 
            TB_Pointer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            TB_Pointer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            TB_Pointer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Pointer.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_Pointer.Location = new System.Drawing.Point(68, 21);
            TB_Pointer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Pointer.Name = "TB_Pointer";
            TB_Pointer.Size = new System.Drawing.Size(290, 20);
            TB_Pointer.TabIndex = 18;
            // 
            // L_Pointer
            // 
            L_Pointer.Location = new System.Drawing.Point(7, 19);
            L_Pointer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Pointer.Name = "L_Pointer";
            L_Pointer.Size = new System.Drawing.Size(53, 23);
            L_Pointer.TabIndex = 17;
            L_Pointer.Text = "Pointer:";
            L_Pointer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(RB_Absolute);
            groupBox6.Controls.Add(RamSize);
            groupBox6.Controls.Add(RB_Main);
            groupBox6.Controls.Add(L_ReadRamSize);
            groupBox6.Controls.Add(RB_Heap);
            groupBox6.Controls.Add(L_OffsRelative);
            groupBox6.Enabled = false;
            groupBox6.Location = new System.Drawing.Point(176, 6);
            groupBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox6.Size = new System.Drawing.Size(210, 115);
            groupBox6.TabIndex = 14;
            groupBox6.TabStop = false;
            groupBox6.Text = "RAM Config";
            // 
            // RB_Absolute
            // 
            RB_Absolute.AutoSize = true;
            RB_Absolute.Location = new System.Drawing.Point(136, 42);
            RB_Absolute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RB_Absolute.Name = "RB_Absolute";
            RB_Absolute.Size = new System.Drawing.Size(72, 19);
            RB_Absolute.TabIndex = 3;
            RB_Absolute.Text = "Absolute";
            RB_Absolute.UseVisualStyleBackColor = true;
            // 
            // RamSize
            // 
            RamSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            RamSize.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            RamSize.Location = new System.Drawing.Point(61, 67);
            RamSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RamSize.MaxLength = 8;
            RamSize.Name = "RamSize";
            RamSize.Size = new System.Drawing.Size(63, 20);
            RamSize.TabIndex = 20;
            RamSize.Text = "344";
            // 
            // RB_Main
            // 
            RB_Main.AutoSize = true;
            RB_Main.ForeColor = System.Drawing.SystemColors.ControlText;
            RB_Main.Location = new System.Drawing.Point(76, 42);
            RB_Main.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RB_Main.Name = "RB_Main";
            RB_Main.Size = new System.Drawing.Size(52, 19);
            RB_Main.TabIndex = 2;
            RB_Main.Text = "Main";
            RB_Main.UseVisualStyleBackColor = true;
            // 
            // L_ReadRamSize
            // 
            L_ReadRamSize.Location = new System.Drawing.Point(8, 64);
            L_ReadRamSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ReadRamSize.Name = "L_ReadRamSize";
            L_ReadRamSize.Size = new System.Drawing.Size(45, 23);
            L_ReadRamSize.TabIndex = 19;
            L_ReadRamSize.Text = "Bytes:";
            L_ReadRamSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RB_Heap
            // 
            RB_Heap.AutoSize = true;
            RB_Heap.Checked = true;
            RB_Heap.Location = new System.Drawing.Point(15, 42);
            RB_Heap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RB_Heap.Name = "RB_Heap";
            RB_Heap.Size = new System.Drawing.Size(53, 19);
            RB_Heap.TabIndex = 1;
            RB_Heap.TabStop = true;
            RB_Heap.Text = "Heap";
            RB_Heap.UseVisualStyleBackColor = true;
            // 
            // L_OffsRelative
            // 
            L_OffsRelative.AutoSize = true;
            L_OffsRelative.Location = new System.Drawing.Point(15, 24);
            L_OffsRelative.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OffsRelative.Name = "L_OffsRelative";
            L_OffsRelative.Size = new System.Drawing.Size(129, 15);
            L_OffsRelative.TabIndex = 0;
            L_OffsRelative.Text = "RAM offsets relative to:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(B_ReadRAM);
            groupBox3.Controls.Add(B_ReadOffset);
            groupBox3.Controls.Add(RamOffset);
            groupBox3.Controls.Add(L_ReadRamOffset);
            groupBox3.Enabled = false;
            groupBox3.Location = new System.Drawing.Point(8, 6);
            groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Size = new System.Drawing.Size(160, 115);
            groupBox3.TabIndex = 10;
            groupBox3.TabStop = false;
            groupBox3.Text = "RAM Editor";
            // 
            // B_ReadRAM
            // 
            B_ReadRAM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_ReadRAM.Location = new System.Drawing.Point(36, 47);
            B_ReadRAM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ReadRAM.Name = "B_ReadRAM";
            B_ReadRAM.Size = new System.Drawing.Size(102, 27);
            B_ReadRAM.TabIndex = 21;
            B_ReadRAM.Text = "Edit RAM";
            B_ReadRAM.UseVisualStyleBackColor = true;
            B_ReadRAM.Click += B_ReadRAM_Click;
            // 
            // RamOffset
            // 
            RamOffset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            RamOffset.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            RamOffset.Location = new System.Drawing.Point(68, 21);
            RamOffset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RamOffset.MaxLength = 16;
            RamOffset.Name = "RamOffset";
            RamOffset.Size = new System.Drawing.Size(81, 20);
            RamOffset.TabIndex = 18;
            RamOffset.Text = "2E32206A";
            // 
            // L_ReadRamOffset
            // 
            L_ReadRamOffset.Location = new System.Drawing.Point(12, 20);
            L_ReadRamOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ReadRamOffset.Name = "L_ReadRamOffset";
            L_ReadRamOffset.Size = new System.Drawing.Size(49, 23);
            L_ReadRamOffset.TabIndex = 17;
            L_ReadRamOffset.Text = "Offset:";
            L_ReadRamOffset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LiveHeXUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(409, 285);
            Controls.Add(tabControl1);
            Controls.Add(connectionMode);
            Controls.Add(TB_Port);
            Controls.Add(L_USBState);
            Controls.Add(TB_IP);
            Controls.Add(B_Connect);
            Controls.Add(B_Disconnect);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LiveHeXUI";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "LiveHeXUI";
            FormClosing += LiveHeXUI_FormClosing;
            tabPage1.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            GB_Boxes.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox connectionMode;
        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.TextBox TB_Port;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox CB_BlockName;
        private System.Windows.Forms.Button B_EditBlock;
        private System.Windows.Forms.Label L_Block;
        private System.Windows.Forms.Label L_USBState;
        private System.Windows.Forms.Button B_Connect;
        private System.Windows.Forms.GroupBox GB_Boxes;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox CB_ReadBox;
        private System.Windows.Forms.Button B_ReadCurrent;
        private System.Windows.Forms.Button B_WriteCurrent;
        private System.Windows.Forms.Button B_Disconnect;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button B_ReadOffset;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button B_ReadPointer;
        private System.Windows.Forms.Button B_CopyAddress;
        private System.Windows.Forms.Button B_EditPointer;
        private HexTextBox TB_Pointer;
        private System.Windows.Forms.Label L_Pointer;
        private System.Windows.Forms.RadioButton RB_Absolute;
        private System.Windows.Forms.RadioButton RB_Main;
        private System.Windows.Forms.RadioButton RB_Heap;
        private System.Windows.Forms.Label L_OffsRelative;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button B_ReadRAM;
        private System.Windows.Forms.TextBox RamSize;
        private System.Windows.Forms.Label L_ReadRamSize;
        private HexTextBox RamOffset;
        private System.Windows.Forms.Label L_ReadRamOffset;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}
