using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace ENDAPLCNetLib
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

        public static void Log(Level lvl, string source, string msg)
        {
            lock (m_lock)
            {
                if (lvl < Filter) return;
                m_ts.WriteLine("[" + DateTime.Now + "] [" + lvl + "] [" + source + "] " + msg);
                m_ts.Flush();
            }
        }
    }
}
