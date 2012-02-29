using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using ENDAPLCNetLib.Accessors;
using ENDAPLCNetLib.Diagnostics;

namespace ENDAPLCNetLib
{
    /// <summary>
    /// Represents a PLC device.
    /// 
    /// Designed in a blocking fashion, whenever the connection is flaky any API calls
    /// will block your application.
    /// <p>So, if you are developing a GUI application take
    /// your precautions.</p>
    /// </summary>
    /// <remarks>
    /// hello
    /// there
    /// buddy
    /// </remarks>
    public class PLC
    {
        TcpClient m_tcp = new TcpClient();
        IPEndPoint m_pip;
        string m_pass;
        byte[] m_lastCmd;
        IPAddress m_ip, m_nm, m_gw, m_dns;
        Logger log;

        /// <summary>
        /// Initiates a PLC object. Does not automatically connect until said.
        /// <see cref="Finder"/> can be used to find IP addresses of PLC devices
        /// from PLC serial numbers.
        /// </summary>
        /// <param name="ip">IP of the PLC</param>
        /// <param name="password">Password of the PLC</param>
        /// <seealso cref="Connect"/>
        public PLC(IPEndPoint ip, string password)
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

        /// <summary>
        /// Attempts to connect to the PLC. This method will be automatically called
        /// if a data send attempt is made prior to connecting, partly because of
        /// your convenience and partly for making the library handle disconnections better.
        /// </summary>
        /// <exception cref="InvalidCredentialException">Thrown if the password is wrong.</exception>
        public void Connect()
        {
            log.Info("Connecting...");
            // In case m_tcp is disposed. Fixes #63.
            m_tcp = new TcpClient();
            try
            {
                m_tcp.Connect(m_pip);
            }
            catch (Exception e)
            {
                log.Error("Connect() error: " + e.Message);
                Connect();
                return;
            }
            ReadUntil("Password: ");
            Write(m_pass);
            string response = ReadUntil(new String[] { "Try again: ", "Password accepted\r\n> " }).String;
            if (!response.Contains("accepted"))
            {
                log.Error("Invalid password");
                m_tcp.Close();
                throw new InvalidCredentialException();
            }
        }


        Response ReadUntil(String[] untils)
        {
            NetworkStream ns = m_tcp.GetStream();
            MemoryStream ms = new MemoryStream();
            bool found = false;
            while (!found)
            {
                int ch = -1;
                try
                {
                    ch = ns.ReadByte();
                }
                catch (Exception e)
                {
                    log.Error("ReadUntil ReadByte() error: " + e.Message);
                }
                if (ch == -1)
                {
                    log.Error("Connection closed");
                    m_tcp.Close();
                    log.Info("Reconnecting");
                    // Save last saved command, because Connect() will overwrite it
                    byte[] currentLastCmd = m_lastCmd;
                    Connect();
                    log.Info("Rewriting last command");
                    // Restore attempted command and resend it after a succesful connection.
                    m_lastCmd = currentLastCmd;
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

        /// <summary>
        /// Sends a byte stream as a telnet command.
        /// </summary>
        /// <param name="buf">telnet command as byte array</param>
        /// <returns>Response</returns>
        public Response Cmd(byte[] buf)
        {
            Write(buf);
            return ReadUntil();
        }

        /// <summary>
        /// Sends a string as a telnet command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>Response</returns>
        public Response Cmd(string cmd)
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

        void UpdateNetwork()
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

        /// <summary>
        /// Sends run command to the PLC.
        /// </summary>
        /// <returns>true if program started running and false otherwise. false
        /// can be returned if there's no valid program in the PLC memory.</returns>
        public bool Run()
        {
            return Cmd("run").String.Contains("OK!");
        }

        /// <summary>
        /// Sends stop command to the PLC.
        /// </summary>
        /// <returns>true if PLC has succesfully stopped. false if a clean stop is failed.</returns>
        public bool Stop()
        {
            return Cmd("stop").String.Contains("Task stopped");
        }

        /// <summary>
        /// Sends a reboot command to the PLC. Your connection will be lost.
        /// </summary>
        public void Reboot()
        {
            Cmd("reboot");
        }

        /// <summary>
        /// Reads 4-byte values from PLC memory at arbitrary offsets given as 
        /// <paramref name="offsets"/>.  Usefull for reading non sequential data
        /// from PLC memory. Unlike alternatives, it does all the reading in a single network packet
        /// roundtrip, hence it is higher performance, yet harder to use.
        /// </summary>
        /// <example>
        /// PLC plc = new PLC("192.168.1.2", "1234");
        /// ushort[] offsets = new ushort[]{0, 4, 256, 260}
        /// // Will read a total of 16 bytes from the ranges 0-4, 4-8, 256-260, 260-264
        /// BinaryReader br = plc.ReadMulti(new ushort[]{0, 4, 256, 260});
        /// for(int i = 0; i &lt; offsets.length; i++)
        ///     Console.Out.WriteLine(br.ReadInt32());
        /// 
        /// </example>
        /// <param name="offsets">List of offsets in bytes.</param>
        /// <returns>A <see cref="BinaryReader"/> for you to conveniently read any data in the response.</returns>
        public BinaryReader ReadMulti(ushort[] offsets)
        {
            string cmd = "readplcm " + (offsets.Length*2) + " ";
            MemoryStream ms = new MemoryStream(cmd.Length + offsets.Length * 2);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(ASCIIEncoding.ASCII.GetBytes(cmd));
            foreach(ushort o in offsets)
                bw.Write(o);
            Write(ms.GetBuffer());
            return ReadUntil().BinaryReader;
        }

        /// <summary>
        /// Reads <paramref name="len"/> bytes of data from <paramref name="offset"/> sequentially.
        /// </summary>
        /// <param name="offset">Offset in bytes.</param>
        /// <param name="len">Length in bytes.</param>
        /// <returns>A <see cref="BinaryReader"/> for you to conveniently read any data in the response.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public BinaryReader Read(int offset, int len)
        {
            Write("readplc " + offset + " " + len);
            Response resp =  ReadUntil();
            if(resp.String.Contains("Out of bounds!"))
                throw new IndexOutOfRangeException("Offset and/or length out of bounds.");
            return resp.BinaryReader;
        }

        /// <summary>
        /// Writes whole <paramref name="data"/> to PLC memory at <paramref name="offset"/>, sequentially.
        /// </summary>
        /// <param name="offset">Offset in bytes.</param>
        /// <param name="data">Data to be written</param>
        public void WriteRaw(int offset, byte[] data)
        {
            byte[] cmd = ASCIIEncoding.ASCII.GetBytes("writeplc " + offset + " " + data.Length + " ");
            // Fixes #65
            MemoryStream ms = new MemoryStream(cmd.Length + data.Length);
            ms.Write(cmd, 0, cmd.Length);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            Response r = Cmd(ms.GetBuffer());
            Console.Out.WriteLine(r.String);
        }

        /// <summary>
        /// Writes an Int32 value to the given offset.
        /// </summary>
        /// <param name="offset">Offset in bytes</param>
        /// <param name="value">Int32 value</param>
        public void WriteRaw(int offset, int value)
        {
            MemoryStream ms = new MemoryStream(4);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        /// <summary>
        /// Writes a floating point value to the given offset.
        /// </summary>
        /// <param name="offset">Offset in bytes</param>
        /// <param name="value">Float value</param>
        public void WriteRaw(int offset, float value)
        {
            MemoryStream ms = new MemoryStream(4);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        /// <summary>
        /// Writes a UInt16 value to the given offset.
        /// </summary>
        /// <param name="offset">Offset in bytes</param>
        /// <param name="value">UInt16 value</param>
        public void WriteRaw(int offset, ushort value)
        {
            MemoryStream ms = new MemoryStream(2);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(value);
            WriteRaw(offset, ms.GetBuffer());
        }

        /// <summary>
        /// Sets or gets time of the RTC (Real Time Clock) of the PLC.
        /// IMPORTANT: This works only on 38x series of PLC devices, as only they contain a
        /// RTC hardware.
        /// </summary>
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

        /// <summary>
        /// Sets or gets the password for this device.
        /// Getting password does not result in any network communication, it is just the
        /// value you provided when initiating this object.
        /// </summary>
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

        /// <summary>
        /// Firmware revision of the PLC device.
        /// </summary>
        public int Revision
        {
            get
            {
                Regex r = new Regex("Revision: ([0-9]{3})");
                Match m = r.Match(Cmd("info").String);
                return Int32.Parse(m.Groups[1].Value);
            }
        }

        int Model
        {
            get
            {
                return Int32.Parse(Cmd("model").String);
            }
        }

        /// <summary>
        /// Configures the networking configuration of the PLC.
        /// 
        /// If either <paramref name="ip"/> or <paramref name="dns"/> is 0.0.0.0 or
        /// 255.255.255.255 the device puts itself in DHCP mode and tries to acquire
        /// IP automatically.
        /// 
        /// If your configuration changed any of <paramref name="ip"/>, <paramref name="nm"/>
        /// or <paramref name="gw"/> all connections to the device will be closed.
        /// 
        /// After sending this command, this object will update its IP configuration regardless of
        /// any response and will try to reconnect during next data send.
        /// 
        /// Note that reconnections will block.
        /// 
        /// Also, note that, if a not working DNS configuration is provided, WMI feature of the PLC
        /// won't be usable.
        /// </summary>
        /// <param name="ip">IP Address of the PLC</param>
        /// <param name="nm">Netmask</param>
        /// <param name="gw">Gateway IP</param>
        /// <param name="dns">DNS IP</param>
        public void ConfigureNetwork(IPAddress ip, IPAddress nm, IPAddress gw, IPAddress dns)
        {
            string cmd = String.Format("ifconfig 0x{0} 0x{1} 0x{2} 0x{3}", IP2Int(ip).ToString("X"), IP2Int(nm).ToString("X"), IP2Int(gw).ToString("X"), IP2Int(dns).ToString("X"));
            Cmd(cmd);
            if(IP2Int(ip) != 0)
                m_pip.Address = ip;
        }

        /// <summary>
        /// IP address of the PLC device.
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                UpdateNetwork();
                return m_ip;
            }
        }

        /// <summary>
        /// Netmask of the PLC device.
        /// </summary>
        public IPAddress Netmask
        {
            get
            {
                UpdateNetwork();
                return m_nm;
            }
        }

        /// <summary>
        /// Gateway of the PLC device.
        /// </summary>
        public IPAddress Gateway
        {
            get
            {
                UpdateNetwork();
                return m_gw;
            }
        }

        /// <summary>
        /// DNS of the PLC device.
        /// </summary>
        public IPAddress DNS
        {
            get
            {
                UpdateNetwork();
                return m_dns;
            }
        }

        /// <summary>
        /// MI registers of PLC, consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public Int32ArrayAccessor MI;

        /// <summary>
        /// MI registers of PLC. These are general purpose integer registers.
        /// Consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public FloatArrayAccessor MF;

        /// <summary>
        /// MI registers of PLC. These are general purpose integer registers.
        /// Consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public BoolArrayAccessor MB;

        /// <summary>
        /// MI registers of PLC. These are general purpose integer registers.
        /// Consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public BoolArrayAccessor QP;

        /// <summary>
        /// MI registers of PLC. These are general purpose integer registers.
        /// Consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public BoolArrayAccessor IP;

        /// <summary>
        /// MI registers of PLC. These are general purpose integer registers.
        /// Consists of 1024 4-byte integers.
        /// Each read and write attempt will result in a network transmission.
        /// <example>
        /// plc.MI[5] = 10;
        /// Console.Out.WriteLine(plc.MI[5]);
        /// </example>
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when offset and/or length is out of PLC memory boundaries.</exception>
        public UInt16ArrayAccessor MW;

        public override string ToString()
        {
            return m_pip.ToString();
        }
    }
}
