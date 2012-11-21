using System;
using System.Collections.Generic;
using System.Text;

namespace ENDA.PLCNetLib.Accessors
{
    public class Int32ArrayAccessor : ArrayAccessor
    {
        public Int32ArrayAccessor(PLC plc, int offset)
            : base(plc, offset, 4)
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
                PLC.Write(Offset + index * ElmSize, value);
            }
        }
    }
}
