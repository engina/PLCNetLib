using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Procurios.Public;
using System.IO;
using ENDA.Diagnostics;

namespace ENDA.Lib
{
    public class Config
    {
        private static readonly Logger log = new Logger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());
        Hashtable m_cfg;
        string m_path;

        public Config(string path)
        {
            Load(path);
        }

        public Config(Hashtable cfg)
        {
            Load(cfg);
        }

        public void Load(string path)
        {
            m_path = path;
            try
            {
                m_cfg = (Hashtable)JSON.JsonDecode(File.ReadAllText(path, Encoding.UTF8));
            }
            catch (Exception e)
            {
                m_cfg = new Hashtable();
                log.Warning("Could not load '" + path + "': " + e.Message + ". Creating a new one upon save.");
            }
        }

        public void Load(Hashtable cfg)
        {
            m_cfg = cfg;
        }

        public T Get<T>(string key, T dflt)
        {
            if (!m_cfg.ContainsKey(key)) return dflt;
            return (T)m_cfg[key];
        }

        public int GetInt(string key, int dflt)
        {
            return Get<int>(key, dflt);
        }

        public string GetString(string key, string dflt)
        {
            return Get<string>(key, dflt);
        }

        public Hashtable GetObject(string key, Hashtable dflt)
        {
            return Get<Hashtable>(key, dflt);
        }

        public Hashtable Root
        {
            get
            {
                return m_cfg;
            }
        }

        public void Save()
        {
            File.WriteAllText(m_path, JSON.JsonEncode(m_cfg), Encoding.UTF8);
        }
    }
}
