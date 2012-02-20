using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ENDAPLCNetLib
{
    public class Response
    {
        MemoryStream m_ms;

        public Response(MemoryStream ms)
        {
            m_ms = ms;
        }

        public MemoryStream MemoryStream
        {
            get
            {
                return m_ms;
            }
        }

        public BinaryReader BinaryReader
        {
            get
            {
                return new BinaryReader(m_ms);
            }
        }

        public String String
        {
            get
            {
                string str = ASCIIEncoding.ASCII.GetString(m_ms.GetBuffer(), 0, (int)m_ms.Length);
                str = str.Substring(0, str.Length - 4);
                return str;
            }
        }
    }
}
