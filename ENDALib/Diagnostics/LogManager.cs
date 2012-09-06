using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

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

        private static int m_pid;
        private static string m_id;

        public delegate void LogHandler(Level lvl, DateTime t, string source, string msg);
        public static event LogHandler LogFired;
        public static Level Filter = Level.Debug;
        public static bool Enabled = true;

        static LogManager()
        {
            m_id = Process.GetCurrentProcess().ProcessName;
            m_pid = Process.GetCurrentProcess().Id;
        }

        public static void Log(Level lvl, string source, string msg)
        {
            if (!Enabled) return;
            if (lvl < Filter) return;
            DateTime t = DateTime.Now;
            if (LogFired != null)
                LogFired(lvl, t, source, msg);
            File.AppendAllText(m_id + ".txt", "[" + t.ToString("HH:mm:ss.fff") + "] [" + m_pid + "] [" + lvl + "] [" + source + "] " + msg);
        }
    }
}
