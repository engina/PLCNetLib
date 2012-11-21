using System;
using System.Collections.Generic;
using System.Text;

namespace ENDA.PLCNetLib.Accessors
{
    public class FloatArrayAccessor : ArrayAccessor
    {
        public FloatArrayAccessor(PLC plc, int offset)
            : base(plc, offset, 4)
        {
        }

        public float this[int index]
        {
            get
            {
                return PLC.Read(Offset + index * ElmSize, ElmSize).ReadSingle();
            }
            set
            {
                PLC.Write(Offset + index * ElmSize, value);
            }
        }
    }
}
