﻿using System;
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
            LogManager.LogFired += new LogManager.LogHandler(LogManager_LogFired);
            m_finder = new Finder();
            m_finder.DeviceFound += new Finder.DeviceFoundHandler(m_finder_DeviceFound);

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
            connectStatusL.Text = "Connecting...";
            Application.DoEvents();
            try
            {
                p.Connect();
                connectStatusL.Text = "Connected!";
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
                asyncConnectL.Text = "Connected";
            }
            catch (InvalidCredentialException exc)
            {
                asyncConnectL.Text = "Invalid password";
            }
            catch (Exception exc)
            {
                asyncConnectL.Text = "Error: " + exc.Message;
            }

        }

        private void asyncConnectB_Click(object sender, EventArgs e)
        {
            PLC p = (PLC)plcCB.SelectedItem;
            if (p == null) return;
            asyncConnectL.Text = "Connecting...";
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


    }
}