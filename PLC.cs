using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using ENDAPLCNetLib.Accessors;
using ENDAPLCNetLib.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace ENDAPLCNetLib
{
    /// <summary>
    /// Represents a PLC device.
    /// 
    /// <p>
    /// Please note that all of the functions except the ones beginning with Begin*
    /// and End* are blocking and calling these functions will block until a response
    /// is received from the PLC device. If there's a communication problem your
    /// application will block (freeze) until a timeout occur.
    /// </p>
    /// <p>
    /// You can either use blocking methods in a separate thread or use advanced
    /// asynchronous methods for a non blocking usage.
    /// </p>
    /// </summary>
    public class PLC
    {
        TcpClient m_tcp = new TcpClient();
        IPEndPoint m_pip;
        string m_pass;
        byte[] m_lastCmd;
        IPAddress m_ip, m_nm, m_gw, m_dns;
        Logger log;

        /// <summary>
        /// <p>
        /// Initiates a PLC object. Does not automatically connect until said.
        /// <see cref="Finder"/> can be used to find IP addresses of PLC devices
        /// from PLC serial numbers (MAC addresses).
        /// </p>
        /// </summary>
        /// <param name="ip">IP of the PLC</param>
        /// <param name="password">Password of the PLC</param>
        /// <seealso cref="Connect"/>
        public PLC(IPEndPoint ip, string password)
        {
            log = new Logger("PLC[" + ip + "]");
            m_pip = ip;
            m_pass = password;
            MI = new Int32ArrayAccessor(this, 0);
            MF = new FloatArrayAccessor(this, 4096);
            MB = new BoolArrayAccessor(this, 8192);
            QP = new BoolArrayAccessor(this, 9856);
            IP = new BoolArrayAccessor(this, 10880);
            MW = new UInt16ArrayAccessor(this, 16896);
            
        }

        /// <summary>
        /// Connects to said PLC with given IP and password on default port 23.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="password"></param>
        /// <see cref="PLC(IPEndPoint ip, string password)"/>
        public PLC(IPAddress ip, string password)
            : this(new IPEndPoint(ip, 23), password)
        {
        }

        #region Privates

        /// <summary>
        /// Waits until any of the strings in the <paramref name="untils"/> array matched.
        /// </summary>
        /// <param name="untils">string of arrays to be wait for</param>
        /// <returns></returns>
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
            log.Debug("Recv: \"" + ASCIIEncoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length));
            ms.Position = 0;
            return new Response(ms);
        }

        /// <summary>
        /// Reads until a <paramref name="str"/> matched
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Response ReadUntil(string str)
        {
            return ReadUntil(new String[] { str });
        }

        /// <summary>
        /// Reads until command prompt.
        /// </summary>
        /// <returns></returns>
        Response ReadUntil()
        {
            return ReadUntil("\r\n> ");
        }

        /// <summary>
        /// Rewrites last command
        /// </summary>
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

            try
            {
                ns.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                log.Error("An error occured while writing data: " + e.Message);
            }
        }

        void Write(string str)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(str));
        }

        internal static int IP2Int(IPAddress addr)
        {
            return BitConverter.ToInt32(addr.GetAddressBytes(), 0);
        }

        internal static IPAddress Int2IP(int addr)
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
        #endregion

        #region Async variants of some public methods
        delegate Response CmdByteDelegate(byte[] buf);

        /// <summary>
        /// <p>
        /// Sends <paramref name="buf"/> byte array command to the PLC asynchronously.
        /// </p>
        /// <p>
        /// This method returns immediately. When the response is received <paramref name="cb"/>
        /// will be invoked.
        /// </p>
        /// </summary>
        /// 
        /// <example>
        /// <code>
        /// 
        ///private void ResponseHandler(IAsyncResult ar)
        ///{
        ///    PLC plc = (PLC)ar.AsyncState;
        ///    Response resp = plc.EndCmd(ar);
        ///    if (resp.String.Contains("OK!"))
        ///    {
        ///        MessageBox.Show("Run command succesfull");
        ///    }
        ///    else
        ///    {
        ///        MessageBox.Show("Run command failed");
        ///    }
        ///}
        ///
        ///private void runB_Click(object sender, EventArgs e)
        ///{
        ///    Object i = plcLB.SelectedItem;
        ///    if (i == null) return;
        ///    PLC plc = (PLC)i;
        ///
        ///   plc.BeginCmd(ASCIIEncoding.ASCII.GetBytes("run"), new AsyncCallback(ResponseHandler), plc);
        ///}
        /// 
        /// </code>
        /// </example>
        /// <param name="buf">
        /// </param>/// <param name="cb">Callback to be called when the operation is complete.</param>
        /// <param name="state">Any state to be passed to <paramref name="cb"/> when invoked, it's a good idea to use the PLC instance as state.</param>
        /// <returns></returns>
        public IAsyncResult BeginCmd(byte[] buf, AsyncCallback cb, object state)
        {
            log.Debug("BeginCmd");
            CmdByteDelegate m_cmdFunc = Cmd;
            return m_cmdFunc.BeginInvoke(buf, cb, state);
        }

        /// <summary>
        /// Ends a BeginCmd asynchronous call.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Response of the sent command</returns>
        /// <seealso cref="BeginCmd"/>
        public Response EndCmd(IAsyncResult ar)
        {
            log.Debug("EndCmd");
            AsyncResult a = (AsyncResult)ar;
            CmdByteDelegate d = (CmdByteDelegate)a.AsyncDelegate;
            return d.EndInvoke(ar);
        }

        delegate void BeginWriteRawDelegate(int offset, byte[] data);

        /// <summary>
        /// Writes raw data to PLC memory in the given <paramref name="offset"/> asynchronously.
        /// <p>
        /// When the write operation is complete <paramref name="cb"/> will be invoked (from another thread).
        /// </p>
        /// </summary>
        /// 
        /// <example>
        /// <code>
        ///         
        ///void WriteRawAsyncHandler(IAsyncResult ar)
        ///{
        ///    AsyncResult a = (AsyncResult)ar;
        ///    PLC p = (PLC)a.AsyncState;
        ///    p.EndWriteRaw(ar);
        ///}
        ///
        ///private void massWriteB_Click(object sender, EventArgs e)
        ///{
        ///    Object o = plcLB.SelectedItem;
        ///    if (o == null) return;
        ///    PLC plc = (PLC)o;
        ///    MemoryStream ms = new MemoryStream();
        ///    BinaryWriter bw = new BinaryWriter(ms);
        ///    for (int i = 0; i &lt; 100; i++)
        ///        bw.Write(i*2);
        ///    // Offset 0 is MI registers. So this code writes 100 integers to MI registers
        ///    // starting from offset 0. Which means MI[0] will be 0, MI[1] will be 2, MI[2] will be 4
        ///    // and so on. Same pattern can be used to write many MW, MF or MB at once.
        ///    plc.BeginWriteRaw(0, ms.GetBuffer(), new AsyncCallback(WriteRawAsyncHandler), plc);
        ///}
        /// </code>
        /// </example>
        /// <param name="offset">Offset of the memory location in the whole PLC memory in bytes.</param>
        /// <param name="data">Byte array of the data you want to write. This can contain sequential 4-byte integers, floats or 2-byte unsigned hosrts or 1-byte data.</param>
        /// <param name="cb">Callback to be called when the operation is complete.</param>
        /// <param name="state">Any state to be passed to <paramref name="cb"/> when invoked, it's a good idea to use the PLC instance as state.</param>
        /// <returns></returns>
        public IAsyncResult BeginWriteRaw(int offset, byte[] data, AsyncCallback cb, object state)
        {
            log.Debug("BeginWriteRaw");
            BeginWriteRawDelegate d = WriteRaw;
            return d.BeginInvoke(offset, data, cb, state);
        }

        /// <summary>
        /// Ends a asynchronous BeginWriteRaw call.
        /// </summary>
        /// <param name="ar"></param>
        /// <seealso cref="BeginWriteRaw"/>
        public void EndWriteRaw(IAsyncResult ar)
        {
            log.Debug("EndWriteRaw");
            AsyncResult a = (AsyncResult)ar;
            BeginWriteRawDelegate d = (BeginWriteRawDelegate)a.AsyncDelegate;
            d.EndInvoke(ar);
        }

        delegate BinaryReader ReadMultiDelegate(ushort[] offsets);

        /// <summary>
        /// Begins an asynchronous multiple read.
        /// </summary>
        /// <param name="offsets"></param>
        /// <param name="cb"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginReadMulti(ushort[] offsets, AsyncCallback cb, object state)
        {
            ReadMultiDelegate d = ReadMulti;
            return d.BeginInvoke(offsets, cb, state);
        }

        public BinaryReader EndReadMulti(IAsyncResult ar)
        {
            AsyncResult a = (AsyncResult)ar;
            ReadMultiDelegate d = (ReadMultiDelegate)a.AsyncDelegate;
            return d.EndInvoke(ar);
        }

        delegate BinaryReader ReadDelegate(int offset, int len);
        /// <summary>
        /// Starts an async reading.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <param name="cb"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginRead(int offset, int len, AsyncCallback cb, object state)
        {
            ReadDelegate d = Read;
            return d.BeginInvoke(offset, len, cb, state);
        }

        public BinaryReader EndRead(IAsyncResult ar)
        {
            AsyncResult a = (AsyncResult)ar;
            ReadDelegate d = (ReadDelegate)a.AsyncDelegate;
            return d.EndInvoke(ar);
        }
        #endregion

        #region public methods

        /// <summary>
        /// <p>Attempts to connect to the PLC. This method will be automatically called
        /// if a data send attempt is made prior to connecting, partly because of
        /// your convenience and partly for making the library handle disconnections better.</p>
        /// <p>
        /// It is a good practice to initally call this method first to check if the password is correct.
        /// </p>
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
        #endregion
    }
}
