using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ENDA.PLCNetLib;
using System.Threading;

namespace plcctrl
{
    class Program
    {
        static string[] m_args;
        static AutoResetEvent m_wait = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            /*
            args = new string[4];
            args[0] = "00:25:fc:00:02:bd";
            args[1] = "1234";
            args[2] = "fw";
            args[3] = @"C:\users\engin\code\workspace-cpp\plc\app\plc.bin.enda";
             * */
            m_args = args;
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: plcctrl MAC|IP PASS ACTION PARAMS");
                return;
            }
            IPEndPoint ip = null;
            if (args[0].Contains(":"))
            {
                Finder f = new Finder();
                f.DeviceFound += new Finder.DeviceFoundHandler(f_DeviceFound);
                f.Scan();
                m_wait.WaitOne(5000);
            }
            else
            {
                ip = new IPEndPoint(IPAddress.Parse(args[0]), 23);
                doit(ip, args);
            }
        }

        static void doit(IPEndPoint ip, string[] args)
        {
            try
            {
                Console.Out.WriteLine("Trying to connect to " + ip.ToString());
                Control c = new Control(ip, args[1]);

                switch (args[2].ToLowerInvariant())
                {
                    case "fw":
                        c.UpdateFW(args[3]);
                        break;
                    case "prog":
                        c.Prog(args[3]);
                        break;
                    case "progrun":
                        c.ProgRun(args[3]);
                        break;
                    case "run":
                        c.Run();
                        break;
                    case "stop":
                        c.Stop();
                        break;
                    case "reboot":
                        c.Cmd("reboot");
                        break;
                    default:
                        Console.Error.WriteLine("Unknown action");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: " + e.Message);
            }
            m_wait.Set();
        }

        static void f_DeviceFound(string mac, IPAddress ip)
        {
            if (mac == m_args[0].ToLowerInvariant())
            {
                doit(new IPEndPoint(ip, 23), m_args);
            }
        }
    }
}
