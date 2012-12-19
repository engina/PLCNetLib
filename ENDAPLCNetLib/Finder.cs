using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ENDA.Diagnostics;
using System.IO;

namespace ENDA.PLCNetLib
{
    /// <summary>
    /// <p>This helper class helps you find IP addresses of the PLC devices on your network
    /// by their serial numbers (MAC) in a non-blocking manner.
    /// </p>
    /// <p>
    /// 
    /// </p>
    /// </summary>
    /// <example>
    /// <code>
    /// Finder f = new Finder();
    /// f.Scan();
    /// IPAddress ip = f.GetIp("00:25:fc:00:01:02");
    /// </code>
    /// </example>
    public class Finder
    {
        Logger log = new Logger("Finder");
        List<UdpClient> m_udpClients = new List<UdpClient>();
        IPEndPoint m_remoteEP = new IPEndPoint(IPAddress.Broadcast, 3802);
        Dictionary<string, IPAddress> m_dict = new Dictionary<string, IPAddress>();

        /// <summary>
        /// Handler that will be invoked when a new device is found.
        /// </summary>
        /// <param name="mac">MAC address of the device just found</param>
        /// <param name="ip">IP address of the device</param>
        public delegate void DeviceFoundHandler(string mac, IPAddress ip);

        /// <summary>
        /// This even will be fired when a new device is found.
        /// </summary>
        public event DeviceFoundHandler DeviceFound;

        /// <exception cref="SocketException">Thrown when scanner cannot bind to local port 3802. Can happen when another process is using that local port.</exception>
        public Finder()
        {
        }
        
        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient client = (UdpClient)ar.AsyncState;
            byte[] data;
            try
            {
                data = client.EndReceive(ar, ref m_remoteEP);
                if (data.Length < 6)
                {
                    client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
                    return;
                }
                BinaryReader br = new BinaryReader(new MemoryStream(data));
                String str = String.Empty;
                for (int i = 0; i < 6; i++)
                    str += Convert.ToString(br.ReadByte(), 16).PadLeft(2, '0') + ":";
                str = str.Substring(0, str.Length - 1);
                if (data.Length > 6)
                {
                    byte proto = br.ReadByte();
                    if (proto == 2)
                    {
                        int rev = br.ReadInt32();
                        //string label = br.ReadString();
                    }
                }
                if (!m_dict.ContainsKey(str) || !m_dict[str].Equals(m_remoteEP.Address))
                {
                    m_dict[str] = m_remoteEP.Address;
                    if (DeviceFound != null)
                        DeviceFound(str, m_dict[str]);
                }
                client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
            }
            catch (ObjectDisposedException e)
            {
            }
        }

        /// <summary>
        /// <p>
        /// Starts a scan. Results will be gathered as PLC devices start sending their informations. This method is not blocking.
        /// </p>
        /// You can wait for <paramref name="DeviceFound"/> event to be fired to be notified about any new devices found.
        /// <p>
        /// </p>
        /// </summary>
        Timer m_t;
        public void Scan()
        {
            // Rebind to ports if they are released by ReleasePorts()
            if (m_udpClients.Count == 0)
            {
                NetworkInterface[] ifs = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface i in ifs)
                {
                    Console.Out.WriteLine(i.Name);
                    if (i.OperationalStatus != OperationalStatus.Up) continue;
                    if (!i.Supports(NetworkInterfaceComponent.IPv4)) continue;
                    IPInterfaceProperties prop = i.GetIPProperties();

                    foreach (UnicastIPAddressInformation ip in prop.UnicastAddresses)
                    {

                        if (ip.Address.GetAddressBytes().Length != 4) continue;
                        UdpClient client = new UdpClient(new IPEndPoint(ip.Address, 3802));
                        client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
                        client.EnableBroadcast = true;
                        m_udpClients.Add(client);
                    }
                }
            }
            foreach (UdpClient client in m_udpClients)
            {
                client.Send(Encoding.ASCII.GetBytes("PING"), 4, m_remoteEP);
            }

            // Release the port after some time, as other programs such as EndaSoft might want to use them.
            m_t = new Timer((x) => { ReleasePorts(); }, null, 10000, System.Threading.Timeout.Infinite);
        }

        public void ReleasePorts()
        {
            foreach(UdpClient client in m_udpClients)
            {
                try
                {
                    client.Close();
                }
                catch (SocketException e)
                {
                    log.Error(e.Message);
                }
            }
            m_udpClients.Clear();
        }

        IPAddress GetIP(string mac)
        {
            return m_dict[mac];
        }
    }
}
