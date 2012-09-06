using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ENDA.Diagnostics
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
        private static StreamWriter m_ts;
        
        public delegate void LogHandler(Level lvl, DateTime t, string source, string msg);
        public static event LogHandler LogFired;
        public static Level Filter = Level.Debug;
        public static bool Enabled = true;

        static LogManager()
        {
            m_ts = File.CreateText(Assembly.GetCallingAssembly().GetName().Name + ".txt");
        }

        public static void Log(Level lvl, string source, string msg)
        {
            lock (m_lock)
            {
                if (!Enabled) return;
                if (lvl < Filter) return;
                DateTime t = DateTime.Now;
                if (LogFired != null)
                    LogFired(lvl, t, source, msg);
                m_ts.WriteLine("[" + t.ToString("HH:mm:ss.fff") + "] [" + lvl + "] [" + source + "] " + msg);
                m_ts.Flush();
            }
        }
    }
}
