using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENDAPLCNetLib
{
    public class BoolArrayAccessor : ArrayAccessor
    {
        public BoolArrayAccessor(PLC plc, int offset, int elementSize)
            : base(plc, offset, elementSize)
        {
        }

        public bool this[int index]
        {
            get
            {
                return PLC.Read(Offset + index * ElmSize, ElmSize).ReadByte() != 0;
            }
            set
            {
                PLC.WriteRaw(Offset + index * ElmSize, new byte[]{(byte)(value ? 1 : 0)});
            }
        }
    }
}
