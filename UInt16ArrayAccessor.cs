using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENDAPLCNetLib
{
    public class UInt16ArrayAccessor : ArrayAccessor
    {
        public UInt16ArrayAccessor(PLC plc, int offset, int elementSize) : base(plc, offset, elementSize)
        {
        }

        UInt16 this[int index]
        {
            get
            {
                return PLC.Read(Offset + index * ElmSize, ElmSize).ReadUInt16();
            }
            set
            {
                PLC.WriteRaw(Offset + index * ElmSize, value);
            }
        }
    }
}
