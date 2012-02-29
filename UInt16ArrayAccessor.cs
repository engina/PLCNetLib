using System;
using System.Collections.Generic;
using System.Text;

namespace ENDAPLCNetLib.Accessors
{
    public class UInt16ArrayAccessor : ArrayAccessor
    {
        public UInt16ArrayAccessor(PLC plc, int offset) : base(plc, offset, 2)
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
