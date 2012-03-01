namespace PLCTCPBenchmark
{
    partial class Form1
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
            this.plcLB = new System.Windows.Forms.ListBox();
            this.addB = new System.Windows.Forms.Button();
            this.ipTB = new System.Windows.Forms.TextBox();
            this.passTB = new System.Windows.Forms.TextBox();
            this.runB = new System.Windows.Forms.Button();
            this.stopB = new System.Windows.Forms.Button();
            this.readB = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.miAddrNUD = new System.Windows.Forms.NumericUpDown();
            this.valueTB = new System.Windows.Forms.TextBox();
            this.valL = new System.Windows.Forms.Label();
            this.writeB = new System.Windows.Forms.Button();
            this.massWriteB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.miAddrNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // plcLB
            // 
            this.plcLB.FormattingEnabled = true;
            this.plcLB.Location = new System.Drawing.Point(12, 53);
            this.plcLB.Name = "plcLB";
            this.plcLB.Size = new System.Drawing.Size(248, 95);
            this.plcLB.TabIndex = 0;
            // 
            // addB
            // 
            this.addB.Location = new System.Drawing.Point(185, 24);
            this.addB.Name = "addB";
            this.addB.Size = new System.Drawing.Size(75, 23);
            this.addB.TabIndex = 1;
            this.addB.Text = "Ekle";
            this.addB.UseVisualStyleBackColor = true;
            this.addB.Click += new System.EventHandler(this.addB_Click);
            // 
            // ipTB
            // 
            this.ipTB.Location = new System.Drawing.Point(27, 26);
            this.ipTB.Name = "ipTB";
            this.ipTB.Size = new System.Drawing.Size(74, 20);
            this.ipTB.TabIndex = 3;
            this.ipTB.Text = "78.171.23.73";
            // 
            // passTB
            // 
            this.passTB.Location = new System.Drawing.Point(139, 26);
            this.passTB.Name = "passTB";
            this.passTB.PasswordChar = '*';
            this.passTB.Size = new System.Drawing.Size(39, 20);
            this.passTB.TabIndex = 4;
            this.passTB.Text = "4321";
            // 
            // runB
            // 
            this.runB.Location = new System.Drawing.Point(266, 57);
            this.runB.Name = "runB";
            this.runB.Size = new System.Drawing.Size(75, 23);
            this.runB.TabIndex = 5;
            this.runB.Text = "Run";
            this.runB.UseVisualStyleBackColor = true;
            this.runB.Click += new System.EventHandler(this.runB_Click);
            // 
            // stopB
            // 
            this.stopB.Location = new System.Drawing.Point(266, 86);
            this.stopB.Name = "stopB";
            this.stopB.Size = new System.Drawing.Size(75, 23);
            this.stopB.TabIndex = 6;
            this.stopB.Text = "Stop";
            this.stopB.UseVisualStyleBackColor = true;
            this.stopB.Click += new System.EventHandler(this.stopB_Click);
            // 
            // readB
            // 
            this.readB.Location = new System.Drawing.Point(15, 227);
            this.readB.Name = "readB";
            this.readB.Size = new System.Drawing.Size(60, 23);
            this.readB.TabIndex = 7;
            this.readB.Text = "Oku";
            this.readB.UseVisualStyleBackColor = true;
            this.readB.Click += new System.EventHandler(this.readB_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "MI Address:";
            // 
            // miAddrNUD
            // 
            this.miAddrNUD.Location = new System.Drawing.Point(81, 166);
            this.miAddrNUD.Name = "miAddrNUD";
            this.miAddrNUD.Size = new System.Drawing.Size(60, 20);
            this.miAddrNUD.TabIndex = 10;
            // 
            // valueTB
            // 
            this.valueTB.Location = new System.Drawing.Point(15, 201);
            this.valueTB.Name = "valueTB";
            this.valueTB.Size = new System.Drawing.Size(60, 20);
            this.valueTB.TabIndex = 11;
            // 
            // valL
            // 
            this.valL.AutoSize = true;
            this.valL.Location = new System.Drawing.Point(90, 232);
            this.valL.Name = "valL";
            this.valL.Size = new System.Drawing.Size(0, 13);
            this.valL.TabIndex = 12;
            // 
            // writeB
            // 
            this.writeB.Location = new System.Drawing.Point(81, 199);
            this.writeB.Name = "writeB";
            this.writeB.Size = new System.Drawing.Size(60, 23);
            this.writeB.TabIndex = 13;
            this.writeB.Text = "Yaz";
            this.writeB.UseVisualStyleBackColor = true;
            this.writeB.Click += new System.EventHandler(this.writeB_Click);
            // 
            // massWriteB
            // 
            this.massWriteB.Location = new System.Drawing.Point(266, 116);
            this.massWriteB.Name = "massWriteB";
            this.massWriteB.Size = new System.Drawing.Size(75, 23);
            this.massWriteB.TabIndex = 14;
            this.massWriteB.Text = "Çok Yaz";
            this.massWriteB.UseVisualStyleBackColor = true;
            this.massWriteB.Click += new System.EventHandler(this.massWriteB_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Şifre";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 258);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.massWriteB);
            this.Controls.Add(this.writeB);
            this.Controls.Add(this.valL);
            this.Controls.Add(this.valueTB);
            this.Controls.Add(this.miAddrNUD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.readB);
            this.Controls.Add(this.stopB);
            this.Controls.Add(this.runB);
            this.Controls.Add(this.passTB);
            this.Controls.Add(this.ipTB);
            this.Controls.Add(this.addB);
            this.Controls.Add(this.plcLB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.miAddrNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox plcLB;
        private System.Windows.Forms.Button addB;
        private System.Windows.Forms.TextBox ipTB;
        private System.Windows.Forms.TextBox passTB;
        private System.Windows.Forms.Button runB;
        private System.Windows.Forms.Button stopB;
        private System.Windows.Forms.Button readB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown miAddrNUD;
        private System.Windows.Forms.TextBox valueTB;
        private System.Windows.Forms.Label valL;
        private System.Windows.Forms.Button writeB;
        private System.Windows.Forms.Button massWriteB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

