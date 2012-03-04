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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace PLCTCPBenchmark
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void addB_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(ipTB.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            plcLB.Items.Add(new PLC(new IPEndPoint(ip, 23), passTB.Text));
        }

        private void ResponseHandler(IAsyncResult ar)
        {
            PLC plc = (PLC)ar.AsyncState;
            Response resp = plc.EndCmd(ar);
            if (resp.String.Contains("OK!"))
            {
                MessageBox.Show("Run command succesfull");
            }
            else
            {
                MessageBox.Show("Run command failed");
            }
        }

        private void runB_Click(object sender, EventArgs e)
        {
            Object i = plcLB.SelectedItem;
            if (i == null) return;
            PLC plc = (PLC)i;

            plc.BeginCmd(ASCIIEncoding.ASCII.GetBytes("run"), new AsyncCallback(ResponseHandler), plc);
        }

        private void stopB_Click(object sender, EventArgs e)
        {
            Object i = plcLB.SelectedItem;
            if (i == null) return;
            PLC plc = (PLC)i;
            plc.Stop();
        }

        private void writeB_Click(object sender, EventArgs e)
        {
            Object i = plcLB.SelectedItem;
            if (i == null) return;
            PLC plc = (PLC)i;
            int addr = (int)miAddrNUD.Value;
            plc.MI[addr] = Int32.Parse(valueTB.Text);
        }

        private void readB_Click(object sender, EventArgs e)
        {
            Object i = plcLB.SelectedItem;
            if (i == null) return;
            PLC plc = (PLC)i;
            int addr = (int)miAddrNUD.Value;
            valL.Text = plc.MI[addr].ToString();
        }

        void WriteRawAsyncHandler(IAsyncResult ar)
        {
            AsyncResult a = (AsyncResult)ar;
            PLC p = (PLC)a.AsyncState;
            p.EndWrite(ar);
        }

        private void massWriteB_Click(object sender, EventArgs e)
        {
            Object o = plcLB.SelectedItem;
            if (o == null) return;
            PLC plc = (PLC)o;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            for (int i = 0; i < 100; i++)
                bw.Write(i*2);
            
            plc.BeginWriteRaw(0, ms.GetBuffer(), new AsyncCallback(WriteRawAsyncHandler), plc);
        }
    }
}
