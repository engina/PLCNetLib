using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ENDAPLCNetLib
{
    /// <summary>
    /// This is a helper class that represents a telnet response received from PLC.
    /// <p>
    /// It lets you interpret the response, as regular string or binary data.
    /// </p>
    /// </summary>
    public class Response
    {
        MemoryStream m_ms;

        internal Response(MemoryStream ms)
        {
            m_ms = ms;
        }

        /// <summary>
        /// Returns the response as a raw memory stream, so you can process it anyway you like. It is recommend to use other alternatives.
        /// </summary>
        /// <see cref="BinaryReader"/>
        /// <see cref="String"/>
        public MemoryStream MemoryStream
        {
            get
            {
                return m_ms;
            }
        }

        /// <summary>
        /// Returns the response as BinaryReader. This is incredibly useful for reading binary data from the response, such as integers and shorts.
        /// </summary>
        public BinaryReader BinaryReader
        {
            get
            {
                return new BinaryReader(m_ms);
            }
        }

        /// <summary>
        /// Returns the response as a string.
        /// </summary>
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
