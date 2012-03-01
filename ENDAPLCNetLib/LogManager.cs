using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace ENDA.PLCNetLib.Diagnostics
{
    public static class LogManager
    {
        public enum Level
        {
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            None = Int32.MaxValue
        }

        private static object m_lock = new object();
        private static StreamWriter m_ts = File.CreateText(Assembly.GetCallingAssembly().GetName().Name + ".txt");
        public static Level Filter = Level.Debug;

        public static bool Enabled = true;

        public static void Log(Level lvl, string source, string msg)
        {
            if (!Enabled) return;
            lock (m_lock)
            {
                if (lvl < Filter) return;
                m_ts.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] [" + lvl + "] [" + source + "] " + msg);
                m_ts.Flush();
            }
        }
    }
}
