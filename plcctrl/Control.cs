using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ENDA.PLCNetLib;

namespace plcctrl
{
    public class Control
    {
        IPEndPoint m_ip = null;
        TcpClient m_client;
        string m_pass;
        PLC plc;
        public Control(IPEndPoint ip, string pass)
        {
            m_ip = ip;
            plc = new PLC(ip, pass);
            plc.Connect();
        }

        public bool UpdateFW(string path)
        {
            plc.Cmd("program enable");

            Console.Out.WriteLine("Programming enabled.");
            TcpClient downloader = new TcpClient();
            downloader.Connect(m_ip.Address, 61163);
            Socket sock = downloader.Client;
            byte[] data = new byte[16];
            sock.Receive(data);
            string str = ASCIIEncoding.ASCII.GetString(data);
            if (!str.StartsWith("Access granted"))
            {
                Console.Error.WriteLine("Access not granted");
                downloader.Close();
                return false;
            }

            Console.Out.Write("Sending firmware ");
            FileStream fs = new FileStream(path, FileMode.Open);

            data = new byte[1024];
            int r = 0;
            while((r = fs.Read(data,0,data.Length))!=0)
            {
                downloader.Client.Send(data, r, 0);
                Console.Out.Write(".");
            }
            data = new byte[22];
            sock.Receive(data);
            str = ASCIIEncoding.ASCII.GetString(data);
            if (!str.StartsWith("Firmware programmed"))
            {
                Console.Error.WriteLine("Could not program firmware.");
                downloader.Close();
                return false;
            }
            Console.Out.WriteLine("\nProgrammed. Rebooting.");
            Cmd("reboot");
            return true;
        }

        public bool Prog(string path)
        {
            plc.Cmd("program enable");
            
            TcpClient downloader = new TcpClient();
            downloader.Connect(m_ip.Address, 61163);
            Socket sock = downloader.Client;
            byte[] data = new byte[16];
            sock.Receive(data);
            string str = ASCIIEncoding.ASCII.GetString(data);
            if (!str.StartsWith("Access granted"))
            {
                Console.Error.WriteLine("Access not granted");
                downloader.Close();
                return false;
            }

            Console.Out.Write("Sending program ");
            FileStream fs = new FileStream(path, FileMode.Open);

            data = new byte[1024];
            int r = 0;
            while ((r = fs.Read(data, 0, data.Length)) != 0)
            {
                downloader.Client.Send(data, r, 0);
                Console.Out.Write(".");
            }
            data = new byte[22];
            sock.Receive(data);
            str = ASCIIEncoding.ASCII.GetString(data);
            if (!str.StartsWith("Programmed"))
            {
                Console.Error.WriteLine("Could not programmed.");
                downloader.Close();
                return false;
            }
            return true;
        }

        public bool ProgRun(string path)
        {
            return Prog(path) && Run();
        }

        public bool Run()
        {
            return plc.Run();
        }

        public bool Stop()
        {
            return plc.Stop();
        }

        internal void Cmd(string p)
        {
            plc.Cmd(p);
        }
    }
}
