using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ENDAPLCNetLib
{
    public class Finder
    {
        Logger log = new Logger("Finder");
        List<UdpClient> m_udpClients = new List<UdpClient>();
        IPEndPoint m_remoteEP = new IPEndPoint(IPAddress.Broadcast, 3802);
        Dictionary<string, IPEndPoint> m_dict = new Dictionary<string, IPEndPoint>();

        public delegate void DeviceFoundHandler(string mac, IPEndPoint ip);
        public event DeviceFoundHandler DeviceFound;

        public Finder()
        {
            NetworkInterface[] ifs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface i in ifs)
            {
                if (!i.Supports(NetworkInterfaceComponent.IPv4)) continue;
                IPInterfaceProperties prop = i.GetIPProperties();

                foreach (UnicastIPAddressInformation ip in prop.UnicastAddresses)
                {

                    if (ip.Address.GetAddressBytes().Length != 4) continue;
                    try
                    {
                        UdpClient client = new UdpClient(new IPEndPoint(ip.Address, 3802));

                        client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
                        client.EnableBroadcast = true;

                        m_udpClients.Add(client);
                    }
                    catch (Exception e)
                    {
                        log.Warning("Could not bind to " + ip.Address + ":3802 on interface " + i.Description);
                    }
                }
            }
        }
        
        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient client = (UdpClient)ar.AsyncState;
            byte[] data = client.EndReceive(ar, ref m_remoteEP);

            if (data.Length != 6)
            {
                client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
                return;
            }

            String str = String.Empty;
            for (int i = 0; i < data.Length; i++)
                str += Convert.ToString(data[i], 16).PadLeft(2, '0') + ":";
            if (!m_dict.ContainsKey(str) || !m_dict[str].Equals(m_remoteEP))
            {
                m_dict[str] = new IPEndPoint(m_remoteEP.Address, m_remoteEP.Port);
                DeviceFound(str, m_dict[str]);
            }
            client.BeginReceive(new AsyncCallback(ReceiveCallback), client);
        }

        public void Scan()
        {
            m_remoteEP.Address = IPAddress.Broadcast;
            foreach (UdpClient client in m_udpClients)
                client.Send(Encoding.ASCII.GetBytes("PING"), 4, m_remoteEP);
        }

        IPEndPoint GetIP(string mac)
        {
            return m_dict[mac];
        }
    }
}
