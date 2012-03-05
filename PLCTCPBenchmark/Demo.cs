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
using System.Security.Authentication;
using ENDA.PLCNetLib.Diagnostics;
using Be.Windows.Forms;
using PLCTCPBenchmark.Properties;
using System.IO;
using System.Text.RegularExpressions;

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
            tabControl1_SelectedIndexChanged(tabControl1, EventArgs.Empty);
            LogManager.LogFired += new LogManager.LogHandler(LogManager_LogFired);
            m_finder = new Finder();
            m_finder.DeviceFound += new Finder.DeviceFoundHandler(m_finder_DeviceFound);
            rwHB.StringViewVisible = true;
            rwHB.LineInfoVisible = true;
            rwHB.UseFixedBytesPerLine = true;
            rwHB.BytesPerLine = 16;
            rwHB.VScrollBarVisible = true;
            readWriteP.Controls.Add(rwHB);
            DynamicByteProvider dbp = new DynamicByteProvider(new byte[1]);
            dbp.LengthChanged +=new EventHandler(writeRawLengthChangedHandler);
            rwHB.ByteProvider = dbp;

        }

        void LogManager_LogFired(LogManager.Level lvl, DateTime t, string source, string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogManager.LogHandler(LogManager_LogFired), new object[] { lvl, t, source, msg });
                return;
            }
            logTB.AppendText("[" + t.ToString("HH:mm:ss.fff") + "] [" + lvl + "] [" + source + "] " + msg + "\r\n");
        }

        void m_finder_DeviceFound(string mac, IPAddress ip)
        {
            // The following code block is required because this method will be
            // called from another thread. To access GUI components we must send this
            // even to GUI thread.
            if (InvokeRequired)
            {
                Invoke((Action<string, IPAddress>)m_finder_DeviceFound, new object[] { mac, ip });
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
            if (o == null)
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
                ushort port = (ushort)portNUD.Value;
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

        private void connectB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            connectStatusL.Text = "Connecting synchronously...";
            Application.DoEvents();
            try
            {
                p.Connect();
                connectStatusL.Text = "Connected synchronously!";
            }
            catch (InvalidCredentialException exc)
            {
                connectStatusL.Text = "Invalid password";
            }
            catch (Exception exc)
            {
                connectStatusL.Text = "Error: " + exc.Message;
            }
        }

        void asyncConnectHandler(IAsyncResult ar)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AsyncCallback(asyncConnectHandler), new object[] { ar });
                return;
            }
            PLC p = (PLC)ar.AsyncState;
            try
            {
                p.EndConnect(ar);
                connectStatusL.Text = "Connected asynchronously!";
            }
            catch (InvalidCredentialException exc)
            {
                connectStatusL.Text = "Invalid password";
            }
            catch (Exception exc)
            {
                connectStatusL.Text = "Error: " + exc.Message;
            }

        }

        private void asyncConnectB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            connectStatusL.Text = "Connecting asynchronously...";
            p.BeginConnect(new AsyncCallback(asyncConnectHandler), p);
        }

        private void miReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)miOffsetNUD.Value;
            miReadL.Text = p.MI[offset].ToString();
        }

        private void miWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                int offset = (int)miOffsetNUD.Value;
                p.MI[offset] = Int32.Parse(miWriteTB.Text);
                miWriteL.Text = "Success";
            }
            catch (Exception exc)
            {
                miWriteL.Text = "Error: " + exc.Message;
            }
        }

        private void mfReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)mfOffsetNUD.Value;
            mfReadL.Text = p.MF[offset].ToString();
        }

        private void mfWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                int offset = (int)mfOffsetNUD.Value;
                p.MF[offset] = Single.Parse(mfWriteTB.Text);
                mfWriteL.Text = "Success";
            }
            catch (Exception exc)
            {
                mfWriteL.Text = "Error: " + exc.Message;
            }
        }

        private void mbReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)mbOffsetNUD.Value;
            mbReadL.Text = p.MB[offset].ToString();
        }

        private void mbWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                int offset = (int)mbOffsetNUD.Value;
                p.MB[offset] = mbWriteCB.Checked;
                mbWriteL.Text = "Success";
            }
            catch (Exception exc)
            {
                mbWriteL.Text = "Error: " + exc.Message;
            }
        }

        private void mwReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)mwOffsetNUD.Value;
            mwReadL.Text = p.MW[offset].ToString();
        }

        private void mwWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                int offset = (int)mwOffsetNUD.Value;
                p.MW[offset] = ushort.Parse(mwWriteTB.Text);
                mwWriteL.Text = "Success";
            }
            catch (Exception exc)
            {
                mwWriteL.Text = "Error: " + exc.Message;
            }
        }

        private void ipReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)ipOffsetNUD.Value;
            ipReadL.Text = p.IP[offset] ? "ON" : "OFF";
        }

        private void qpReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)qpOffsetNUD.Value;
            qpReadL.Text = p.QP[offset].ToString();
        }

        private void qpWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                int offset = (int)qpOffsetNUD.Value;
                p.QP[offset] = qpWriteCB.Checked;
                qpWriteL.Text = "Success";
            }
            catch (Exception exc)
            {
                qpWriteL.Text = "Error: " + exc.Message;
            }
        }

        private void timeReadB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                timeReadL.Text = p.Time.ToString();
            }
            catch (InvalidOperationException exc)
            {
                timeReadL.Text = "This device does not have RTC hardware";
            }
        }

        private void timeWriteB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            try
            {
                p.Time = timeWriteDTP.Value;
            }
            catch (InvalidOperationException exc)
            {
                timeReadL.Text = "This device does not have RTC hardware";
            }
        }

        private void runB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            runL.Text = p.Run() ? "OK" : "FAIL";
        }

        private void stopB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            stopL.Text = p.Stop() ? "OK" : "FAIL";
        }

        private void writeRawOffsetNUD_ValueChanged(object sender, EventArgs e)
        {
            rwHB.LineInfoOffset = (long)rwOffsetNUD.Value;
        }

        void writeRawLengthChangedHandler(object sender, EventArgs e)
        {
            rwLenNUD.Value = ((DynamicByteProvider)sender).Length;
        }

        private void writeRawB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            DynamicByteProvider dbp = (DynamicByteProvider)rwHB.ByteProvider;
            byte[] data = dbp.Bytes.ToArray();
            int offset = (int)rwOffsetNUD.Value;
            rwStatusL.Text = "Writing synchronously...";
            Application.DoEvents(); // Force GUI update
            p.Write(offset, data);
            rwStatusL.Text = "Done";
        }

        private void writeAsyncHandler(IAsyncResult ar)
        {
            if (InvokeRequired)
            {
                // We'll update GUI, so we need to pass the async call back processing
                // to the GUI thread.
                BeginInvoke(new AsyncCallback(writeAsyncHandler), new object[]{ar});
                return;
            }
            PLC p = (PLC)ar.AsyncState;
            try
            {
                p.EndWrite(ar);
                rwStatusL.Text = "Done";
            }
            catch (IndexOutOfRangeException exc)
            {
                rwStatusL.Text = "Out of range or max request capacity";
            }
            catch (Exception exc)
            {
                rwStatusL.Text = "Unexpected error: " + exc.Message;
            }
        }

        private void writeAsyncB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            DynamicByteProvider dbp = (DynamicByteProvider)rwHB.ByteProvider;
            byte[] data = dbp.Bytes.ToArray();
            int offset = (int)rwOffsetNUD.Value;
            rwStatusL.Text = "Writing asynchronously...";
            p.BeginWriteRaw(offset, data, new AsyncCallback(writeAsyncHandler), p);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string name = tabControl1.SelectedTab.Text;
                string file = String.Empty;
                foreach (char ch in name)
                    file += Char.IsLetterOrDigit(ch) ? ch : '_';
                helpRTB.LoadFile("docs/" + file + ".rtf");
            }
            catch (Exception exc)
            {
                helpRTB.Text = "Could not load help: " + exc.Message;
            }
        }

        private void readB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)rwOffsetNUD.Value;
            int len = (int)rwLenNUD.Value;
            BinaryReader br = p.Read(offset, len);
            rwHB.ByteProvider = new DynamicByteProvider(new byte[len]);
            rwHB.ByteProvider.LengthChanged += new EventHandler(writeRawLengthChangedHandler);
            for (int i = 0; i < len; i++)
                rwHB.ByteProvider.WriteByte(i, br.ReadByte());
        }

        private void readAsyncHandler(IAsyncResult ar)
        {
            if (InvokeRequired)
            {
                // We'll update GUI, so we need to pass the async call back processing
                // to the GUI thread.
                BeginInvoke(new AsyncCallback(readAsyncHandler), new object[] { ar });
                return;
            }
            PLC p = (PLC)ar.AsyncState;
            BinaryReader br;

            try
            {
                br = p.EndRead(ar);
                rwStatusL.Text = "Done";

                rwHB.ByteProvider = new DynamicByteProvider(new byte[br.BaseStream.Length]);
                for (int i = 0; i < br.BaseStream.Length; i++)
                    rwHB.ByteProvider.WriteByte(i, br.ReadByte());
            }
            catch (IndexOutOfRangeException exc)
            {
                rwStatusL.Text = "Out of range or max request capacity";
            }
            catch (Exception exc)
            {
                rwStatusL.Text = "Unexpected error: " + exc.Message;
            }
        }

        private void readAsyncB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            int offset = (int)rwOffsetNUD.Value;
            int len = (int)rwLenNUD.Value;
            p.BeginRead(offset, len, new AsyncCallback(readAsyncHandler), p);
        }

        private void cmdAsyncHandler(IAsyncResult ar)
        {
            if (InvokeRequired)
            {
                // We'll update GUI, so we need to pass the async call back processing
                // to the GUI thread.
                BeginInvoke(new AsyncCallback(cmdAsyncHandler), new object[] { ar });
                return;
            }
            PLC p = (PLC)ar.AsyncState;
            Response resp = p.EndCmd(ar);
            // Let's convert UNIX line endings to Windows.
            string respStr = resp.String.Replace("\n", "\r\n");
            // And append a new line, just in case.
            respStr += "\r\n";
            cmdRespTB.AppendText(respStr);
        }

        private List<String> m_cmdHist = new List<string>();
        private int m_cmdIdx = 0;
        private void cmdTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PLC p = (PLC)plcCB.SelectedItem;
                if (p == null) return;
                if (cmdAsyncCB.Checked)
                {
                    p.BeginCmd(ASCIIEncoding.ASCII.GetBytes(cmdTB.Text), new AsyncCallback(cmdAsyncHandler), p);
                }
                else
                {
                    Response resp = p.Cmd(cmdTB.Text);
                    // Let's convert UNIX line endings to Windows.
                    string respStr = resp.String.Replace("\n", "\r\n");
                    // And append a new line, just in case.
                    respStr += "\r\n";
                    cmdRespTB.AppendText(respStr);
                }
                m_cmdHist.Add(cmdTB.Text);
                cmdTB.Text = "";
                m_cmdIdx = 0;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (m_cmdIdx < m_cmdHist.Count)
                {
                    cmdTB.Text = m_cmdHist[m_cmdHist.Count - ++m_cmdIdx];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (m_cmdIdx > 1)
                {
                    cmdTB.Text = m_cmdHist[m_cmdHist.Count - --m_cmdIdx];
                }
                else
                {
                    m_cmdIdx = 0;
                    cmdTB.Text = "";
                }
            }
        }

        private void readMultiB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;

            int rows = readMultiDGV.Rows.Count - 1;
            if (rows < 1)
            {
                MessageBox.Show("Please at least enter 1 offset");
                return;
            }
            ushort[] offsets = new ushort[rows];
            try
            {
                for (int i = 0; i < offsets.Length; i++)
                {
                    offsets[i] = UInt16.Parse((string)readMultiDGV.Rows[i].Cells[0].Value);
                }
                BinaryReader br = p.ReadMulti(offsets);
                for (int i = 0; i < offsets.Length; i++)
                {
                    readMultiDGV.Rows[i].Cells[1].Value = br.ReadInt32();
                }
            }
            catch (Exception exc)
            {
                readMultiStatusL.Text = exc.Message;
            }
        }
    }
}
