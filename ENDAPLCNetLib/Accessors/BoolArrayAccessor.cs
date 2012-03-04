using System;
using System.Collections.Generic;
using System.Text;

namespace ENDA.PLCNetLib.Accessors
{
    public class BoolArrayAccessor : ArrayAccessor
    {
        public BoolArrayAccessor(PLC plc, int offset)
            : base(plc, offset, 1)
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
