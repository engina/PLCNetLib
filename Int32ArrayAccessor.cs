using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENDAPLCNetLib.Accessors
{
    public class Int32ArrayAccessor : ArrayAccessor
    {
        public Int32ArrayAccessor(PLC plc, int offset, int elementSize)
            : base(plc, offset, elementSize)
        {
        }

        public int this[int index]
        {
            get
            {
                return PLC.Read(Offset + index * ElmSize, ElmSize).ReadInt32();
            }
            set
            {
                PLC.WriteRaw(Offset + index * ElmSize, value);
            }
        }
    }
}
