namespace PLCTCPBenchmark
{
    partial class Demo
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.miP = new System.Windows.Forms.TabPage();
            this.mfP = new System.Windows.Forms.TabPage();
            this.mbP = new System.Windows.Forms.TabPage();
            this.ipP = new System.Windows.Forms.TabPage();
            this.qpP = new System.Windows.Forms.TabPage();
            this.timeP = new System.Windows.Forms.TabPage();
            this.runstopP = new System.Windows.Forms.TabPage();
            this.writeRawP = new System.Windows.Forms.TabPage();
            this.readP = new System.Windows.Forms.TabPage();
            this.readMultiP = new System.Windows.Forms.TabPage();
            this.Cmd = new System.Windows.Forms.TabPage();
            this.asyncWriteRawP = new System.Windows.Forms.TabPage();
            this.asyncReadP = new System.Windows.Forms.TabPage();
            this.asyncReadMultiP = new System.Windows.Forms.TabPage();
            this.asyncCmdP = new System.Windows.Forms.TabPage();
            this.mwP = new System.Windows.Forms.TabPage();
            this.connectP = new System.Windows.Forms.TabPage();
            this.asyncConnectP = new System.Windows.Forms.TabPage();
            this.finderP = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.plcCB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ipTB = new System.Windows.Forms.TextBox();
            this.passTB = new System.Windows.Forms.TextBox();
            this.addB = new System.Windows.Forms.Button();
            this.removeB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.scanLB = new System.Windows.Forms.ListBox();
            this.scanB = new System.Windows.Forms.Button();
            this.scanAddB = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.scanPassTB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.portNUD = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.finderP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.portNUD);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.removeB);
            this.groupBox1.Controls.Add(this.addB);
            this.groupBox1.Controls.Add(this.passTB);
            this.groupBox1.Controls.Add(this.ipTB);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.plcCB);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 50);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pick a PLC to work on";
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.finderP);
            this.tabControl1.Controls.Add(this.connectP);
            this.tabControl1.Controls.Add(this.asyncConnectP);
            this.tabControl1.Controls.Add(this.miP);
            this.tabControl1.Controls.Add(this.mfP);
            this.tabControl1.Controls.Add(this.mbP);
            this.tabControl1.Controls.Add(this.mwP);
            this.tabControl1.Controls.Add(this.ipP);
            this.tabControl1.Controls.Add(this.qpP);
            this.tabControl1.Controls.Add(this.timeP);
            this.tabControl1.Controls.Add(this.runstopP);
            this.tabControl1.Controls.Add(this.writeRawP);
            this.tabControl1.Controls.Add(this.readP);
            this.tabControl1.Controls.Add(this.readMultiP);
            this.tabControl1.Controls.Add(this.Cmd);
            this.tabControl1.Controls.Add(this.asyncWriteRawP);
            this.tabControl1.Controls.Add(this.asyncReadP);
            this.tabControl1.Controls.Add(this.asyncReadMultiP);
            this.tabControl1.Controls.Add(this.asyncCmdP);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(30, 100);
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(616, 575);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 21;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            // 
            // miP
            // 
            this.miP.Location = new System.Drawing.Point(104, 4);
            this.miP.Name = "miP";
            this.miP.Padding = new System.Windows.Forms.Padding(3);
            this.miP.Size = new System.Drawing.Size(370, 567);
            this.miP.TabIndex = 0;
            this.miP.Text = "MI";
            this.miP.UseVisualStyleBackColor = true;
            // 
            // mfP
            // 
            this.mfP.Location = new System.Drawing.Point(104, 4);
            this.mfP.Name = "mfP";
            this.mfP.Padding = new System.Windows.Forms.Padding(3);
            this.mfP.Size = new System.Drawing.Size(370, 567);
            this.mfP.TabIndex = 1;
            this.mfP.Text = "MF";
            this.mfP.UseVisualStyleBackColor = true;
            // 
            // mbP
            // 
            this.mbP.Location = new System.Drawing.Point(104, 4);
            this.mbP.Name = "mbP";
            this.mbP.Padding = new System.Windows.Forms.Padding(3);
            this.mbP.Size = new System.Drawing.Size(370, 567);
            this.mbP.TabIndex = 2;
            this.mbP.Text = "MB";
            this.mbP.UseVisualStyleBackColor = true;
            // 
            // ipP
            // 
            this.ipP.Location = new System.Drawing.Point(104, 4);
            this.ipP.Name = "ipP";
            this.ipP.Padding = new System.Windows.Forms.Padding(3);
            this.ipP.Size = new System.Drawing.Size(370, 567);
            this.ipP.TabIndex = 3;
            this.ipP.Text = "IP";
            this.ipP.UseVisualStyleBackColor = true;
            // 
            // qpP
            // 
            this.qpP.Location = new System.Drawing.Point(104, 4);
            this.qpP.Name = "qpP";
            this.qpP.Padding = new System.Windows.Forms.Padding(3);
            this.qpP.Size = new System.Drawing.Size(370, 567);
            this.qpP.TabIndex = 4;
            this.qpP.Text = "QP";
            this.qpP.UseVisualStyleBackColor = true;
            // 
            // timeP
            // 
            this.timeP.Location = new System.Drawing.Point(104, 4);
            this.timeP.Name = "timeP";
            this.timeP.Padding = new System.Windows.Forms.Padding(3);
            this.timeP.Size = new System.Drawing.Size(370, 567);
            this.timeP.TabIndex = 5;
            this.timeP.Text = "Time";
            this.timeP.UseVisualStyleBackColor = true;
            // 
            // runstopP
            // 
            this.runstopP.Location = new System.Drawing.Point(104, 4);
            this.runstopP.Name = "runstopP";
            this.runstopP.Padding = new System.Windows.Forms.Padding(3);
            this.runstopP.Size = new System.Drawing.Size(370, 567);
            this.runstopP.TabIndex = 6;
            this.runstopP.Text = "Run/Stop";
            this.runstopP.UseVisualStyleBackColor = true;
            // 
            // writeRawP
            // 
            this.writeRawP.Location = new System.Drawing.Point(104, 4);
            this.writeRawP.Name = "writeRawP";
            this.writeRawP.Padding = new System.Windows.Forms.Padding(3);
            this.writeRawP.Size = new System.Drawing.Size(370, 567);
            this.writeRawP.TabIndex = 7;
            this.writeRawP.Text = "WriteRaw";
            this.writeRawP.UseVisualStyleBackColor = true;
            // 
            // readP
            // 
            this.readP.Location = new System.Drawing.Point(104, 4);
            this.readP.Name = "readP";
            this.readP.Padding = new System.Windows.Forms.Padding(3);
            this.readP.Size = new System.Drawing.Size(370, 567);
            this.readP.TabIndex = 8;
            this.readP.Text = "Read";
            this.readP.UseVisualStyleBackColor = true;
            // 
            // readMultiP
            // 
            this.readMultiP.Location = new System.Drawing.Point(104, 4);
            this.readMultiP.Name = "readMultiP";
            this.readMultiP.Padding = new System.Windows.Forms.Padding(3);
            this.readMultiP.Size = new System.Drawing.Size(370, 567);
            this.readMultiP.TabIndex = 9;
            this.readMultiP.Text = "ReadMulti";
            this.readMultiP.UseVisualStyleBackColor = true;
            // 
            // Cmd
            // 
            this.Cmd.Location = new System.Drawing.Point(104, 4);
            this.Cmd.Name = "Cmd";
            this.Cmd.Padding = new System.Windows.Forms.Padding(3);
            this.Cmd.Size = new System.Drawing.Size(370, 567);
            this.Cmd.TabIndex = 10;
            this.Cmd.Text = "Cmd";
            this.Cmd.UseVisualStyleBackColor = true;
            // 
            // asyncWriteRawP
            // 
            this.asyncWriteRawP.Location = new System.Drawing.Point(104, 4);
            this.asyncWriteRawP.Name = "asyncWriteRawP";
            this.asyncWriteRawP.Padding = new System.Windows.Forms.Padding(3);
            this.asyncWriteRawP.Size = new System.Drawing.Size(370, 567);
            this.asyncWriteRawP.TabIndex = 11;
            this.asyncWriteRawP.Text = "Async WriteRaw";
            this.asyncWriteRawP.UseVisualStyleBackColor = true;
            // 
            // asyncReadP
            // 
            this.asyncReadP.Location = new System.Drawing.Point(104, 4);
            this.asyncReadP.Name = "asyncReadP";
            this.asyncReadP.Padding = new System.Windows.Forms.Padding(3);
            this.asyncReadP.Size = new System.Drawing.Size(370, 567);
            this.asyncReadP.TabIndex = 12;
            this.asyncReadP.Text = "Async Read";
            this.asyncReadP.UseVisualStyleBackColor = true;
            // 
            // asyncReadMultiP
            // 
            this.asyncReadMultiP.Location = new System.Drawing.Point(104, 4);
            this.asyncReadMultiP.Name = "asyncReadMultiP";
            this.asyncReadMultiP.Padding = new System.Windows.Forms.Padding(3);
            this.asyncReadMultiP.Size = new System.Drawing.Size(370, 567);
            this.asyncReadMultiP.TabIndex = 13;
            this.asyncReadMultiP.Text = "Async ReadMulti";
            this.asyncReadMultiP.UseVisualStyleBackColor = true;
            // 
            // asyncCmdP
            // 
            this.asyncCmdP.Location = new System.Drawing.Point(104, 4);
            this.asyncCmdP.Name = "asyncCmdP";
            this.asyncCmdP.Padding = new System.Windows.Forms.Padding(3);
            this.asyncCmdP.Size = new System.Drawing.Size(370, 567);
            this.asyncCmdP.TabIndex = 14;
            this.asyncCmdP.Text = "Async Cmd";
            this.asyncCmdP.UseVisualStyleBackColor = true;
            // 
            // mwP
            // 
            this.mwP.Location = new System.Drawing.Point(104, 4);
            this.mwP.Name = "mwP";
            this.mwP.Padding = new System.Windows.Forms.Padding(3);
            this.mwP.Size = new System.Drawing.Size(370, 567);
            this.mwP.TabIndex = 15;
            this.mwP.Text = "MW";
            this.mwP.UseVisualStyleBackColor = true;
            // 
            // connectP
            // 
            this.connectP.Location = new System.Drawing.Point(104, 4);
            this.connectP.Name = "connectP";
            this.connectP.Size = new System.Drawing.Size(370, 567);
            this.connectP.TabIndex = 16;
            this.connectP.Text = "Connect";
            this.connectP.UseVisualStyleBackColor = true;
            // 
            // asyncConnectP
            // 
            this.asyncConnectP.Location = new System.Drawing.Point(104, 4);
            this.asyncConnectP.Name = "asyncConnectP";
            this.asyncConnectP.Size = new System.Drawing.Size(370, 567);
            this.asyncConnectP.TabIndex = 17;
            this.asyncConnectP.Text = "Async Connect";
            this.asyncConnectP.UseVisualStyleBackColor = true;
            // 
            // finderP
            // 
            this.finderP.Controls.Add(this.label5);
            this.finderP.Controls.Add(this.scanPassTB);
            this.finderP.Controls.Add(this.label4);
            this.finderP.Controls.Add(this.scanAddB);
            this.finderP.Controls.Add(this.scanB);
            this.finderP.Controls.Add(this.scanLB);
            this.finderP.Location = new System.Drawing.Point(104, 4);
            this.finderP.Name = "finderP";
            this.finderP.Size = new System.Drawing.Size(508, 567);
            this.finderP.TabIndex = 18;
            this.finderP.Text = "Finder";
            this.finderP.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 625);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(616, 83);
            this.textBox1.TabIndex = 0;
            // 
            // plcCB
            // 
            this.plcCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.plcCB.FormattingEnabled = true;
            this.plcCB.Location = new System.Drawing.Point(43, 19);
            this.plcCB.Name = "plcCB";
            this.plcCB.Size = new System.Drawing.Size(108, 21);
            this.plcCB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "PLC";
            // 
            // ipTB
            // 
            this.ipTB.Location = new System.Drawing.Point(244, 19);
            this.ipTB.Name = "ipTB";
            this.ipTB.Size = new System.Drawing.Size(76, 20);
            this.ipTB.TabIndex = 2;
            // 
            // passTB
            // 
            this.passTB.Location = new System.Drawing.Point(456, 19);
            this.passTB.Name = "passTB";
            this.passTB.Size = new System.Drawing.Size(70, 20);
            this.passTB.TabIndex = 3;
            // 
            // addB
            // 
            this.addB.Location = new System.Drawing.Point(531, 18);
            this.addB.Name = "addB";
            this.addB.Size = new System.Drawing.Size(39, 23);
            this.addB.TabIndex = 4;
            this.addB.Text = "Add";
            this.addB.UseVisualStyleBackColor = true;
            this.addB.Click += new System.EventHandler(this.addB_Click);
            // 
            // removeB
            // 
            this.removeB.Location = new System.Drawing.Point(157, 18);
            this.removeB.Name = "removeB";
            this.removeB.Size = new System.Drawing.Size(63, 23);
            this.removeB.TabIndex = 5;
            this.removeB.Text = "Remove";
            this.removeB.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "IP:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(420, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Pass:";
            // 
            // scanLB
            // 
            this.scanLB.FormattingEnabled = true;
            this.scanLB.Location = new System.Drawing.Point(13, 38);
            this.scanLB.Name = "scanLB";
            this.scanLB.Size = new System.Drawing.Size(213, 95);
            this.scanLB.TabIndex = 0;
            // 
            // scanB
            // 
            this.scanB.Location = new System.Drawing.Point(89, 9);
            this.scanB.Name = "scanB";
            this.scanB.Size = new System.Drawing.Size(61, 23);
            this.scanB.TabIndex = 1;
            this.scanB.Text = "Scan";
            this.scanB.UseVisualStyleBackColor = true;
            this.scanB.Click += new System.EventHandler(this.scanB_Click);
            // 
            // scanAddB
            // 
            this.scanAddB.Location = new System.Drawing.Point(151, 198);
            this.scanAddB.Name = "scanAddB";
            this.scanAddB.Size = new System.Drawing.Size(75, 23);
            this.scanAddB.TabIndex = 2;
            this.scanAddB.Text = "Add";
            this.scanAddB.UseVisualStyleBackColor = true;
            this.scanAddB.Click += new System.EventHandler(this.scanAddB_Click);
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Location = new System.Drawing.Point(0, 541);
            this.label4.MaximumSize = new System.Drawing.Size(500, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(485, 26);
            this.label4.TabIndex = 3;
            this.label4.Text = "This is a demomnstration of ENDA.PLCNetLib.Finder class. It scans the local netwo" +
    "rk for online PLCs and then you can add them to the combobox at top.";
            // 
            // scanPassTB
            // 
            this.scanPassTB.Location = new System.Drawing.Point(13, 200);
            this.scanPassTB.Name = "scanPassTB";
            this.scanPassTB.Size = new System.Drawing.Size(120, 20);
            this.scanPassTB.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(396, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Select a PLC from the list and write its password in the textbox below and click " +
    "add";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(325, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Port:";
            // 
            // portNUD
            // 
            this.portNUD.Location = new System.Drawing.Point(356, 19);
            this.portNUD.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.portNUD.Name = "portNUD";
            this.portNUD.Size = new System.Drawing.Size(56, 20);
            this.portNUD.TabIndex = 9;
            this.portNUD.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 708);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(632, 746);
            this.Name = "Demo";
            this.Text = "Demo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.finderP.ResumeLayout(false);
            this.finderP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage miP;
        private System.Windows.Forms.TabPage mfP;
        private System.Windows.Forms.TabPage mbP;
        private System.Windows.Forms.TabPage ipP;
        private System.Windows.Forms.TabPage qpP;
        private System.Windows.Forms.TabPage timeP;
        private System.Windows.Forms.TabPage runstopP;
        private System.Windows.Forms.TabPage writeRawP;
        private System.Windows.Forms.TabPage readP;
        private System.Windows.Forms.TabPage readMultiP;
        private System.Windows.Forms.TabPage Cmd;
        private System.Windows.Forms.TabPage asyncWriteRawP;
        private System.Windows.Forms.TabPage asyncReadP;
        private System.Windows.Forms.TabPage asyncReadMultiP;
        private System.Windows.Forms.TabPage asyncCmdP;
        private System.Windows.Forms.TabPage mwP;
        private System.Windows.Forms.TabPage connectP;
        private System.Windows.Forms.TabPage asyncConnectP;
        private System.Windows.Forms.TabPage finderP;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox plcCB;
        private System.Windows.Forms.Button addB;
        private System.Windows.Forms.TextBox passTB;
        private System.Windows.Forms.TextBox ipTB;
        private System.Windows.Forms.Button removeB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button scanAddB;
        private System.Windows.Forms.Button scanB;
        private System.Windows.Forms.ListBox scanLB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox scanPassTB;
        private System.Windows.Forms.NumericUpDown portNUD;
        private System.Windows.Forms.Label label6;
    }
}