using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENDAPLCNetLib.Accessors
{
    public class FloatArrayAccessor : ArrayAccessor
    {
        public FloatArrayAccessor(PLC plc, int offset, int elementSize)
            : base(plc, offset, elementSize)
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
                PLC.WriteRaw(Offset + index * ElmSize, value);
            }
        }
    }
}
