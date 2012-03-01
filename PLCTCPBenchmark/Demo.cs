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

namespace PLCTCPBenchmark
{
    public partial class Demo : Form
    {
        private class PLCEntry
        {
            public IPAddress IP;
            public string MAC;
            public PLCEntry(IPAddress ip, string mac)
            {
                IP = ip;
                MAC = mac;
            }

            public override string ToString()
            {
                return IP + " (" + MAC + ")";
            }
        }

        Finder m_finder;

        public Demo()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            m_finder = new Finder();
            m_finder.DeviceFound += new Finder.DeviceFoundHandler(m_finder_DeviceFound);
        }

        void m_finder_DeviceFound(string mac, IPAddress ip)
        {
            // The following code block is required because this method will be
            // called from another thread. To access GUI components we must send this
            // even to GUI thread.
            if (InvokeRequired)
            {
                Invoke((Action<string, IPAddress>) m_finder_DeviceFound, new object[]{mac, ip});
                return;
            }
            scanLB.Items.Add(new PLCEntry(ip, mac));
        }

        // Draw texts on the side handles horizontall (instead of the default vertical)
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.White);
                g.FillRectangle(Brushes.Gray, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void scanB_Click(object sender, EventArgs e)
        {
            m_finder.Scan();
        }

        private void scanAddB_Click(object sender, EventArgs e)
        {
            if (scanPassTB.Text.Length == 0)
            {
                MessageBox.Show("Please enter a password");
                return;
            }
            object o = scanLB.SelectedItem;
            if(o == null)
            {
                MessageBox.Show("Please select a PLC first");
                return;
            }
            PLCEntry p = (PLCEntry)o;
            plcCB.Items.Add(new PLC(p.IP, scanPassTB.Text));
            plcCB.SelectedIndex = plcCB.Items.Count - 1;
        }

        private void addB_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress a = IPAddress.Parse(ipTB.Text);
                ushort port = (ushort) portNUD.Value;
                IPEndPoint ip = new IPEndPoint(a, port);
                if (passTB.Text.Length == 0)
                    throw new Exception("Please enter a password");

                plcCB.Items.Add(new PLC(ip, passTB.Text));
                plcCB.SelectedIndex = plcCB.Items.Count - 1;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    

    }
}
