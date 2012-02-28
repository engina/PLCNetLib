using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENDAPLCNetLib.Accessors
{
    public class ArrayAccessor
    {
        PLC m_plc;
        int m_offset, m_elmSize;

        public ArrayAccessor(PLC plc, int offset, int elmSize)
        {
            m_plc = plc;
            m_offset = offset;
            m_elmSize = elmSize;
        }

        protected PLC PLC
        {
            get
            {
                return m_plc;
            }
        }

        protected int Offset
        {
            get
            {
                return m_offset;
            }
        }

        protected int ElmSize
        {
            get
            {
                return m_elmSize;
            }
        }
    }
}
