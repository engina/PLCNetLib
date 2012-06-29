using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ENDA.PLCNetLib;
using System.Net;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PLC plc;

        private void button1_Click(object sender, EventArgs e)
        {
            int n = (int)numericUpDown1.Value;
            for (int i = 0; i < n; i++)
                plc.MW[0] = (ushort)(i % 65535);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            plc = new PLC(IPAddress.Parse("192.168.1.96"), "1234");
            plc.Connect();

            plc.Time = DateTime.Now;
        }
    }
}
