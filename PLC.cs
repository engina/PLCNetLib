using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ENDAPLCNetLib
{
    public class PLC
    {
        TcpClient m_tcp = new TcpClient();
        IPAddress m_pip;
        string m_pass;
        byte[] m_lastCmd;
        IPAddress m_ip, m_nm, m_gw, m_dns;
        Logger log;

        public Int32ArrayAccessor MI;
        public FloatArrayAccessor MF;
        public BoolArrayAccessor MB;
        public BoolArrayAccessor QP;
        public BoolArrayAccessor IP;
        public UInt16ArrayAccessor MW;

        public PLC(IPAddress ip, string password)
        {
            log = new Logger("PLC[" + ip + "]");
            m_pip = ip;
            m_pass = password;
            MI = new Int32ArrayAccessor(this, 0, 4);
            MF = new FloatArrayAccessor(this, 4096, 4);
            MB = new BoolArrayAccessor(this, 8192, 1);
            QP = new BoolArrayAccessor(this, 9856, 1);
            IP = new BoolArrayAccessor(this, 10880, 1);
            MW = new UInt16ArrayAccessor(this, 16896, 2);
        }

        public void Connect()
        {
            m_tcp.Connect(m_pip, 23);
            ReadUntil("Password: ");
            Write(m_pass);
            string response = ReadUntil(new String[] { "Try again: ", "Password accepted\r\n> " }).String;
            if (!response.Contains("accepted"))
            {
                log.Error("Invalid password");
                m_tcp.Close();
                throw new Exception("Invalid password");
            }
        }


        public Response ReadUntil(String[] untils)
        {
            NetworkStream ns = m_tcp.GetStream();
            MemoryStream ms = new MemoryStream();
            bool found = false;
            while (!found)
            {
                int ch = ns.ReadByte();
                if (ch == -1)
                {
                    log.Error("Connection closed");
                    m_tcp.Close();
                    log.Info("Reconnecting");
                    Connect();
                    log.Info("Rewriting last command");
                    Rewrite();
                    return ReadUntil(untils);
                }
                ms.WriteByte((byte)ch);
                foreach (string until in untils)
                {
                    int len = until.Length;

                    if (ms.Position > (len - 1))
                    {
                        byte[] buf = new byte[len];
                        ms.Seek(-len, SeekOrigin.Current);
                        ms.Read(buf, 0, buf.Length);
                        string end = ASCIIEncoding.ASCII.GetString(buf);
                        if (end == until)
                            found = true;
                    }
                }
            }
            log.Debug("Recv: \"" + ASCIIEncoding.ASCII.GetString(ms.GetBuffer()));
            ms.Position = 0;
            return new Response(ms);
        }

        Response ReadUntil(string str)
        {
            return ReadUntil(new String[] { str });
        }

        Response ReadUntil()
        {
            return ReadUntil("\r\n> ");
        }

        void Rewrite()
        {
            Write(m_lastCmd);
        }

        void Write(byte[] data)
        {
            if (!m_tcp.Connected)
            {
                log.Warning("Write attempt while disconnected, trying to reconnect");
                Connect();
            }
            log.Debug("Sending \"" + ASCIIEncoding.ASCII.GetString(data));
            m_lastCmd = data;
            NetworkStream ns = m_tcp.GetStream();
            ns.Write(data, 0, data.Length);
        }

        void Write(string str)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(str));
        }

        Response Cmd(byte[] buf)
        {
            Write(buf);
            return ReadUntil();
        }

        Response Cmd(string cmd)
        {
            return Cmd(ASCIIEncoding.ASCII.GetBytes(cmd));
        }

        int IP2Int(IPAddress addr)
        {
            return BitConverter.ToInt32(addr.GetAddressBytes(), 0);
        }

        IPAddress Int2IP(int addr)
        {
            return new IPAddress(BitConverter.GetBytes(addr));
        }

        internal void UpdateNetwork()
        {
            string[] tokens = Cmd("ifconfig").String.Trim().Split(new char[] { ' ' });
            int ip = Int32.Parse(tokens[1].Substring(2), NumberStyles.HexNumber);
            int nm = Int32.Parse(tokens[3].Substring(2), NumberStyles.HexNumber);
            int gw = Int32.Parse(tokens[5].Substring(2), NumberStyles.HexNumber);
            int dns = Int32.Parse(tokens[7].Substring(2), NumberStyles.HexNumber);
            m_ip = Int2IP(ip);
            m_nm = Int2IP(nm);
            m_gw = Int2IP(gw);
            m_dns = Int2IP(dns);
        }

        public bool Run()
        {
            return Cmd("run").String.Contains("OK!");
        }

        public bool Stop()
        {
            return Cmd("stop").String.Contains("Task stopped");
        }

        public void Reboot()
        {
            Cmd("reboot");
        }

        public BinaryReader ReadMulti(ushort[] offsets)
        {
            return null;
        }

        public BinaryReader Read(int offset, int len)
        {
            Write("readplc " + offset + " " + len);
            Response resp =  ReadUntil();
            if(resp.String.Contains("Out of bounds!"))
                throw new IndexOutOfRangeException("Offset and/or length out of bounds.");
            return resp.BinaryReader;
        }

        public void WriteRaw(int offset, byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            byte[] cmd = ASCIIEncoding.ASCII.GetBytes("writeplc " + offset + " " + data.Length + " ");
            ms.Write(cmd, 0, cmd.Length);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            Cmd(ms.GetBuffer());
        }

        public void WriteRaw(int offset, int value)
        {
            MemoryStream ms = new MemoryStream(4);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        public void WriteRaw(int offset, float value)
        {
            MemoryStream ms = new MemoryStream(4);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        public void WriteRaw(int offset, ushort value)
        {
            MemoryStream ms = new MemoryStream(2);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        public DateTime Time
        {
            get
            {
                return DateTime.ParseExact(Cmd("time").String.Substring(0, 19), "HH:mm:ss dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            set
            {
                string str = "settime " + value.ToString("HH:mm:ss dd-MM-yy " + (int)value.DayOfWeek);
                Cmd(str).String.Contains("OK!");
            }
        }

        public string Password
        {
            get
            {
                return m_pass;
            }
            set
            {
                if (Cmd("passwd " + value).String.Contains("success"))
                    m_pass = Password;
                else
                    log.Error("Password could not be changed.");
            }
        }

        public int Revision
        {
            get
            {
                Regex r = new Regex("Revision: ([0-9]{3})");
                Match m = r.Match(Cmd("info").String);
                return Int32.Parse(m.Groups[1].Value);
            }
        }


        public int Model
        {
            get
            {
                return Int32.Parse(Cmd("model").String);
            }
        }

        public void ConfigureNetwork(IPAddress ip, IPAddress nm, IPAddress gw, IPAddress dns)
        {
            string cmd = String.Format("ifconfig 0x{0} 0x{1} 0x{2} 0x{3}", IP2Int(ip).ToString("X"), IP2Int(nm).ToString("X"), IP2Int(gw).ToString("X"), IP2Int(dns).ToString("X"));
            Cmd(cmd);
            if(IP2Int(ip) != 0)
                m_pip = ip;
        }

        public IPAddress IPAddress
        {
            get
            {
                UpdateNetwork();
                return m_ip;
            }
        }

        public IPAddress Netmask
        {
            get
            {
                UpdateNetwork();
                return m_nm;
            }
        }

        public IPAddress Gateway
        {
            get
            {
                UpdateNetwork();
                return m_gw;
            }
        }

        public IPAddress DNS
        {
            get
            {
                UpdateNetwork();
                return m_dns;
            }
        }
    }
}
