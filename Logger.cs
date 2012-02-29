using System;
using System.Collections.Generic;
using System.Text;

namespace ENDAPLCNetLib.Diagnostics
{
    public class Logger
    {
        string m_source;

        public Logger(string source)
        {
            m_source = source;
        }

        public void Debug(string str)
        {
            LogManager.Log(LogManager.Level.Debug, m_source, str);
        }

        public void Info(string str)
        {
            LogManager.Log(LogManager.Level.Info, m_source, str);
        }

        public void Warning(string str)
        {
            LogManager.Log(LogManager.Level.Warning, m_source, str);
        }

        public void Error(string str)
        {
            LogManager.Log(LogManager.Level.Error, m_source, str);
        }
    }
}
